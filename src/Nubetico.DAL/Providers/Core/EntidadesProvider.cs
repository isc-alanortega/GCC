using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;

namespace Nubetico.DAL.Providers.Core
{
    public static class EntidadesProvider
    {
        #region REGIMEN_FISCAL
        public static async Task<List<TablaRelacionDto>> GetAllTipoRegimenFiscal(IDbContextFactory<CoreDbContext> dbContextFactory)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();

            var result = await coreDbContext.Database
                .SqlQueryRaw<TablaRelacionDto>("SELECT ID, Valor, Descripcion FROM Core.vTipoRegimenFiscal")
                .ToListAsync();

            return result;
        }
        public static async Task<List<TablaRelacionDto>> GetAllRegimenFiscal(IDbContextFactory<CoreDbContext> dbContextFactory)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();

            var result = await coreDbContext.Database
                .SqlQueryRaw<TablaRelacionDto>("SELECT ID, Valor, Descripcion FROM Core.vTipoRegimen")
                .ToListAsync();

            return result;
        }
        public static async Task<List<TablaRelacionDto>> GetAllFormaPago(IDbContextFactory<CoreDbContext> dbContextFactory)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();

            var result = await coreDbContext.Database
                .SqlQueryRaw<TablaRelacionDto>("SELECT ID, Valor, Descripcion FROM Core.vTipoFormaPago")
                .ToListAsync();

            return result;
        }
        public static async Task<List<TablaRelacionStringDto>> GetAllMetodoDePago(IDbContextFactory<CoreDbContext> dbContextFactory)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();

            var result = await coreDbContext.Database
                .SqlQueryRaw<TablaRelacionStringDto>("SELECT ID, ValorString, Descripcion FROM Core.vTipoMetodoPago")
                .ToListAsync();

            return result;
        }
        public static async Task<List<TablaRelacionStringDto>> GetAllUsoCFDI(IDbContextFactory<CoreDbContext> dbContextFactory)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();

            var result = await coreDbContext.Database
                .SqlQueryRaw<TablaRelacionStringDto>("SELECT ID, ValorString, Descripcion FROM Core.vTipoUsoCfdi")
                .ToListAsync();

            return result;
        }
        #endregion


    }
}
