using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.ReportesDestajos;
using Nubetico.WebAPI.Application.Utils;
using System.Globalization;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services
{
    public class ReportesDestajosService
    {
        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _pcDbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

        public ReportesDestajosService(IDbContextFactory<ProyectosConstruccionDbContext> pcDbContextFactory, IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _pcDbContextFactory = pcDbContextFactory;
            _coreDbContextFactory = coreDbContextFactory;
        }

        public async Task<List<ReporteDestajoGridDto>> GetGridReportesDestajosDtoAsync(int idSeccion, int? idStatus = null)
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;

            using var pcDbContext = _pcDbContextFactory.CreateDbContext();
            var query = from reporte in pcDbContext.ReportesDestajos
                        join estatus in pcDbContext.ReportesDestajos_Estatus on reporte.IdReporteDestajoEstatus equals estatus.IdReporteDestajoEstatus
                        join seccion in pcDbContext.Secciones on reporte.IdSeccion equals seccion.Id_Seccion
                        where reporte.IdSeccion == idSeccion
                        select new ReporteDestajoGridDto
                        {
                            IdReporteDestajo = reporte.IdReporteDestajo,
                            IdSeccion = reporte.IdSeccion,
                            Seccion = seccion.Nombre,
                            Fecha = reporte.FechaReporte,
                            IdStatus = reporte.IdReporteDestajoEstatus,
                            Status = currentCulture == "en-US" ? estatus.EstatusEN : estatus.Estatus,
                            Glyph = estatus.GlyphIcon,
                            DarkColorHex = estatus.DarkColorHex,
                            LightColorHex = estatus.LightColorHex
                        };

            if (idStatus.HasValue)
            {
                query = query.Where(reporte => reporte.IdStatus == idStatus.Value);
            }

            List<ReporteDestajoGridDto> result = await query.ToListAsync();
            return result;
        }

        public async Task<ReporteDestajoDto?> GetReporteDestajoDtoAsync(int idReporteDestajoDto)
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;

            using (var pcDbContext = _pcDbContextFactory.CreateDbContext())
            {
                var result = await (from reporte in pcDbContext.ReportesDestajos
                                    join estatus in pcDbContext.ReportesDestajos_Estatus on reporte.IdReporteDestajoEstatus equals estatus.IdReporteDestajoEstatus
                                    join seccion in pcDbContext.Secciones on reporte.IdSeccion equals seccion.Id_Seccion
                                    join proyecto in pcDbContext.Proyectos on seccion.Id_Proyecto equals proyecto.Id_Proyecto
                                    where reporte.IdReporteDestajo == idReporteDestajoDto
                                    select new ReporteDestajoDto
                                    {
                                        IdReporteDestajo = reporte.IdReporteDestajo,
                                        FechaReporte = reporte.FechaReporte,
                                        IdProyecto = proyecto.Id_Proyecto,
                                        Proyecto = proyecto.Descripcion,
                                        IdSeccion = reporte.IdSeccion,
                                        Seccion = seccion.Descripcion,
                                        IdStatus = reporte.IdReporteDestajoEstatus,
                                        Status = currentCulture == "en-US" ? estatus.EstatusEN : estatus.Estatus,
                                        Glyph = estatus.GlyphIcon,
                                        DarkColorHex = estatus.DarkColorHex,
                                        LightColorHex = estatus.LightColorHex
                                    }).FirstOrDefaultAsync();

                if (result == null)
                    return null;

                result.Partidas = await (from partida in pcDbContext.ReportesDestajos_Partidas
                                         join modeloPu in pcDbContext.Modelos_Precios_Unitarios on partida.IdModeloPrecioUnitario equals modeloPu.ID_Modelo_Precio_Unitario
                                         where partida.IdReporteDestajo == result.IdReporteDestajo
                                         select new ReporteDestajoPartidaDto
                                         {
                                             IdReporteDestajoPartida = partida.IdReporteDestajoPartida,
                                             IdModeloPU = partida.IdModeloPrecioUnitario,
                                             ModeloPU = modeloPu.Descripcion,
                                             Lotes = partida.NotasSupervisor,
                                             PorcentajeAvance = partida.PorcentajeReportado,
                                             Notas = partida.NotasContratista
                                         }).ToListAsync();

                if (result.Partidas.Count > 0)
                {
                    foreach (var partida in result.Partidas)
                    {
                        partida.Imagenes = await pcDbContext.vReportesDestajos_Partidas_Fotos
                            .Where(f => f.IdReporteDestajoPartida.Equals(partida.IdReporteDestajoPartida))
                            .Select(v => new ImagenDestajoDto
                            {
                                Id = SqidsUtil.Encode(v.IdReporteDestajoPartidaFoto),
                                TokenUpload = v.TokenUpload.ToString() ?? string.Empty,
                                FechaCaptura = v.FechaCaptura,
                                FechaSincronizacion = v.FechaSincronizacion,
                                Url = $"api/v1/proyectosconstruccion/reportesdestajos/photo/{SqidsUtil.Encode(v.IdReporteDestajoPartidaFoto)}"
                            }).ToListAsync();
                    }
                }

                return result;
            }
        }

        public async Task<List<BasicItemSelectDto>> GetReportesDestajosEstatusAsync()
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;

            using (var pcDbContext = _pcDbContextFactory.CreateDbContext())
            {
                var result = await pcDbContext.ReportesDestajos_Estatus
                    .Select(m => new BasicItemSelectDto
                    {
                        Value = m.IdReporteDestajoEstatus,
                        Text = currentCulture == "en-US" ? m.EstatusEN : m.Estatus,
                    }).ToListAsync();

                return result;
            }
        }

        public async Task<IEnumerable<BasicItemSelectDto>> GetProjectSelectListAsync()
        {
            using (var pcDbContext = _pcDbContextFactory.CreateDbContext())
            {
                List<BasicItemSelectDto> result = await pcDbContext.Proyectos
                    .Select(m => new BasicItemSelectDto
                    {
                        Value = m.Id_Proyecto,
                        Text = m.Nombre,
                    }).ToListAsync();

                return result;
            }
        }

        public async Task<IEnumerable<BasicItemSelectDto>> GetSectionSelectListAsync(int projectId, int? contractorId)
        {
            using (var pcDbContext = _pcDbContextFactory.CreateDbContext())
            {
                var query = pcDbContext.Secciones
                    .Where(m => m.Id_Proyecto == projectId);

                if (contractorId.HasValue)
                {
                    query = query.Where(m => m.Id_Contratista == contractorId.Value);
                }

                List<BasicItemSelectDto> result = await query
                    .Select(m => new BasicItemSelectDto
                    {
                        Value = m.Id_Seccion,
                        Text = m.Nombre,
                    }).ToListAsync();

                return result;
            }
        }

        // contenido, tipo y nombre
        public async Task<Tuple<byte[], string, string>?> GetPhotoByIdAsync(string photoId)
        {
            int? idDetalleFoto = SqidsUtil.Decode(photoId);

            if (idDetalleFoto == null)
                return null;

            using var pcDbContext = _pcDbContextFactory.CreateDbContext();

            vReportesDestajos_Partidas_Fotos? detalleFoto = await pcDbContext.vReportesDestajos_Partidas_Fotos
                .FirstOrDefaultAsync(m => m.IdReporteDestajoPartidaFoto == idDetalleFoto.Value);

            if (detalleFoto == null)
                return null;

            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            Parametros parametroRutaTrabajo = await coreDbContext.Parametros.FirstOrDefaultAsync(m => m.Alias == "archivos.rutaTrabajo")
                ?? throw new Exception("Parametro de ruta de trabajo no encontrado.");

            var rutaArchivo = Path.Combine(parametroRutaTrabajo.Valor1, detalleFoto.RutaRelativa);

            if (!File.Exists(rutaArchivo))
                throw new Exception("El archivo no se encuentra donde debería estar");

            return new Tuple<byte[], string, string>(File.ReadAllBytes(rutaArchivo), detalleFoto.MimeType, detalleFoto.NombreOriginal);
        }

        public async Task<ReporteDestajoDto> SetInsertReporteDestajoAsync(ReporteDestajoDto reporteDtoPost)
        {
            using var pcDbContext = await _pcDbContextFactory.CreateDbContextAsync();

            Secciones seccionSeleccionada = await pcDbContext.Secciones.FirstOrDefaultAsync(m => m.Id_Seccion == reporteDtoPost.IdSeccion)
                ?? throw new Exception("Sección seleccionada no encontrada");

            ReportesDestajos? nuevoReporte = new ReportesDestajos
            {
                IdReporteDestajoEstatus = reporteDtoPost.IdStatus,
                IdProyecto = seccionSeleccionada.Id_Proyecto,
                IdSeccion = seccionSeleccionada.Id_Seccion,
                IdContratista = 0,
                FechaReporte = reporteDtoPost.FechaReporte,
                NotasReporte = string.Empty,
                IdUsuarioAlta = 1,
                FechaAlta = DateTime.UtcNow,
                ReportesDestajos_Partidas = reporteDtoPost.Partidas
                    .Select(partidaDto => new ReportesDestajos_Partidas
                    {
                        IdModeloPrecioUnitario = partidaDto.IdModeloPU,
                        PorcentajeReportado = partidaDto.PorcentajeAvance,
                        NotasContratista = partidaDto.Notas,
                        NotasSupervisor = partidaDto.Lotes,
                        ReportesDestajos_Partidas_Fotos = partidaDto.Imagenes.Select(i => new ReportesDestajos_Partidas_Fotos
                        {
                            TokenUpload = Guid.Parse(i.TokenUpload),
                            FechaCaptura = i.FechaCaptura
                        }).ToList(),
                        FechaAlta = DateTime.UtcNow
                    }).ToList()
            } ?? throw new Exception("Error al mapear clase objeto posteado a bd");

            await pcDbContext.ReportesDestajos.AddAsync(nuevoReporte);
            if (await pcDbContext.SaveChangesAsync() <= 0)
                throw new Exception("Error al guardar reporte en base de datos");

            reporteDtoPost.IdReporteDestajo = nuevoReporte.IdReporteDestajo;

            return reporteDtoPost;
        }

        public async Task<bool?> SetPhotoPartidaDestajo(Guid tokenUpload, byte[] photoData, string nombreArchivo)
        {
            using var pcDbContext = await _pcDbContextFactory.CreateDbContextAsync();

            ReportesDestajos_Partidas_Fotos? detalleFoto = await pcDbContext.ReportesDestajos_Partidas_Fotos
                .FirstOrDefaultAsync(m => m.TokenUpload == tokenUpload && m.IdArchivo == null);

            if (detalleFoto == null)
                return null;

            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            Parametros parametroRutaTrabajo = await coreDbContext.Parametros.FirstOrDefaultAsync(m => m.Alias == "archivos.rutaTrabajo")
                ?? throw new Exception("Parametro de ruta de trabajo no encontrado.");
            string rutaGuardado = Path.Combine("destajos", detalleFoto.IdReporteDestajoPartida.ToString());

            Archivos_Tipos tipoArchivo = await coreDbContext.Archivos_Tipos.FirstOrDefaultAsync(m => m.Extension == Path.GetExtension(nombreArchivo))
                ?? throw new Exception($"Extensión '{Path.GetExtension(nombreArchivo)}' no permitida.");

            Archivos nuevoArchivo = new Archivos
            {
                NombreOriginal = nombreArchivo,
                RutaRelativa = Path.Combine(rutaGuardado, nombreArchivo),
                IdArchivoTipo = tipoArchivo.IdArchivoTipo,
                FechaAlta = DateTime.UtcNow
            };

            await coreDbContext.Archivos.AddAsync(nuevoArchivo);

            if (await coreDbContext.SaveChangesAsync() <= 0)
                throw new Exception("Se produjo un error al guardar la relación del archivo.");

            var rutaDirectorio = Path.Combine(parametroRutaTrabajo.Valor1, rutaGuardado);
            if (!Directory.Exists(rutaDirectorio))
            {
                Directory.CreateDirectory(rutaDirectorio);
            }

            await File.WriteAllBytesAsync(Path.Combine(rutaDirectorio, nuevoArchivo.NombreOriginal), photoData);

            detalleFoto.IdArchivo = nuevoArchivo.IdArchivo;

            return await pcDbContext.ReportesDestajos_Partidas_Fotos
                .Where(f => f.IdReporteDestajoPartidaFoto == detalleFoto.IdReporteDestajoPartidaFoto)
                .ExecuteUpdateAsync(m => m
                    .SetProperty(f => f.IdArchivo, nuevoArchivo.IdArchivo)
                    .SetProperty(f => f.FechaSincronizacion, DateTime.UtcNow)) > 0;
        }
    }
}
