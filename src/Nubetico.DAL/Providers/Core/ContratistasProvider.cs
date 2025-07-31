using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.ResultSets.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Contratistas;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;

namespace Nubetico.DAL.Providers.Core
{
    public static class ContratistasProvider
    {
        #region POST
        public static async Task<ContratistaResult?> CreateContratistaAsync(IDbContextFactory<CoreDbContext> dbContextFactory, ContratistasDto contratistaRequest)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();
            var parameters = new[]
            {
        new SqlParameter("@UUID", SqlDbType.UniqueIdentifier) { Value = contratistaRequest.UUID },
        new SqlParameter("@Folio", SqlDbType.NVarChar, 10) { Value = (object)contratistaRequest.Folio ?? DBNull.Value },
        new SqlParameter("@Rfc", SqlDbType.NVarChar, 13) { Value = contratistaRequest.Rfc ?? (object)DBNull.Value },
        new SqlParameter("@Email", SqlDbType.NVarChar, 250) { Value = (object)contratistaRequest.Email ?? DBNull.Value },
        new SqlParameter("@Web", SqlDbType.NVarChar, 250) { Value = (object)contratistaRequest.Web ?? DBNull.Value },
        new SqlParameter("@Credito", SqlDbType.Bit) { Value = contratistaRequest.Credito },
        new SqlParameter("@Telefono", SqlDbType.NVarChar, 10) { Value = (object)contratistaRequest.Telefono ?? DBNull.Value },
        new SqlParameter("@Celular", SqlDbType.NVarChar, 10) { Value = (object)contratistaRequest.Celular ?? DBNull.Value },
        new SqlParameter("@LimiteCreditoMXN", SqlDbType.Money) { Value = contratistaRequest.LimiteCreditoMXN },
        new SqlParameter("@LimiteCreditoUSD", SqlDbType.Money) { Value = contratistaRequest.LimiteCreditoUSD },
        new SqlParameter("@DiasCredito", SqlDbType.Int) { Value = contratistaRequest.DiasCredito },
        new SqlParameter("@DiasGracia", SqlDbType.Int) { Value = (object)contratistaRequest.DiasGracia ?? DBNull.Value },
        new SqlParameter("@SaldoMXN", SqlDbType.Money) { Value = contratistaRequest.SaldoMXN },
        new SqlParameter("@SaldoUSD", SqlDbType.Money) { Value = contratistaRequest.SaldoUSD },
        new SqlParameter("@IdTipoPersonaSat", SqlDbType.Int) { Value = contratistaRequest.IdTipoPersonaSat },
        new SqlParameter("@CuentaContable", SqlDbType.NChar, 50) { Value = (object)contratistaRequest.CuentaContable ?? DBNull.Value },
        new SqlParameter("@IdFormaPago", SqlDbType.Int) { Value = (object)contratistaRequest.IdFormaPago ?? DBNull.Value },
        new SqlParameter("@IdTipoRegimenFiscal", SqlDbType.Int) { Value = contratistaRequest.IdTipoRegimenFiscal },
        new SqlParameter("@Habilitado", SqlDbType.Bit) { Value = contratistaRequest.Habilitado },
        new SqlParameter("@IdUsuarioAlta", SqlDbType.Int) { Value = (object)contratistaRequest.IdUsuarioAlta ?? DBNull.Value },
        //new SqlParameter("@IdSucursal", SqlDbType.Int) { Value = (object)contratistaRequest.IdSucursal ?? DBNull.Value },
        new SqlParameter("@NombreComercial", SqlDbType.NVarChar, 50) { Value = (object)contratistaRequest.NombreComercial ?? DBNull.Value },
        new SqlParameter("@RazonSocial", SqlDbType.NVarChar, 50) { Value = (object)contratistaRequest.RazonSocial ?? DBNull.Value },
        new SqlParameter("@IdRegimenFiscal", SqlDbType.Int) { Value = contratistaRequest.IdRegimenFiscal },
        new SqlParameter("@IdTipoMetodoPago", SqlDbType.Int) { Value = contratistaRequest.IdTipoMetodoPago },
        new SqlParameter("@IdUsoCFDI", SqlDbType.Int) { Value = contratistaRequest.IdUsoCFDI },
        new SqlParameter("@ReferenciaBancaria", SqlDbType.NVarChar, 50) { Value = (object)contratistaRequest.ReferenciaBancaria ?? DBNull.Value }
    };

            var result = await coreDbContext.Database
                .SqlQueryRaw<ContratistaResult>(
                    "EXEC [Core].[SP_Contratistas_Post] @UUID, @Folio, @Rfc, @Email, @Web, @Credito, " +
                    "@Telefono, @Celular, @LimiteCreditoMXN, @LimiteCreditoUSD, @DiasCredito, @DiasGracia, " +
                    "@SaldoMXN, @SaldoUSD, @IdTipoPersonaSat, @CuentaContable, @IdFormaPago, @IdTipoRegimenFiscal, " +
                    "@Habilitado, @IdUsuarioAlta, @NombreComercial, @RazonSocial, @IdRegimenFiscal, " +
                    "@IdTipoMetodoPago, @IdUsoCFDI, @ReferenciaBancaria",
                    parameters
                )
                .ToListAsync();

