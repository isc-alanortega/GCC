using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Enums.Core;
using Nubetico.WebAPI.Application.External.CIEmail;
using Nubetico.WebAPI.Application.External.Directory;
using Nubetico.WebAPI.Application.External.Directory.Dto;
using Nubetico.WebAPI.Application.Modules.Core.Models;
using Nubetico.WebAPI.Application.Modules.Core.Models.Static;
using Nubetico.WebAPI.Application.Utils;
using System.Globalization;
using System.Net;
using System.Text;

namespace Nubetico.WebAPI.Application.Modules.Core.Services
{
    public class UsuariosService
    {
        private readonly DirectoryApiServices _directoryApiServices;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly JwtHandlerService _jwtHandlerService;
        private readonly TenantConnectionService _tenantConnectionService;
        private readonly CIEmailService _CIEmailService;

        public UsuariosService(
            DirectoryApiServices directoryApiServices,
            IDbContextFactory<CoreDbContext> coreDbContextFactory,
            IStringLocalizer<SharedResources> localizer,
            JwtHandlerService jwtHandlerService,
            TenantConnectionService tenantConnectionService,
            CIEmailService ciEmailService
            )
        {
            _directoryApiServices = directoryApiServices;
            _coreDbContextFactory = coreDbContextFactory;
            _localizer = localizer;
            _jwtHandlerService = jwtHandlerService;
            _tenantConnectionService = tenantConnectionService;
            _CIEmailService = ciEmailService;
        }

        public async Task<int> GetUserIdByGuidAsync(string userGuid)
        {
            using (var contextCore = _coreDbContextFactory.CreateDbContext())
            {
                var user = await contextCore.Usuarios.FirstOrDefaultAsync(user => user.GuidUsuarioDirectory.ToString() == userGuid);
                if (user == null)
                {
                    throw new Exception("No se encontró el ID del usuario en sesión.");
                }

                return user.IdUsuario;
            }
        }

        public async Task<Tuple<AuthResponseDto?, string>> GetPerfilUsuarioPorAutenticacionAsync(AuthRequestDto authDto)
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            var vUsuario = await coreDbContext.vUsuariosPersonas.Where(m => m.Username.Equals(authDto.Username)).FirstOrDefaultAsync();

            if (vUsuario == null)
                return new Tuple<AuthResponseDto?, string>(null, _localizer["Core.Errors.BadCredentials"]);

            // Serializar contenido del body
            var content = new StringContent(
                    JsonConvert.SerializeObject(new { authDto.Username, authDto.Password }),
                    Encoding.UTF8,
                    "application/json");

            // Ejecutar petición
            var directoryResponse = await _directoryApiServices.GetUsuarioByCredencialesAsync(content);

            // Validar posibles respuestas de directory
            if (directoryResponse.StatusCode == HttpStatusCode.BadRequest)
                return new Tuple<AuthResponseDto?, string>(null, _localizer["Core.Errors.ServerAuthError"]);

            if (directoryResponse.StatusCode == HttpStatusCode.NotFound)
                return new Tuple<AuthResponseDto?, string>(null, _localizer["Core.Errors.BadCredentials"]);

            if (directoryResponse.StatusCode == HttpStatusCode.InternalServerError)
                return new Tuple<AuthResponseDto?, string>(null, _localizer["Core.Errors.ServerAuthError"]);

            // Si todo es correcto, deseralizar el response
            var responseDirectoryContent = await directoryResponse.Content.ReadAsStringAsync();
            var usuarioDirectoryResponse = JsonConvert.DeserializeObject<BaseResponseDto<UsuarioDto>>(responseDirectoryContent);

            if (vUsuario.GuidUsuarioDirectory != usuarioDirectoryResponse.Data.UUID.Value)
                return new Tuple<AuthResponseDto?, string>(null, _localizer["Core.Errors.ProfileOnTenantNotFound"]);

            if (vUsuario.Bloqueado)
                return new Tuple<AuthResponseDto?, string>(null, (_localizer["Core.Users.CannotLogin"]).ToString().Replace("__STATUS__", _localizer["Core.Users.Blocked"].ToString().ToUpper()));

            if (vUsuario.UsuarioPuedeAutenticar == false)
                return new Tuple<AuthResponseDto?, string>(null, (_localizer["Core.Users.CannotLogin"]).ToString().Replace("__STATUS__", currentCulture == "en-US" ? vUsuario.UsuarioEstadoEN : vUsuario.UsuarioEstado));

            // Si el perfil de usuario requiere autenticación de dos factores y no envió un token, informar al cliente
            if (!string.IsNullOrEmpty(vUsuario.TwoFactorKey) && string.IsNullOrEmpty(authDto.Token))
            {
                return new Tuple<AuthResponseDto?, string>(new AuthResponseDto { PerfilUsuario = null, JwtData = null, TwoFactorRequired = true }, "");
            }

            if (!string.IsNullOrEmpty(vUsuario.TwoFactorKey))
            {
                //Validar token
                bool tokenValido = OtpHandler.TokenEsValido(authDto.Token, vUsuario.TwoFactorKey);

                if (!tokenValido)
                {
                    return new Tuple<AuthResponseDto?, string>(new AuthResponseDto { PerfilUsuario = null, JwtData = null, TwoFactorRequired = true }, _localizer["Core.Errors.WrongToken"]);
                }
            }

