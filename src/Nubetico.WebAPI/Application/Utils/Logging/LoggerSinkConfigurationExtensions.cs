using Serilog;
using Serilog.Configuration;

namespace Nubetico.WebAPI.Application.Utils.Logging
{
    public static class LoggerSinkConfigurationExtensions
    {
        public static LoggerConfiguration MultitenantSqlServerSink(
        this LoggerSinkConfiguration sinkConfiguration,
        IServiceProvider serviceProvider,
        IFormatProvider? formatProvider = null)
        {
            return sinkConfiguration.Sink(new MultitenantSqlServerSink(serviceProvider, formatProvider));
        }
    }
}