            return result.FirstOrDefault();
        }
        #endregion
        #region GET
        public static async Task<PaginatedListDto<ContratistaGridResultSet>> GetContratistasPaginadoAsync(IDbContextFactory<CoreDbContext> dbContextFactory, int limit, int offset, string? orderBy, string? nombre, string? rfc)
        {
            using
            var coreDbContext = await dbContextFactory.CreateDbContextAsync();
            var parameters = new[] {
            new SqlParameter("@Limit", limit),
                new SqlParameter("@Offset", offset),
                new SqlParameter("@Nombre", (object ? ) nombre ?? DBNull.Value),
                new SqlParameter("@RFC", (object ? ) rfc ?? DBNull.Value),
                new SqlParameter("@OrderBy", (object ? ) orderBy ?? "NombreComercial ASC")
        };
            var result = await coreDbContext.Database.SqlQueryRaw<ContratistaGridResultSet>("EXEC Core.SP_Contratistas_GetPaginado @Limit, @Offset, @Nombre, @RFC, @OrderBy", parameters).ToListAsync();
            return new PaginatedListDto<ContratistaGridResultSet>
            {
                Data = result,
            };
        }

        public static async Task<List<ContratistasDto>> GetContratistaByIdAsync(IDbContextFactory<CoreDbContext> dbContextFactory, int idContratista)
        {
            using var coreDbContext = await dbContextFactory.CreateDbContextAsync();
            var parameter = new SqlParameter("@IdContratista", SqlDbType.Int) { Value = idContratista };
            var result = await coreDbContext.Database
                .SqlQueryRaw<ContratistasDto>("EXEC Core.SP_Contratistas_GetById @IdContratista", parameter)
                .ToListAsync();
            return result;
        }
        #endregion
        #region PUT
        public static async Task<ContratistaResult?> PutSaveContratista(IDbContextFactory<CoreDbContext> dbContextFactory, ContratistasDto request)
        {
            try
            {
                using var context = dbContextFactory.CreateDbContext();

                var parameters = new[]
                {
            new SqlParameter("@UUID", request.UUID),
            new SqlParameter("@Folio", (object)request.Folio ?? DBNull.Value),
            new SqlParameter("@Rfc", request.Rfc),
            new SqlParameter("@Email", request.Email ?? string.Empty),
            new SqlParameter("@Web", request.Web ?? string.Empty),
            new SqlParameter("@Credito", request.Credito),
            new SqlParameter("@Telefono", request.Telefono ?? string.Empty),
            new SqlParameter("@Celular", request.Celular ?? string.Empty),
            new SqlParameter("@LimiteCreditoMXN", request.LimiteCreditoMXN),
            new SqlParameter("@LimiteCreditoUSD", request.LimiteCreditoUSD),
            new SqlParameter("@DiasCredito", request.DiasCredito),
            new SqlParameter("@DiasGracia", request.DiasGracia),
            new SqlParameter("@SaldoMXN", request.SaldoMXN),
            new SqlParameter("@SaldoUSD", request.SaldoUSD),
            new SqlParameter("@IdTipoPersonaSat", request.IdTipoPersonaSat),
            new SqlParameter("@CuentaContable", request.CuentaContable ?? string.Empty),
            new SqlParameter("@IdFormaPago", request.IdFormaPago),
            new SqlParameter("@IdTipoRegimenFiscal", request.IdTipoRegimenFiscal),
            new SqlParameter("@Habilitado", request.Habilitado),
            new SqlParameter("@NombreComercial", request.NombreComercial ?? string.Empty),
            new SqlParameter("@RazonSocial", request.RazonSocial ?? string.Empty),
            new SqlParameter("@IdRegimenFiscal", request.IdRegimenFiscal),
            new SqlParameter("@IdTipoMetodoPago", request.IdTipoMetodoPago),
            new SqlParameter("@IdUsoCFDI", request.IdUsoCFDI),
            new SqlParameter("@ReferenciaBancaria", request.ReferenciaBancaria ?? string.Empty)
        };

                var result = await context.Database
                    .SqlQueryRaw<ContratistaResult>("EXEC [Core].[SP_Contratistas_Put] @UUID, @Folio, @Rfc, @Email, @Web, @Credito, @Telefono, @Celular, @LimiteCreditoMXN, @LimiteCreditoUSD, @DiasCredito, @DiasGracia, @SaldoMXN, @SaldoUSD, @IdTipoPersonaSat, @CuentaContable, @IdFormaPago, @IdTipoRegimenFiscal, @Habilitado, @NombreComercial, @RazonSocial, @IdRegimenFiscal, @IdTipoMetodoPago, @IdUsoCFDI, @ReferenciaBancaria", parameters)
                    .ToListAsync();

                return result.SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar SP_Contratistas_Put", ex);
            }
        }

        #endregion
    }
}
