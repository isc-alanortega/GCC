using Nubetico.WebAPI.Application.Modules.Core.Models;

namespace Nubetico.WebAPI.Application.Utils
{
    public class TenantConnectionService
    {
        private TenantModel? _tenant { get; set; }

        public void SetTenant(TenantModel? tenant)
        {
            _tenant = tenant;
        }

        public TenantModel? GetTenant()
        {
            return _tenant;
        }

        public void ClearTenant()
        {
            _tenant = null;
        }
    }

}
