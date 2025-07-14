using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.PortalProveedores;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.PortalProveedores;
using Nubetico.Shared.Entities.PortalProveedores;

namespace Nubetico.DAL.Providers.PortalProveedores
{
    public class ObrasProvider
    {
        private readonly IDbContextFactory<PortalProveedoresContext> _dbContextFactory;

        public ObrasProvider(IDbContextFactory<PortalProveedoresContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<PaginatedListDto<ObraDto>> GetObrasPaginado(int start, int length, FiltroObras? filtro)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var query = context.vObras.AsQueryable();

                PaginatedListDto<ObraDto> result = new PaginatedListDto<ObraDto>
                {
                    RecordsTotal = await query.CountAsync(),
                    Data = await query
                        .Skip(start)
                        .Take(length)
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
