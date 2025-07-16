using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;

namespace Nubetico.WebAPI.Application.Modules.PortalClientes
{
    public class CwEMDbUtil
    {
        public static async Task<string> GetConnectionStringAsync(IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            using var coreDbContext = coreDbContextFactory.CreateDbContext();

            var result = await coreDbContext.Parametros
                .FirstOrDefaultAsync(p => p.Alias == "external.cwemdb");

            return result != null
                ? result.Valor1
                : throw new Exception("ConnectionString for 'CW_EduardoMagallonDbContext' not found at 'CwEMDbUtil'");
        }
    }
}
