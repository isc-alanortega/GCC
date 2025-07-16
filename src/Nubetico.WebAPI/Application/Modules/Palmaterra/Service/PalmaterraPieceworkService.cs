using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Palmaterra.Piecework;
using Palmaterra.DAL.Models;

namespace Nubetico.WebAPI.Application.Modules.Palmaterra.Service
{
    public class PalmaterraPieceworkService
    {
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;
        public PalmaterraPieceworkService(IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _coreDbContextFactory = coreDbContextFactory;
        }

        public async Task Create(IEnumerable<PieceworkjElemntsDto> piecework)
        {
            // Instanciar el modelo de la bd de palmaterra
            using var palmaDbContext = new PalmaTerraDbContext(
                new DbContextOptionsBuilder<PalmaTerraDbContext>()
                .UseSqlServer(await PalmaterraDbUtil.GetConnectionStringAsync(_coreDbContextFactory))
                .Options);


        }
    }
}
