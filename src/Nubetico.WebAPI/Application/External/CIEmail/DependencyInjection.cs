using System.Net.Http.Headers;
using System.Text;

namespace Nubetico.WebAPI.Application.External.CIEmail
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCIEmailServices(this IServiceCollection services, IConfiguration config)
        {
            string usr = config["CIEmailService:Credentials:Username"] ?? "";
            string pwd = config["CIEmailService:Credentials:Password"] ?? "";
            var byteArray = Encoding.ASCII.GetBytes($"{usr}:{pwd}");
            var base64Credentials = Convert.ToBase64String(byteArray);

            services.AddHttpClient("ci.smsbridge", client =>
            {
                client.BaseAddress = new Uri(config["CIEmailService:Url"] ?? "");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
            });

            services.AddTransient<CIEmailService>();

            return services;
        }
    }
}
