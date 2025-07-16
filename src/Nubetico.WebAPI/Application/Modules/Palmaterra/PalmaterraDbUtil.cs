using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;

namespace Nubetico.WebAPI.Application.Modules.Palmaterra
{
    public class PalmaterraDbUtil
    {
        public static async Task<string> GetConnectionStringAsync(IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            using var coreDbContext = coreDbContextFactory.CreateDbContext();

            var result = await coreDbContext.Parametros
                .FirstOrDefaultAsync(p => p.Alias == "external.palmaterradb");

            return result != null
                ? result.Valor1
                : throw new Exception("ConnectionString for 'PalmaTerraDbContext' not found at 'PalmaterraDbUtil'");
        }
    }
}
