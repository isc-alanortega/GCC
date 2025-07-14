using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.ProyectosConstruccion;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services
{
    public class CatalogsSupplysService(IDbContextFactory<ProyectosConstruccionDbContext> dbContextFactory)
    {
        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _dbContextFactory = dbContextFactory;

        #region UNIT SUPPLY
        public async Task<int?> GetTypeSupplyIdByNameAsync(string unitName)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
               return await db.Catalogo_Insumos_Unidades
                        .Where(item => item.Name.ToLower().Contains(unitName.ToLower()))
                        .Select(item => (int?)item.ID)
                        .FirstOrDefaultAsync();
            }
        }

        public async Task<int?> AddUnitSupplyAsync(string unitName)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
                var unitDb = new Catalogo_Insumos_Unidades { Name = unitName };
                await db.AddAsync(unitDb);
                await db.SaveChangesAsync();

                return unitDb.ID;
            }
        }
        #endregion

    }
}