            var userJwtRequestModel = new UserJwtRequestModel
            {
                Id = usuarioDirectoryResponse.Data.UUID.ToString()
                    ?? throw new Exception("UserID not valid"),
                Username = usuarioDirectoryResponse.Data.Username,
                Nombre = $"{usuarioDirectoryResponse.Data.Nombres} {usuarioDirectoryResponse.Data.PrimerApellido} {usuarioDirectoryResponse.Data.SegundoApellido}".Trim(),
                Email = usuarioDirectoryResponse.Data.Email,
                TenantGuid = this.GetTenant().TenantGuid.ToString()
            };

            var vContactoEntidad = await coreDbContext.vUsuariosContactos
                .FirstOrDefaultAsync(v => v.IdUsuario == vUsuario.IdUsuario);

            if(vContactoEntidad != null)
            {
                var tipo = Enum.IsDefined(typeof(TypeContactUserEnum), vContactoEntidad.IdTipoEntidad)
                    ? (TypeContactUserEnum)vContactoEntidad.IdTipoEntidad
                    : TypeContactUserEnum.None;

                EntidadContactoUsuarioDto entidadContacto = new EntidadContactoUsuarioDto
                {
                    Id = vContactoEntidad.UUID_Entidad,
                    Rfc = vContactoEntidad.RfcEntidad,
                    Tipo = tipo,
                    Nombre = vContactoEntidad.NombreComercial
                };

                userJwtRequestModel.EntidadContacto = entidadContacto;
            }

            AuthResponseDto authResponseDto = new AuthResponseDto
            {
                PerfilUsuario = new PerfilUsuarioDto
                {
                    Username = usuarioDirectoryResponse.Data.Username,
                    PrimerApellido = usuarioDirectoryResponse.Data.PrimerApellido,
                    SegundoApellido = usuarioDirectoryResponse.Data.SegundoApellido,
                    Nombre = usuarioDirectoryResponse.Data.Nombres,
                    NavegaTabs = vUsuario.NavegarTabsActivo
                },
                JwtData = _jwtHandlerService.UserJwtSigner(userJwtRequestModel),
                TwoFactorRequired = false,
                Roles = await coreDbContext.vUsuariosPermisos
                    .Where(m => m.IsRole && m.IdUsuario == vUsuario.IdUsuario)
                    .Select(v => new RolDto { Rol = v.Alias, IdSucursal = v.IdSucursal ?? -1 })
                    .ToListAsync()
            };

            return new Tuple<AuthResponseDto?, string>(authResponseDto, "");
        }

        public async Task<JwtDataDto?> GetTokenByUserGuidAsync(Guid guidUsuario)
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            var vUsuario = await coreDbContext.vUsuariosPersonas.Where(m => m.GuidUsuarioDirectory.Equals(guidUsuario)).FirstOrDefaultAsync();

            if (vUsuario == null) return null;

            var directoryResponse = await _directoryApiServices.GetUsuarioByGuidAsync(vUsuario.GuidUsuarioDirectory);

            // Validar posibles respuestas de directory
            if (directoryResponse.StatusCode == HttpStatusCode.BadRequest)
                throw new Exception(_localizer["Core.Errors.ServerAuthError"]);

