using System.Net.Http;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.DAL.Providers.Core;
using Nubetico.DAL.ResultSets.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;

//using Nubetico.DAL.Models.Core;
//using Nubetico.DAL.Models.ProyectosConstruccion;

//using Nubetico.Shared.Dto.Common;
//using Nubetico.Shared.Dto.ProyectosConstruccion;
//using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
//using Nubetico.Shared.Enums.ProyectosConstruccion;


namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services
{
	public class ProveedoresService	{

        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _dbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

        public ProveedoresService(IDbContextFactory<ProyectosConstruccionDbContext> dbContextFactory, IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _coreDbContextFactory = coreDbContextFactory;

        }

        public async Task<ProveedorResultSet?> CreateProveedorAsync(ProveedorRequestDto proveedorRequest)
        {
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            var result = await ProveedoresProvider.CreateProveedorAsync(_coreDbContextFactory, proveedorRequest);

            return result;
        }
        public async Task<List<ProveedorGridResultSet>> GetAllProveedoresAsync()
        {
            var result = await ProveedoresProvider.GetAllProveedoresAsync(_coreDbContextFactory);
            return result;
        }

        public async Task<ProveedorGetDto?> GetProveedorByIdAsync(int idProveedor)
        {
            using var coreDbContext = await _coreDbContextFactory.CreateDbContextAsync();
            var result = await ProveedoresProvider.GetProveedorByIdAsync(_coreDbContextFactory, idProveedor);
            return result?.FirstOrDefault(); // Devuelve un solo registro o null si no hay resultados
        }

    }

}
