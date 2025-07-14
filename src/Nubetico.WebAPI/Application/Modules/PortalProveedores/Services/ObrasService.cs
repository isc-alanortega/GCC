using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.PortalProveedores;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.PortalProveedores;
using Nubetico.Shared.Entities.PortalProveedores;

namespace Nubetico.WebAPI.Application.Modules.PortalProveedores.Services
{
    public class ObrasService
    {
        private readonly IDbContextFactory<PortalProveedoresContext> _dbContextFactory;

        public ObrasService(IDbContextFactory<PortalProveedoresContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<PaginatedListDto<ObraDto>> GetObrasPaginado(int limit, int offset, FiltroObras? filtro)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var query = context.vObras.AsQueryable();

                PaginatedListDto<ObraDto> result = new PaginatedListDto<ObraDto>
                {
                    RecordsTotal = await query.CountAsync(),
                    Data = await query
                        .Skip(offset)
                        .Take(limit)
                        .Select(m => new ObraDto
                        {
                            IdObra = m.Id_Obra,
                            Nombre = m.Nombre,
                            FechaInicio = m.Fecha_Inicio,
                            FechaFin = m.Fecha_Fin,
                            IdPersonaResidente = m.Id_Persona_Residente,
                            Residente = m.Residente,
                            IdEstadoObra = m.Id_Estado_Obra,
                            EstadoObra = m.Estado_Obra,
                            IdTipoObra = m.Id_Tipo_Obra,
                            TipoObra = m.Tipo_Obra,
                            EsProyectoObra = m.Es_Proyecto_Obra,
                            Completada = m.Completada,
                            Observaciones = m.Observaciones
                        }).ToListAsync()
                };

                result.RecordsFiltered = result.Data.Count;

                return result;
            }
        }
    }
}
