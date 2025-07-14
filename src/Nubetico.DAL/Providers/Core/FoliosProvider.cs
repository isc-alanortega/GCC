using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.ResultSets.Core;
using System.Data;

namespace Nubetico.DAL.Providers.Core
{
    public static class FoliosProvider
    {
        /// <summary>
        /// Provider estático para la generación de folios.
        /// </summary>
        /// <param name="dbContextFactory">Factory de CoreDbContext</param>
        /// <param name="alias">Debe estar definido en la tabla Core.Folios</param>
        /// <param name="idSucursal">Valor opcional para folios agrupados por sucursal</param>
        /// <returns></returns>
        public static async Task<FolioResultSet?> GetFolioAsync(IDbContextFactory<CoreDbContext> dbContextFactory, string alias, int? idSucursal)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();

            var parameters = new[]
            {
                new SqlParameter("@Alias", SqlDbType.NVarChar) { Value = alias },
                new SqlParameter("@IdSucursal", SqlDbType.Int) { Value = idSucursal ?? (object)DBNull.Value}
            };

            var result = await coreDbContext.Database
                .SqlQueryRaw<FolioResultSet>("EXEC Core.SP_Folio_Get @Alias, @IdSucursal", parameters)
                .ToListAsync();

            return result.FirstOrDefault();
        }
    }
}