            if (directoryResponse.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (directoryResponse.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception(_localizer["Core.Errors.ServerAuthError"]);

            var responseDirectoryContent = await directoryResponse.Content.ReadAsStringAsync();
            var usuarioDirectoryResponse = JsonConvert.DeserializeObject<BaseResponseDto<UsuarioDto>>(responseDirectoryContent) ?? throw new JsonSerializationException();

            if (vUsuario.Bloqueado)
                throw new Exception((_localizer["Core.Users.CannotLogin"]).ToString().Replace("__STATUS__", _localizer["Core.Users.Blocked"].ToString().ToUpper()));

            if (vUsuario.UsuarioPuedeAutenticar == false)
                throw new Exception((_localizer["Core.Users.CannotLogin"]).ToString().Replace("__STATUS__", currentCulture == "en-US" ? vUsuario.UsuarioEstadoEN : vUsuario.UsuarioEstado));

            var userJwtRequestModel = new UserJwtRequestModel
            {
                Id = vUsuario.GuidUsuarioDirectory.ToString()
                    ?? throw new Exception("UserID not valid"),
                Username = vUsuario.Username,
                Email = vUsuario.Email,
                TenantGuid = this.GetTenant().TenantGuid.ToString()
            };

            return _jwtHandlerService.UserJwtSigner(userJwtRequestModel);
        }

        public async Task<PaginatedListDto<UsuarioNubeticoGridDto>?> GetUsuariosPaginadoAsync(int limit, int offset, string? orderBy, string? username, string? nombreCompleto, int? idEstadoUsuario)
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;

            // Clase auxiliar para mapear las propiedades de los campos Dto a una propiedad de la clase del modelo
            var fieldMappings = new List<Dto2ModelHandler<vUsuariosPersonas>>
            {
                new Dto2ModelHandler<vUsuariosPersonas>("NombreCompleto", m => m.NombreCompleto),
                new Dto2ModelHandler<vUsuariosPersonas>("Nombres", m => m.Nombres),
                new Dto2ModelHandler<vUsuariosPersonas>("PrimerApellido", m => m.PrimerApellido),
                new Dto2ModelHandler<vUsuariosPersonas>("SegundoApellido", m => m.SegundoApellido),
                new Dto2ModelHandler<vUsuariosPersonas>("Email", m => m.Email),
                new Dto2ModelHandler<vUsuariosPersonas>("Username", m => m.Username),
                new Dto2ModelHandler<vUsuariosPersonas>("IdEstadoUsuario", m => m.IdUsuarioEstado),
                new Dto2ModelHandler<vUsuariosPersonas>("EstadoUsuario", m => m.UsuarioEstado)
            };

            string[] orderParts = orderBy?.Split(' ') ?? Array.Empty<string>();

            // Ordenar por defecto por nombre completo
            string fieldName = orderParts.ElementAtOrDefault(0) ?? "NombreCompleto";
            string sortDirection = orderParts.ElementAtOrDefault(1)?.ToLower() ?? "asc";

            // Instanciar BD
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            var query = coreDbContext.vUsuariosPersonas.AsQueryable();

            if (fieldName != "EstadoUsuario")
            {
                var sortMapping = fieldMappings.FirstOrDefault(f => f.FieldName == fieldName);
                var sortExpression = sortMapping?.Expression ?? fieldMappings.First().Expression;

                if (!string.IsNullOrWhiteSpace(username))
                    query = query.Where(m => m.Username.Contains(username));

                if (!string.IsNullOrWhiteSpace(nombreCompleto))
                    query = query.Where(m => m.NombreCompleto.Contains(nombreCompleto));

                if (idEstadoUsuario.HasValue)
                    query = idEstadoUsuario.Value != 0
                        ? query.Where(m => m.IdUsuarioEstado == idEstadoUsuario.Value && m.Bloqueado == false) : query.Where(m => m.Bloqueado == true);

                // Aplicar ordenamiento
                query = sortDirection == "desc"
                    ? query.OrderByDescending(sortExpression)
                    : query.OrderBy(sortExpression);
            }
            else
            {
                // Este ordenamiento considera que el usuario con bloqueo sobreescribe cualquier otro estatus
                query = sortDirection == "desc"
                    ? query.OrderByDescending(m => m.Bloqueado).ThenByDescending(m => m.UsuarioEstado)
                    : query.OrderBy(m => m.Bloqueado).ThenBy(m => m.UsuarioEstado);
            }

            PaginatedListDto<UsuarioNubeticoGridDto> result = new PaginatedListDto<UsuarioNubeticoGridDto>
            {
                RecordsTotal = await query.CountAsync(),
                Data = await query
                    .Skip(offset)
                    .Take(limit)
                    .Select(m => new UsuarioNubeticoGridDto
                    {
                        UUID = m.GuidUsuarioDirectory,
                        Username = m.Username,
                        Nombres = m.Nombres,
                        PrimerApellido = m.PrimerApellido,
                        SegundoApellido = m.SegundoApellido,
                        NombreCompleto = m.NombreCompleto,
                        Email = m.Email,
                        IdEstadoUsuario = !m.Bloqueado ? m.IdUsuarioEstado : 0,
                        EstadoUsuario = !m.Bloqueado
                            ? currentCulture == "en-US" ? m.UsuarioEstadoEN : m.UsuarioEstado
                            : (_localizer["Core.Users.Blocked"]).ToString().ToUpper()
                    })
                    .ToListAsync()
            };

            return result;
        }

        public async Task<UsuarioNubeticoDto?> GetUsuarioNubeticoByGuidAsync(Guid guid)
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;

            TenantModel? tenant = _tenantConnectionService.GetTenant();
            if (tenant == null)
                return null;

            if (tenant.TenantGuid == Guid.Empty)
                throw new Exception("Tenant not valid");

            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            var usuarioInstancia = await coreDbContext.vUsuariosPersonas.Where(m => m.GuidUsuarioDirectory == guid).FirstOrDefaultAsync();

            if (usuarioInstancia == null)
                return null;

            UsuarioNubeticoDto result = new UsuarioNubeticoDto
            {
                UUID = usuarioInstancia.GuidUsuarioDirectory,
                Username = usuarioInstancia.Username,
                Nombres = usuarioInstancia.Nombres,
                PrimerApellido = usuarioInstancia.PrimerApellido,
                SegundoApellido = usuarioInstancia.SegundoApellido,
                NombreCompleto = usuarioInstancia.NombreCompleto,
                Email = usuarioInstancia.Email,
                IdEstadoUsuario = !usuarioInstancia.Bloqueado ? usuarioInstancia.IdUsuarioEstado : 0,
                EstadoUsuario = !usuarioInstancia.Bloqueado
                            ? currentCulture == "en-US" ? usuarioInstancia.UsuarioEstadoEN : usuarioInstancia.UsuarioEstado
                            : (_localizer["Core.Users.Blocked"]).ToString().ToUpper(),
                Bloqueado = usuarioInstancia.Bloqueado,
                TwoFactorKey = usuarioInstancia.TwoFactorKey,
                TwoFactorUrl = usuarioInstancia.TwoFactorKey != null ? OtpHandler.GenerarUrl(tenant.Description, usuarioInstancia.Username, usuarioInstancia.TwoFactorKey) : null
            };

            var menuUsuario = await (from menu in coreDbContext.Menus
                                     join rel in coreDbContext.Usuarios_Menus
                                     on new { MenuId = menu.IdMenu, UsuarioId = usuarioInstancia.IdUsuario } equals new { MenuId = rel.IdMenu, UsuarioId = rel.IdUsuario }
                                     into userMenus
                                     from rel in userMenus.DefaultIfEmpty()
                                     select new MenuDto
                                     {
                                         IdMenu = menu.IdMenu,
                                         Nombre = currentCulture == "en-US" ? menu.NombreEN : menu.Nombre,
                                         Nivel = menu.Nivel,
                                         IdMenuPadre = menu.IdMenuPadre,
                                         Habilitado = menu.Habilitado,
                                         Seleccionable = menu.RutaPagina != null,
                                         Check = rel.IdUsuarioMenu > 0
                                     }).ToListAsync();

