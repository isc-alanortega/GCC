using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;

namespace Nubetico.WebAPI.Application.Modules.Core.Services
{
    public class PermisosService
    {
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;
        public PermisosService(IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _coreDbContextFactory = coreDbContextFactory;
        }

        /// <summary>
        /// Leer si un usuario tiene acceso a un endpoint
        /// </summary>
        /// <param name="guidUsuario"></param>
        /// <param name="idSucursal"></param>
        /// <param name="endpoint"></param>
        /// <param name="webMethod"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool?> UserHasApiPermissionAsync(Guid guidUsuario, int? idSucursal, string endpoint, string webMethod)
        {
            using (var coreDbContext = _coreDbContextFactory.CreateDbContext())
            {
                Usuarios usuario = await coreDbContext.Usuarios.FirstOrDefaultAsync(u => u.GuidUsuarioDirectory == guidUsuario)
                    ?? throw new Exception("No se encontró el ID del usuario en sesión.");

                Permisos? permiso = await coreDbContext.Permisos.FirstOrDefaultAsync(p => p.Endpoint == endpoint && p.WebMethod == webMethod);

                if (permiso == null)
                    return null;

                vUsuariosPermisos? permisoVigente = await coreDbContext.vUsuariosPermisos.FirstOrDefaultAsync(v => v.IdPermiso == permiso.IdPermiso && usuario.IdUsuario == usuario.IdUsuario && v.IdSucursal == idSucursal);
                return permisoVigente != null;
            }
        }

        //public async Task<bool?> UserHasCommponentPermissionAsync()
        //{

        //}
    }
}
