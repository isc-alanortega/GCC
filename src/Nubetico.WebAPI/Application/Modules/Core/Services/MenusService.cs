using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Core;
using System.Globalization;

namespace Nubetico.WebAPI.Application.Modules.Core.Services
{
    public class MenusService
    {
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;
        public MenusService(IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _coreDbContextFactory = coreDbContextFactory;
        }

        public async Task<List<MenuUsuarioDto>> GetUserMenusAsync(Guid guidUsuario)
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();

            var idUsuario = await coreDbContext.Usuarios
                .Where(u => u.GuidUsuarioDirectory == guidUsuario)
                .Select(u => u.IdUsuario)
                .FirstOrDefaultAsync();

            if (idUsuario == 0)
                return new List<MenuUsuarioDto>();

            var menus = await coreDbContext.vMenusFormularios
                .Where(m => m.Habilitado && coreDbContext.Usuarios_Menus.Any(um => um.IdUsuario == idUsuario && um.IdMenu == m.IdMenu))
                .Select(m => new vMenusFormularios
                {
                    IdMenu = m.IdMenu,
                    Nombre = currentCulture == "en-US" ? m.NombreEN : m.Nombre,
                    Descripcion = m.Descripcion,
                    Nivel = m.Nivel,
                    IdMenuPadre = m.IdMenuPadre,
                    RutaPagina = m.RutaPagina,
                    ComponentNamespace = m.ComponentNamespace,
                    ComponentTypeName = m.ComponentTypeName,
                    Expandido = m.Expandido,
                    Repetir = m.Repetir,
                    IconoText = m.IconoText,
                    IconoUnicode = m.IconoUnicode,
                    IconoCss = m.IconoCss,
                    IdModulo = m.IdModulo,
                    Habilitado = m.Habilitado,
                }).ToListAsync();

            List<MenuUsuarioDto> MapMenus(List<vMenusFormularios> menus, int? parentId = null)
            {
                return menus
                    .Where(m => m.IdMenuPadre == parentId || (parentId == null && m.IdMenuPadre == 0))
                    .OrderBy(m=>m.Nivel)
                    .Select(m => new MenuUsuarioDto
                    {
                        Name = m.Nombre,
                        Path = m.RutaPagina,
                        Title = m.Nombre,
                        Description = m.Descripcion,
                        Icon = m.IconoText,
                        UnicodeIcon = m.IconoUnicode,
                        IconClass = m.IconoCss,
                        ComponentNamespace = m.ComponentNamespace,
                        ComponentTypeName = m.ComponentTypeName,
                        Expanded = m.Expandido ?? false,
                        CanRepeatTab = m.Repetir,
                        Children = MapMenus(menus, m.IdMenu)
                    })
                    .ToList();
            }

            return MapMenus(menus);
        }

        public async Task<List<MenuDto>> GetAllMenusAsync()
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();

            var menus = await coreDbContext.Menus
               .Select(m => new MenuDto
               {
                   IdMenu = m.IdMenu,
                   Nombre = currentCulture == "en-US" ? m.NombreEN : m.Nombre,
                   Nivel = m.Nivel,
                   IdMenuPadre = m.IdMenuPadre,
                   Habilitado = m.Habilitado,
                   Seleccionable = m.RutaPagina != null
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
                        Seleccionable = m.Seleccionable,
                    })
                    .ToList();
            }

            return MapMenus(menus);
        }

        public async Task<List<MenuPermisosDto>?> GetAllMenusPermissionsAsync()
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();

            var menus = await coreDbContext.Menus
                .Include(m => m.IdFormularioNavigation)
                    .ThenInclude(f => f.Permisos)
                .Select(m => new
                {
                    m.IdMenu,
                    Nombre = currentCulture == "en-US" ? m.NombreEN : m.Nombre,
                    m.Nivel,
                    m.IdMenuPadre,
                    m.IdFormularioNavigation.Permisos
                })
                .ToListAsync();

            var result = menus.Select(m => new MenuPermisosDto
            {
                IdMenu = m.IdMenu,
                Nombre = m.Nombre,
                Nivel = m.Nivel,
                IdMenuPadre = m.IdMenuPadre,
                Permisos = m.Permisos.Select(p => new PermisoDto
                {
                    IdPermiso = p.IdPermiso,
                    Descripcion = currentCulture == "en-US" ? p.DescripcionEN : p.Descripcion
                }).ToList()
            }).ToList();

            List<MenuPermisosDto> MapMenus(List<MenuPermisosDto> menus, int? parentId = null)
            {
                return menus
                    .Where(m => m.IdMenuPadre == parentId || (parentId == null && m.IdMenuPadre == 0))
                    .OrderBy(m => m.Nivel)
                    .Select(m => new MenuPermisosDto
                    {
                        IdMenu = m.IdMenu,
                        Nombre = m.Nombre,
                        Nivel = m.Nivel,
                        IdMenuPadre = m.IdMenuPadre,
                        Children = MapMenus(menus, m.IdMenu),
                        Permisos = m.Permisos
                    })
                    .ToList();
            }

            return MapMenus(result);
        }

    }
}