            result.Permisos = await coreDbContext.vUsuariosPermisos
                .Where(m => m.IdUsuario == usuarioInstancia.IdUsuario)
                .Select(m => new PermisoAsignadoDto
                {
                    IdPermiso = m.IdPermiso,
                    Permiso = currentCulture == "en-US" ? m.DescripcionEN : m.Descripcion,
                    IdSucursal = m.IdSucursal ?? -1,
                    Sucursal = m.Sucursal ?? (currentCulture == "en-US" ? "ALL" : "TODAS"),
                    IdMenu = m.IdMenu ?? 0
                }).ToListAsync();

            result.Sucursales = await coreDbContext.vUsuariosSucursales
                .Where(m => m.IdUsuario == usuarioInstancia.IdUsuario)
                .Select(m => new SucursalDto
                {
                    IdSucursal = m.IdSucursal ?? -1,
                    Descripcion = m.Denominacion ?? (currentCulture == "en-US" ? "ALL" : "TODAS"),
                    CveSucursal = m.CveSucursal ?? string.Empty
                }).ToListAsync();

            List<MenuDto> MapMenus(List<MenuDto> menus, int? parentId = null)
            {
                return menus
                    .Where(m => m.IdMenuPadre == parentId || (parentId == null && m.IdMenuPadre == 0))
                    .OrderBy(m => m.Nivel)
                    .Select(m => new MenuDto
                    {
                        IdMenu = m.IdMenu,
                        Nombre = m.Nombre,
                        Nivel = m.Nivel,
                        IdMenuPadre = m.IdMenuPadre,
                        Habilitado = m.Habilitado,
                        Children = MapMenus(menus, m.IdMenu),
                        Check = m.Check,
                        Seleccionable = m.Seleccionable
                    })
                    .ToList();
            }

            result.ListaMenus = MapMenus(menuUsuario);

            return result;
        }

        public async Task<GeneratedTwoFactorCodeDto?> GetCodigoQrByGuidAsync(Guid guidUsuario, bool newCode)
        {
            UsuarioNubeticoDto? usuario = await GetUsuarioNubeticoByGuidAsync(guidUsuario);
            if (usuario == null) return null;

            if (!newCode)
            {
                if (usuario.TwoFactorKey == null) return null;

                return new GeneratedTwoFactorCodeDto
                {
                    KeyGenerated = usuario.TwoFactorKey,
                    QrImage64 = QrHandler.QrBase64(usuario.TwoFactorUrl)
                };
            }

            TenantModel? tenant = _tenantConnectionService.GetTenant();
            if (tenant == null) return null;

            var twoFactorKey = OtpHandler.GenerarSecret();
            var twoFactorUrl = OtpHandler.GenerarUrl(tenant.Description, usuario.Username, twoFactorKey);

            return new GeneratedTwoFactorCodeDto
            {
                KeyGenerated = twoFactorKey,
                QrImage64 = QrHandler.QrBase64(twoFactorUrl)
            };
        }

        public async Task<List<BasicItemSelectDto>> GetUserStatusListAsync()
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;

            using var coreDbContext = _coreDbContextFactory.CreateDbContext();

            List<BasicItemSelectDto> result = await coreDbContext.Usuarios_Estados
                .Select(m => new BasicItemSelectDto
                {
                    Value = m.IdUsuarioEstado,
                    Text = currentCulture == "en-US" ? m.EstadoEN : m.Estado
                }).ToListAsync();

            result.Add(new BasicItemSelectDto { Value = 0, Text = currentCulture == "en-US" ? "BLOCKED" : "BLOQUEADO" });

            return result;
        }

        public async Task<UsuarioNubeticoDto> SetInsertUsuarioAsync(UsuarioNubeticoDto usuarioNubeticoDto, string guidUsuarioAlta, string httpScheme)
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;
            var idUsuarioAlta = await this.GetUserIdByGuidAsync(guidUsuarioAlta);
            TenantModel tenant = GetTenant();

            // Normalizar datos
            usuarioNubeticoDto.Nombres = usuarioNubeticoDto.Nombres.Trim().ToUpper();
            usuarioNubeticoDto.PrimerApellido = usuarioNubeticoDto.PrimerApellido.Trim().ToUpper();
            usuarioNubeticoDto.SegundoApellido = usuarioNubeticoDto.SegundoApellido.Trim().ToUpper();
            usuarioNubeticoDto.Email = usuarioNubeticoDto.Email.Trim();
            usuarioNubeticoDto.Username = usuarioNubeticoDto.Username.Trim();

            // Buscar usuario por Email en Directory
            var responseGetUsuarioDirectory = await _directoryApiServices.GetUsuarioByEmailAsync(usuarioNubeticoDto.Email);
            var responseContentGetUsuarioDirectory = await responseGetUsuarioDirectory.Content.ReadAsStringAsync();
            var getUsuarioResponseDto = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContentGetUsuarioDirectory);

            string suggestedPwd = PasswordGenerator.GeneratePassword(8);

#if (DEBUG)
            Console.WriteLine($"Password: {suggestedPwd}");
