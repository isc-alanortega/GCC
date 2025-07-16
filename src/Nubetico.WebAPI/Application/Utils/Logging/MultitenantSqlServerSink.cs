using Serilog.Events;
using Serilog;
using Serilog.Core;

namespace Nubetico.WebAPI.Application.Utils.Logging
{
    public class MultitenantSqlServerSink : ILogEventSink
    {
        private readonly IFormatProvider? _formatProvider;
        private readonly IServiceProvider _serviceProvider;

        public MultitenantSqlServerSink(IServiceProvider serviceProvider, IFormatProvider? formatProvider)
        {
            _serviceProvider = serviceProvider;
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            using var scope = _serviceProvider.CreateScope();
            var tenantConnectionService = scope.ServiceProvider.GetRequiredService<TenantConnectionService>();
            var tenant = tenantConnectionService.GetTenant();

            if (tenant != null && !string.IsNullOrEmpty(tenant.ConnectionString))
            {
                var properties = logEvent.Properties.Select(p => new LogEventProperty(p.Key, p.Value));

                LogEvent logEventWithUtc = new LogEvent(
                    logEvent.Timestamp.ToUniversalTime(),
                    logEvent.Level,
                    logEvent.Exception,
                    logEvent.MessageTemplate,
                    properties);

                // Configuracion de BD para el Log
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(
                        connectionString: tenant.ConnectionString,
                        schemaName: "Core",
                        tableName: "Logs",
                        autoCreateSqlTable: true,
                        restrictedToMinimumLevel: LogEventLevel.Warning,
                        formatProvider: _formatProvider)
                    .CreateLogger();

                logger.Write(logEventWithUtc);
            }
        }
    }
}
