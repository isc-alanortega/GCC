using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.WebAPI.Application.Modules.Core.Services
{
    public class SucursalesService
    {
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;
        public SucursalesService(IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _coreDbContextFactory = coreDbContextFactory;
        }

        public async Task<IEnumerable<SucursalDto>?> GetSucursalesAsync()
        {
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();

            return await coreDbContext.Sucursales
                .Where(m => m.FechaElimina == null)
                .Select(m => new SucursalDto
                { 
                    IdSucursal = m.IdSucursal,
                    Descripcion = m.Denominacion,
                    CveSucursal = m.CveSucursal
                }).ToListAsync();
        }
    }
}