#endif

            UsuarioDto usuarioDirectoryDto;

            if (getUsuarioResponseDto?.Success == true)
            {
                usuarioDirectoryDto = JsonConvert.DeserializeObject<UsuarioDto>(getUsuarioResponseDto.Data.ToString());

                // Verificar si ya está enrolado en el Tenant
                if (usuarioDirectoryDto.TenantsEnrolados.Any(m => m.UUID.Equals(tenant.TenantGuid)))
                    throw new Exception(_localizer["Core.Users.Errors.UserExists"]);

                // Agregarlo al Tenant y actualizar Directory
                usuarioDirectoryDto.TenantsEnrolados.Add(new EnrolamientoTenantDto { UUID = tenant.TenantGuid, Nombre = "", EnrolamientoConfirmado = false });
                var responsePutUsuarioDirectory = await _directoryApiServices.PutUsuarioAsync(usuarioDirectoryDto);
                var responseContentPutUsuarioDirectory = await responsePutUsuarioDirectory.Content.ReadAsStringAsync();
                var putUsuarioResponseDto = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContentPutUsuarioDirectory);

                if (putUsuarioResponseDto?.Success != true)
                    throw new Exception($"{_localizer["Core.Users.Errors.UpdateUserDirectory"]}: {putUsuarioResponseDto?.StatusCode}");
            }
            else
            {
                // Crear nuevo usuario en Directory
                usuarioDirectoryDto = new UsuarioDto
                {
                    Username = usuarioNubeticoDto.Username,
                    Password = suggestedPwd,
                    Nombres = usuarioNubeticoDto.Nombres,
                    PrimerApellido = usuarioNubeticoDto.PrimerApellido,
                    SegundoApellido = usuarioNubeticoDto.SegundoApellido,
                    Email = usuarioNubeticoDto.Email,
                    Habilitado = usuarioNubeticoDto.Bloqueado,
                    TenantsEnrolados = new List<EnrolamientoTenantDto>
                    {
                        new EnrolamientoTenantDto { UUID = tenant.TenantGuid, Nombre = "", EnrolamientoConfirmado = true }
                    }
                };

                var responsePostUsuarioDirectory = await _directoryApiServices.PostUsuarioAsync(usuarioDirectoryDto);
                var responseContentPostUsuarioDirectory = await responsePostUsuarioDirectory.Content.ReadAsStringAsync();
                var postUsuarioResponseDto = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContentPostUsuarioDirectory);

                if (postUsuarioResponseDto == null || !postUsuarioResponseDto.Success)
                    throw new Exception($"{_localizer["Core.Users.Errors.CreateUserDirectory"]}: {postUsuarioResponseDto?.StatusCode}");

                usuarioDirectoryDto = JsonConvert.DeserializeObject<UsuarioDto>(postUsuarioResponseDto.Data.ToString());
            }

            using var coreDbContext = _coreDbContextFactory.CreateDbContext();

            var persona = new Personas
            {
                Nombres = usuarioDirectoryDto.Nombres,
                PrimerApellido = usuarioDirectoryDto.PrimerApellido,
                SegundoApellido = usuarioDirectoryDto.SegundoApellido,
                Email = usuarioDirectoryDto.Email,
                IdUsuarioAlta = idUsuarioAlta,
                FechaAlta = DateTime.UtcNow,
                Usuarios = new List<Usuarios>
                {
                    new Usuarios
                    {
                        GuidUsuarioDirectory = usuarioDirectoryDto.UUID ?? throw new Exception(_localizer["Core.Users.Errors.UserGuidNotFound"]),
                        IdUsuarioEstado = 2,
                        Username = usuarioDirectoryDto.Username,
                        TwoFactorKey = null,
                        NavegarTabsActivo = usuarioNubeticoDto.Parametros.NavegarPorTabs,
                        IdMenuInicio = null,
                        DefaultCulture = usuarioNubeticoDto.Parametros.CulturaDefault,
                        DefaultTimeZone = usuarioNubeticoDto.Parametros.ZonaHorariaDefault,
                        Bloqueado = usuarioNubeticoDto.Bloqueado,
                        IdUsuarioAlta = idUsuarioAlta,
                        FechaAlta = DateTime.UtcNow,
                        Usuarios_Permisos = usuarioNubeticoDto.Permisos.Select(m => new Usuarios_Permisos { IdPermiso = m.IdPermiso, IdSucursal = (m.IdSucursal != -1 ? m.IdSucursal : null) }).ToList(),
                        Usuarios_Sucursales = usuarioNubeticoDto.Sucursales.Select(m=> new Usuarios_Sucursales { IdSucursal = (m.IdSucursal != -1 ? m.IdSucursal : null) } ).ToList()
                    }
                }
            };

            await coreDbContext.Personas.AddAsync(persona);

            if (await coreDbContext.SaveChangesAsync() <= 0)
                throw new Exception(_localizer["Core.Users.Errors.OnSave"]);

            usuarioNubeticoDto.UUID = usuarioDirectoryDto.UUID;

            // Registrar relación de menus seleccionados
            List<int> idsMenusSeleccionados = usuarioNubeticoDto.ListaMenus.Select(m => m.IdMenu).ToList();
            List<Menus> menusSelect = await coreDbContext.Menus
                .Where(m => idsMenusSeleccionados.Contains(m.IdMenu))
                .ToListAsync();

            // get menus padres
            var menusConPadres = new List<Menus>();
            var menusPadresIds = new HashSet<int>(menusSelect.Select(m => m.IdMenuPadre).Where(id => id.HasValue).Cast<int>());

            while (menusPadresIds.Any())
            {
                var padres = await coreDbContext.Menus
                    .Where(m => menusPadresIds.Contains(m.IdMenu))
                    .ToListAsync();

                menusSelect.AddRange(padres);
                menusPadresIds = new HashSet<int>(padres.Select(m => m.IdMenuPadre).Where(id => id.HasValue).Cast<int>());
            }

            // Eliminar menús duplicados
            menusSelect = menusSelect.DistinctBy(m => m.IdMenu).ToList();

            List<Usuarios_Menus> relacionUsuariosMenusInsert = menusSelect
                .Select(m => new Usuarios_Menus
                {
                    IdMenu = m.IdMenu,
                    IdUsuario = persona.Usuarios.FirstOrDefault().IdUsuario
                }).ToList();

            await coreDbContext.Usuarios_Menus.AddRangeAsync(relacionUsuariosMenusInsert);

            if (await coreDbContext.SaveChangesAsync() <= 0)
                throw new Exception(_localizer["Core.Users.Errors.OnSaveMenus"]);

            var tokenBd = new Usuarios_Tokens()
            {
                IdUsuario = persona.Usuarios.FirstOrDefault().IdUsuario,
                GuidToken = Guid.NewGuid(),
                Activacion = true,
                VigenteHasta = DateTime.UtcNow.AddDays(1),
                FechaAlta = DateTime.UtcNow
            };

            await coreDbContext.Usuarios_Tokens.AddAsync(tokenBd);

            if (await coreDbContext.SaveChangesAsync() <= 0)
                throw new Exception(_localizer["Core.Users.Errors.OnSaveToken"]);

            string urlActivacion = $"{httpScheme}://{tenant.TenantUrl}/activate-account/{tokenBd.GuidToken}?lang={currentCulture}";

            var resultEmail = usuarioNubeticoDto.Parametros.CulturaDefault == "en-US"
                ? await _CIEmailService.SendEmailAsync(new List<string> { $"{persona.Nombres} {persona.PrimerApellido} <{persona.Email}>" }, $"User Account Activation {tenant.Description}", FormatosCorreos.ActivacionCuentaTxtEn(urlActivacion), FormatosCorreos.ActivacionCuentaHtmlEn(urlActivacion))
                : await _CIEmailService.SendEmailAsync(new List<string> { $"{persona.Nombres} {persona.PrimerApellido} <{persona.Email}>" }, $"Activación de Cuenta de Usuario {tenant.Description}", FormatosCorreos.ActivacionCuentaTxtEs(urlActivacion), FormatosCorreos.ActivacionCuentaHtmlEs(urlActivacion));

            return usuarioNubeticoDto;
        }

        public async Task<UsuarioNubeticoDto?> SetUpdateUsuarioAsync(UsuarioNubeticoDto usuarioNubeticoDto)
        {
            usuarioNubeticoDto.Email = usuarioNubeticoDto.Email.Trim();

            using var coreDbContext = _coreDbContextFactory.CreateDbContext();

            Usuarios? usuario = await coreDbContext.Usuarios.FirstOrDefaultAsync(m => m.GuidUsuarioDirectory == usuarioNubeticoDto.UUID);

            if (usuario == null)
                return null;

            usuario.DefaultCulture = usuarioNubeticoDto.Parametros?.CulturaDefault ?? throw new ArgumentNullException(nameof(usuarioNubeticoDto.Parametros.CulturaDefault));
            usuario.DefaultTimeZone = usuarioNubeticoDto.Parametros?.ZonaHorariaDefault ?? throw new ArgumentNullException(nameof(usuarioNubeticoDto.Parametros.ZonaHorariaDefault));
            usuario.Bloqueado = usuarioNubeticoDto.Bloqueado;
            usuario.TwoFactorKey = usuarioNubeticoDto.TwoFactorKey;

            await coreDbContext.SaveChangesAsync();

            // Obtener menus actuales del usuario en base de datos
            List<int> idsMenusActuales = await coreDbContext.Usuarios_Menus
                .Where(m => m.IdUsuario == usuario.IdUsuario)
                .Select(m => m.IdMenu)
                .ToListAsync();
            List<Menus> menusActuales = await coreDbContext.Menus
                .Where(m => idsMenusActuales.Contains(m.IdMenu))
                .ToListAsync();

            // Obtener los ids de menus posteados y recuperar sus equivalentes en base de datos
            List<int> idsMenusPosteados = usuarioNubeticoDto.ListaMenus
                .Select(m => m.IdMenu)
                .ToList();
            List<Menus> menusPosteados = await coreDbContext.Menus
                .Where(m => idsMenusPosteados.Contains(m.IdMenu))
                .ToListAsync();

            // Función anidada para obtener menús padres de forma recursiva
            async Task ObtenerMenusPadresRecursivosAsync()
            {
                var nuevosMenusPadres = new List<Menus>();

                // Copiar los elementos actuales para iterar
                var copiaMenusPosteados = menusPosteados.ToList();

                foreach (var menu in copiaMenusPosteados)
                {
                    var menuPadreId = menu.IdMenuPadre;

                    // Continuar si tiene un menú padre y no está ya en los menús posteados
                    if (menuPadreId.HasValue && !idsMenusPosteados.Contains(menuPadreId.Value))
                    {
                        var menuPadre = await coreDbContext.Menus.FindAsync(menuPadreId.Value);
                        if (menuPadre != null)
                        {
                            // Acumular los nuevos menús padres en una lista separada
                            nuevosMenusPadres.Add(menuPadre);
                            idsMenusPosteados.Add(menuPadre.IdMenu);
                        }
                    }
                }

                // Agregar los nuevos menús padres a la lista principal
                menusPosteados.AddRange(nuevosMenusPadres);

                // Si se agregaron nuevos menús, continuar con la recursión
                if (nuevosMenusPadres.Any())
                {
                    await ObtenerMenusPadresRecursivosAsync();
                }
            }

            // Llamar la función anidada
            await ObtenerMenusPadresRecursivosAsync();

            // Identificar IDs que deben ser agregados
            var idsParaAgregar = idsMenusPosteados.Except(idsMenusActuales).ToList();
            var nuevosUsuariosMenus = idsParaAgregar.Select(id => new Usuarios_Menus
            {
                IdUsuario = usuario.IdUsuario,
                IdMenu = id
            }).ToList();

            // Agregar los nuevos IDs a Usuarios_Menus
            if (nuevosUsuariosMenus.Any())
            {
                await coreDbContext.Usuarios_Menus.AddRangeAsync(nuevosUsuariosMenus);
            }

            // Identificar IDs que deben ser eliminados
            var idsParaEliminar = idsMenusActuales.Except(idsMenusPosteados).ToList();
            var usuariosMenusParaEliminar = await coreDbContext.Usuarios_Menus
                .Where(um => um.IdUsuario == usuario.IdUsuario && idsParaEliminar.Contains(um.IdMenu))
                .ToListAsync();

            // Eliminar los IDs sobrantes de Usuarios_Menus
            if (usuariosMenusParaEliminar.Any())
            {
                coreDbContext.Usuarios_Menus.RemoveRange(usuariosMenusParaEliminar);
            }

            // Alta y baja de permisos
            // Convertir Dto posteado con permisos a modelo bd
            List<Usuarios_Permisos> permisosPosteados = usuarioNubeticoDto.Permisos
                .Select(m => new Usuarios_Permisos
                {
                    IdUsuario = usuario.IdUsuario,
                    IdPermiso = m.IdPermiso,
                    IdSucursal = m.IdSucursal == -1 ? null : m.IdSucursal
                }).ToList();

            // Obtener permisos actuales
            List<Usuarios_Permisos> permisosActuales = await coreDbContext.Usuarios_Permisos
                .Where(m => m.IdUsuario == usuario.IdUsuario)
                .ToListAsync();

            var permisosAgregar = permisosPosteados
                .Where(p => !permisosActuales.Any(pa => pa.IdPermiso == p.IdPermiso && pa.IdSucursal == p.IdSucursal))
                .ToList();
            coreDbContext.Usuarios_Permisos.AddRange(permisosAgregar);

            var permisosEliminar = permisosActuales
                .Where(pa => !permisosPosteados.Any(p => p.IdPermiso == pa.IdPermiso && p.IdSucursal == pa.IdSucursal))
                .ToList();
            coreDbContext.Usuarios_Permisos.RemoveRange(permisosEliminar);

            // Alta y baja de sucursales
            List<Usuarios_Sucursales> sucursalesPosteadas = usuarioNubeticoDto.Sucursales
                .Select(m => new Usuarios_Sucursales
                {
                    IdUsuario = usuario.IdUsuario,
                    IdSucursal = m.IdSucursal == -1 ? null : m.IdSucursal
                }).ToList();

            // List
            List<Usuarios_Sucursales> sucursalesActuales = await coreDbContext.Usuarios_Sucursales
                .Where(m => m.IdUsuario == usuario.IdUsuario)
                .ToListAsync();

            var sucursalesAgregar = sucursalesPosteadas
                .Where(p => !sucursalesActuales.Any(sa => sa.IdSucursal == p.IdSucursal))
                .ToList();
            coreDbContext.Usuarios_Sucursales.AddRange(sucursalesAgregar);

            var sucursalesEliminar = sucursalesActuales
                .Where(sa => !sucursalesPosteadas.Any(p => p.IdSucursal == sa.IdSucursal))
                .ToList();
            coreDbContext.Usuarios_Sucursales.RemoveRange(sucursalesEliminar);

            // Guardar los cambios en la base de datos
            await coreDbContext.SaveChangesAsync();

            return await this.GetUsuarioNubeticoByGuidAsync(usuario.GuidUsuarioDirectory);
        }

        public async Task<UserTwoFactorCodeDto?> SetUserTokenByValidation(UserTwoFactorCodeDto userTwoFactorCodeDto)
        {
            bool validToken = OtpHandler.TokenEsValido(userTwoFactorCodeDto.Code, userTwoFactorCodeDto.Key);

            if (!validToken)
                return null;

            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            Usuarios? usuario = await coreDbContext.Usuarios.Where(m => m.GuidUsuarioDirectory == userTwoFactorCodeDto.GuidUsuario).FirstOrDefaultAsync();

            if (usuario == null)
                return null;

            usuario.TwoFactorKey = userTwoFactorCodeDto.Key;

            if (await coreDbContext.SaveChangesAsync() <= 0)
                throw new DbUpdateException();

            return userTwoFactorCodeDto;
        }

        public async Task<UsuarioExisteDirectoryDto?> GetUsuarioDirectoryByUsernameAsync(string username)
        {
            // 1. Verificar localmente en bd del tenant
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            var usuarioInstancia = await coreDbContext.vUsuariosPersonas.Where(m => m.Username == username).FirstOrDefaultAsync();

            if (usuarioInstancia != null)
            {
                return new UsuarioExisteDirectoryDto
                {
                    Username = usuarioInstancia.Username,
                    Nombres = usuarioInstancia.Nombres,
                    PrimerApellido = usuarioInstancia.PrimerApellido,
                    SegundoApellido = usuarioInstancia.SegundoApellido,
                    GuidUsuario = usuarioInstancia.GuidUsuarioDirectory,
                    Confirmado = true,
                    EnTenant = true
                };
            }

            // 2. Verificar en Directory

            TenantModel? tenant = _tenantConnectionService.GetTenant();
            if (tenant == null)
                return null;

            if (tenant.TenantGuid == Guid.Empty)
                throw new Exception("Tenant not valid");

            var responseDirectory = await _directoryApiServices.GetUsuarioByUsernameAsync(username);

            if (!responseDirectory.IsSuccessStatusCode)
                return null;

            var responseDirectoryContent = await responseDirectory.Content.ReadAsStringAsync();
            var directoryResponseDto = JsonConvert.DeserializeObject<BaseResponseDto<UsuarioDto>>(responseDirectoryContent);
            var usuarioDirectory = directoryResponseDto.Data;

            var enrolamiento = usuarioDirectory.TenantsEnrolados.Where(m => m.UUID == tenant.TenantGuid).FirstOrDefault();

            return new UsuarioExisteDirectoryDto
            {
                Username = usuarioDirectory.Username,
                Nombres = usuarioDirectory.Nombres,
                PrimerApellido = usuarioDirectory.PrimerApellido,
                SegundoApellido = usuarioDirectory.SegundoApellido,
                GuidUsuario = usuarioDirectory.UUID,
                Email = usuarioDirectory.Email,
                EnTenant = enrolamiento != null,
                Confirmado = enrolamiento != null ? enrolamiento.EnrolamientoConfirmado ?? false : false
            };
        }

        public async Task<bool?> GetTokenIsValidAsync(Guid guidToken)
        {
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();

            var token = await coreDbContext.Usuarios_Tokens
                .Where(m => m.GuidToken == guidToken)
                .Select(m => new
                {
                    m.VigenteHasta,
                    m.FechaUsado,
                    Usuario = new
                    {
                        m.IdUsuarioNavigation.IdUsuario,
                        m.IdUsuarioNavigation.Bloqueado
                    }
                })
                .FirstOrDefaultAsync();

            if (token == null)
                return null;

            if (token.VigenteHasta <= DateTime.UtcNow || token.FechaUsado.HasValue)
                return false;

            if (token.Usuario == null || token.Usuario.Bloqueado)
                throw new Exception(_localizer["Core.Users.CannotLogin"].ToString().Replace("__STATUS__", _localizer["Core.Users.Blocked"].ToString().ToUpper()));

            return true;
        }

        public async Task<bool?> SetUpdatePasswordByTokenAsync(UpdatePswdByTokenDto updatePswdByTokenDto)
        {
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            var token = await coreDbContext.Usuarios_Tokens.Where(m => m.GuidToken == updatePswdByTokenDto.Token).FirstOrDefaultAsync();

            if (token == null)
                return null;

            if (token.VigenteHasta <= DateTime.UtcNow || token.FechaUsado.HasValue)
                return false;

            var usuario = await coreDbContext.Usuarios.Where(m => m.IdUsuario == token.IdUsuario).FirstOrDefaultAsync();

            if (usuario == null || usuario.Bloqueado)
                throw new Exception(_localizer["Core.Users.CannotLogin"].ToString().Replace("__STATUS__", _localizer["Core.Users.Blocked"].ToString().ToUpper()));

            var responseGetUsuarioDirectory = await _directoryApiServices.GetUsuarioByGuidAsync(usuario.GuidUsuarioDirectory);

            if (!responseGetUsuarioDirectory.IsSuccessStatusCode)
                throw new Exception("Core.Errors.UserDirectoryNotFound");

            var responseContentGetUsuarioDirectory = await responseGetUsuarioDirectory.Content.ReadAsStringAsync();
            var getUsuarioResponseDto = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContentGetUsuarioDirectory);

            UsuarioDto usuarioDirectoryDto = JsonConvert.DeserializeObject<UsuarioDto>(getUsuarioResponseDto.Data.ToString())
                ?? throw new Exception("Error deserializing user data from Directory");

            var tenant = GetTenant();

            var enrolamiento = usuarioDirectoryDto.TenantsEnrolados.Where(m => m.UUID == tenant.TenantGuid).FirstOrDefault();

            if (enrolamiento == null)
                throw new Exception("Core.Errors.UserNotFoundOnTenant");

            enrolamiento.EnrolamientoConfirmado = true;
            usuarioDirectoryDto.Password = updatePswdByTokenDto.Pswd;

            var responseUpdateUsuarioDirectory = await _directoryApiServices.PutUsuarioAsync(usuarioDirectoryDto);
            var responseContentPutUsuarioDirectory = await responseUpdateUsuarioDirectory.Content.ReadAsStringAsync();
            var putUsuarioResponseDto = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContentPutUsuarioDirectory);

            if (putUsuarioResponseDto?.Success != true)
                throw new Exception($"Core.Errors.UserDirectoryCode: {putUsuarioResponseDto?.StatusCode}");

            token.FechaUsado = DateTime.UtcNow;
            coreDbContext.Usuarios_Tokens.Update(token);

            usuario.IdUsuarioEstado = 1;
            coreDbContext.Usuarios.Update(usuario);

            return await coreDbContext.SaveChangesAsync() > 0;
        }

        private TenantModel GetTenant()
        {
            TenantModel? tenant = _tenantConnectionService.GetTenant();
            if (tenant == null)
                throw new Exception("Tenant not valid");

            if (tenant.TenantGuid == Guid.Empty)
                throw new Exception("Tenant not valid");

            return tenant;
        }
    }
}
