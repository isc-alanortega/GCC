using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.DAL.Providers.ProyectosConstruccion
{
    public static class InsumosProvider
    {
        public static async Task<List<TablaRelacionDto>> GetAllTipoInsumo(IDbContextFactory<ProyectosConstruccionDbContext> dbContextFactory)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();

            var result = await coreDbContext.Database
                .SqlQueryRaw<TablaRelacionDto>("SELECT ID, Valor, Descripcion FROM Core.vTipoTipoDeInsumo")
                .ToListAsync();

            return result;
        }
        
        public static async Task<List<TablaRelacionDto>> GetAllInsumos(IDbContextFactory<ProyectosConstruccionDbContext> dbContextFactory)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();

            var result = await coreDbContext.Database
                .SqlQueryRaw<TablaRelacionDto>("SELECT ID, Valor, Descripcion FROM Core.vTipoProveedor")
                .ToListAsync();

            return result;
        }
    }
}
