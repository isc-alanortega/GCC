using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.DAL.Providers.Core;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;

namespace Nubetico.WebAPI.Application.Modules.Core.Services
{
    public class EntidadesService
    {
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

        public EntidadesService(IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _coreDbContextFactory = coreDbContextFactory;

        }
        public async Task<List<TablaRelacionDto>> GetAllTipoRegimenFiscal()
        {
            var result = await EntidadesProvider.GetAllTipoRegimenFiscal(_coreDbContextFactory);
            return result;
        } 
        public async Task<List<TablaRelacionDto>> GetAllRegimenFiscal()
        {
            var result = await EntidadesProvider.GetAllRegimenFiscal(_coreDbContextFactory);
            return result;
        } 
        public async Task<List<TablaRelacionDto>> GetAllFormaPago()
        {
            var result = await EntidadesProvider.GetAllFormaPago(_coreDbContextFactory);
            return result;
        }
        public async Task<List<TablaRelacionStringDto>> GetAllMetodoDePago()
        {
            var result = await EntidadesProvider.GetAllMetodoDePago(_coreDbContextFactory);
            return result;
        }
        public async Task<List<TablaRelacionStringDto>> GetAllUsoCFDI()
        {
            var result = await EntidadesProvider.GetAllUsoCFDI(_coreDbContextFactory);
            return result;
        }



    }

    }
