using Nubetico.Frontend.Services.Core.XmlServices.Mapper;
using Nubetico.Frontend.Services.Core.XmlServices.ReadXmlServices;

namespace Nubetico.Frontend.Services.Core.XmlServices
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddXmlServices(this IServiceCollection services)
        {
            services.AddTransient<IXmlReader, XmlReader>();
            services.AddTransient<IFacturaDataMapper, FacturaDataMapper>();
            services.AddTransient<InvoiceXMLExtractorService>();

            return services;
        }
    }
}
