using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.ResultSets.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;
using System.Data;

namespace Nubetico.DAL.Providers.Core
{
    public static class ProveedoresProvider
    {
        #region POST

        /// <summary>
        /// Crea un nuevo proveedor ejecutando el procedimiento almacenado SP_Proovedores_Post.
        /// </summary>
        /// <param name="dbContextFactory">Factory de CoreDbContext</param>
        /// <param name="proveedorRequest">Datos del proveedor a crear</param>
        /// <returns>Resultado del procedimiento (éxito, mensaje e ID del proveedor)</returns>
        public static async Task<ProveedorResultSet?> CreateProveedorAsync(IDbContextFactory<CoreDbContext> dbContextFactory, ProveedorRequestDto proveedorRequest)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();
            var parameters = new[]
            {
        //new SqlParameter("@UUID", SqlDbType.UniqueIdentifier) {Value = (model.UUID == Guid.Empty)? DBNull.Value: (object)model.UUID};
        new SqlParameter("@UUID", SqlDbType.UniqueIdentifier) { Value = proveedorRequest.UUID },
        new SqlParameter("@Folio", SqlDbType.NVarChar, 10) { Value = (object)proveedorRequest.Folio ?? DBNull.Value },
        new SqlParameter("@Rfc", SqlDbType.NVarChar, 13) { Value = proveedorRequest.Rfc ?? (object)DBNull.Value },
        new SqlParameter("@RazonSocial", SqlDbType.NVarChar, 500) { Value = proveedorRequest.RazonSocial   },
        new SqlParameter("@NombreComercial", SqlDbType.NVarChar, 500) { Value = proveedorRequest.NombreComercial  },
        new SqlParameter("@IdTipoPersonaSAT", SqlDbType.Int) { Value = proveedorRequest.IdTipoPersonaSAT },
        new SqlParameter("@Email", SqlDbType.NVarChar, 250) { Value = (object)proveedorRequest.Email ?? DBNull.Value },
        new SqlParameter("@Web", SqlDbType.NVarChar, 250) { Value = (object)proveedorRequest.Web ?? DBNull.Value },
        new SqlParameter("@Credito", SqlDbType.Bit) { Value = proveedorRequest.Credito },
        new SqlParameter("@LimiteCreditoMXN", SqlDbType.Money) { Value = proveedorRequest.LimiteCreditoMXN },
        new SqlParameter("@LimiteCreditoUSD", SqlDbType.Money) { Value = proveedorRequest.LimiteCreditoUSD },
        new SqlParameter("@DiasCredito", SqlDbType.Int) { Value = proveedorRequest.DiasCredito },
        new SqlParameter("@DiasGracia", SqlDbType.Int) { Value = (object)proveedorRequest.DiasGracia ?? DBNull.Value },
        new SqlParameter("@SaldoMXN", SqlDbType.Money) { Value = proveedorRequest.SaldoMXN },
        new SqlParameter("@SaldoUSD", SqlDbType.Money) { Value = proveedorRequest.SaldoUSD },
        new SqlParameter("@CuentaContable", SqlDbType.NVarChar, 50) { Value = (object)proveedorRequest.CuentaContable ?? DBNull.Value },
        new SqlParameter("@IdFormaPago", SqlDbType.Int) { Value = (object)proveedorRequest.IdFormaPago ?? DBNull.Value },
        new SqlParameter("@IdTipoMetodoPago", SqlDbType.Int) { Value = proveedorRequest.IdTipoMetodoPago },
        new SqlParameter("@IdUsoCFDI", SqlDbType.Int) { Value = proveedorRequest.IdUsoCFDI },
        new SqlParameter("@IdTipoRegimenFiscal", SqlDbType.Int) { Value = proveedorRequest.IdTipoRegimenFiscal },
        new SqlParameter("@IdTipoInsumo", SqlDbType.Int) { Value = proveedorRequest.IdTipoInsumo },
        new SqlParameter("@IdUsuarioAlta", SqlDbType.Int) { Value = proveedorRequest.IdUsuarioAlta },
        new SqlParameter("@IdEstadoProveedor", SqlDbType.Int) { Value = proveedorRequest.IdEstadoProveedor },
        new SqlParameter("@IdRegimenFiscal", SqlDbType.Int) { Value = proveedorRequest.IdRegimenFiscal }
        };

            var result = await coreDbContext.Database
                .SqlQueryRaw<ProveedorResultSet>(
                    "EXEC [Core].[SP_Proovedores_Post] @UUID, @Folio, @Rfc, @RazonSocial, @NombreComercial, " +
                    "@IdTipoPersonaSAT, @Email, @Web, @Credito, @LimiteCreditoMXN, @LimiteCreditoUSD, " +
                    "@DiasCredito, @DiasGracia, @SaldoMXN, @SaldoUSD, @CuentaContable, @IdFormaPago, " +
                    "@IdTipoMetodoPago, @IdUsoCFDI, @IdTipoRegimenFiscal, @IdTipoInsumo, @IdUsuarioAlta, " +
                    "@IdEstadoProveedor, @IdRegimenFiscal",
                    parameters
                )
                .ToListAsync();

            return result.FirstOrDefault();
        }



        #endregion


        #region GET



        public static async Task<List<ProveedorGridResultSet>> GetAllProveedoresAsync(IDbContextFactory<CoreDbContext> dbContextFactory)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();

            var result = await coreDbContext.Database
                .SqlQueryRaw<ProveedorGridResultSet>("EXEC Core.SP_Proveedores_GetAll")
                .ToListAsync();

            return result;
        }

        public static async Task<List<ProveedorGetDto>> GetProveedorByIdAsync(IDbContextFactory<CoreDbContext> dbContextFactory, int idProveedor)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();
            var parameter = new SqlParameter("@IdProveedor", SqlDbType.Int) { Value = idProveedor };
            var result = await coreDbContext.Database
                .SqlQueryRaw<ProveedorGetDto>("EXEC Core.SP_Proveedores_GetById @IdProveedor", parameter)
                .ToListAsync();
            return result;
        }
        #endregion
    }
}