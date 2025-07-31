using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.DAL.Providers.Core;
using Nubetico.DAL.ResultSets.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services
{
    public class EgresosService
    {
        //Aquí defino las funciones que accesan a los datos
        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _proyectosConstruccionDbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

        public EgresosService(IDbContextFactory<ProyectosConstruccionDbContext> proyectosConstruccionDbContextFactory, IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _proyectosConstruccionDbContextFactory = proyectosConstruccionDbContextFactory;
            _coreDbContextFactory = coreDbContextFactory;
        }

        public async Task<List<EgresoDto>?> GetEgresosAsync()
        {
            using var proyectosConstruccionDbContext = _proyectosConstruccionDbContextFactory.CreateDbContext();
            var result = await proyectosConstruccionDbContext.Egresos.Select(le => new EgresoDto { Id_Egreso = le.Id_Egreso }).ToListAsync();

            return result;
        }

        public async Task<List<vEgresos_Partidas_Detalles>?> GetvEgresos_Partidas_DetallesAsync()
        {
            using var proyectosConstruccionDbContext = _proyectosConstruccionDbContextFactory.CreateDbContext();
            //var result = await proyectosConstruccionDbContext.vEgresos_Partidas_Detalles.Select(le => new vEgresos_Partidas_Detalles { Id_Egreso = le.Id_Egreso }).ToListAsync();
            var result = await proyectosConstruccionDbContext.vEgresos_Partidas_Detalles.Select(le => le).ToListAsync();

            return result;
        }
    }
}
