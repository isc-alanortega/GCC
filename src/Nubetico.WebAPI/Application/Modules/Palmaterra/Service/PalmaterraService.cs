using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Palmaterra;
//using Nubetico.Shared.Dto.ProyectosConstruccion;
using Palmaterra.DAL.Models;

namespace Nubetico.WebAPI.Application.Modules.Palmaterra.Service
{
	public class PalmaterraService
	{
		//Aquí defino las funciones que accesan a los datos
		//private readonly IDbContextFactory<PalmaTerraDbContext> _palmaterraDbContextFactory;
		private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

		public PalmaterraService(IDbContextFactory<CoreDbContext> coreDbContextFactory)
		{
			//_palmaterraDbContextFactory = palmaterraDbContextFactory;
			_coreDbContextFactory = coreDbContextFactory;
		}

		public async Task<List<Obras>?> GetObrasAsync()
		{
			//using var palmaterraDbContext = _palmaterraDbContextFactory.CreateDbContext();

			using var palmaterraDbContext = new PalmaTerraDbContext(
				new DbContextOptionsBuilder<PalmaTerraDbContext>()
				.UseSqlServer(await PalmaterraDbUtil.GetConnectionStringAsync(_coreDbContextFactory))
				.Options);

			var result = await palmaterraDbContext.Obras.Select(ob => ob).ToListAsync();

			return result;
		}
	}
}
