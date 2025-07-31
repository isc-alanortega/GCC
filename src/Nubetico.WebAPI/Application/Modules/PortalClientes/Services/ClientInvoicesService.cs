using CwEM.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.PortalClientes;

namespace Nubetico.WebAPI.Application.Modules.PortalClientes.Services
{
    public class ClientInvoicesService
    {
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

        public ClientInvoicesService(IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _coreDbContextFactory = coreDbContextFactory;
        }

        public async Task<PaginatedListDto<ExternalClientInvoices>> GetExternalClientInvoices(int limit, int offset, ExternalInvoicesFilter? filter)
        {
            using var cwEMDbContext = new CW_EduardoMagallonDbContext(
                    new DbContextOptionsBuilder<CW_EduardoMagallonDbContext>().UseSqlServer(
                        await CwEMDbUtil.GetConnectionStringAsync(_coreDbContextFactory)
                    ).Options
            );

            // Get External Client ID
            int? ExternalClientID = 0;
            using (var context = _coreDbContextFactory.CreateDbContext())
            {
                var result = await context.Entidades.FirstOrDefaultAsync(entidad => entidad.UUID_Entidad.ToString() == filter!.EntityContactGuid);
                if (result != null)
                {
                    ExternalClientID = result.IdExterno;
                }
            }

            var query = from ventas in cwEMDbContext.AD_Ventas
                                join entidades in cwEMDbContext.AD_Entidades
                                on ventas.IDCliente equals entidades.IDEntidad
                                where ventas.IDCliente == 8 && (entidades == null || entidades.TipoEntidad == 1)
                                select new ExternalClientInvoices
                                {
                                    Serial = ventas.SerieFE,
                                    Numeric_Folio = ventas.FolioFE,
                                    Folio = ventas.SerieFE + ventas.FolioFE,
                                    InvoiceType = ventas.Tipo == "F"
                                        ? "FACTURA"
                                        : ventas.Tipo == "P"
                                            ? "PAGO"
                                            : "NOTA DE CRÉDITO",
                                    Date = ventas.Fecha,
                                    BusinessName = entidades.RazonSocial,
                                    Total = ventas.Total,
                                    Balance = ventas.Saldo,
                                    Payed = ventas.Pagado ?? false,
                                    Status = (ventas.Pagado.HasValue && ventas.Pagado.Value) 
                                        ? "PAGADA"
                                        : ventas.Estado == "A"
                                            ? "ACTIVA"
                                            : "CANCELADA"
                                };
            if (filter is not null)
            {
                if (!string.IsNullOrEmpty(filter.Folio))
                    query = query.Where(i => i.Folio.Contains(filter.Folio));

                if (!string.IsNullOrEmpty(filter.Type) && filter.Type != "TODAS")
                    query = query.Where(i => i.InvoiceType.Contains(filter.Type));

                if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "TODAS")
                    query = query.Where(i => i.Status == filter.Status);

                if (filter.DateFrom.HasValue)
                    query = query.Where(i => i.Date >= filter.DateFrom.Value);

                if (filter.DateTo.HasValue)
                    query = query.Where(i => i.Date <= filter.DateTo.Value);
            }

            var total = await query.CountAsync();

            var invoices = await query.Skip(offset).Take(limit).OrderByDescending(invoice => invoice.Date).ToListAsync();

            return new PaginatedListDto<ExternalClientInvoices>
            {
                RecordsTotal = total,
                RecordsFiltered = invoices.Count,
                Data = invoices
            };
        }
    }
}
