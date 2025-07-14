using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nubetico.WebAPI.Application;
using Nubetico.WebAPI.Application.Utils.Logging;
using Nubetico.WebAPI.Filters;
using Nubetico.WebAPI.Hubs;
using Nubetico.WebAPI.Middlewares;
using Serilog;
using System.Globalization;
using System.Text;

namespace Nubetico.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMemoryCache();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("es-MX"),
                    new CultureInfo("en-US")
                };

                options.DefaultRequestCulture = new RequestCulture("es-MX");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
                {
                    var userLangs = context.Request.Headers["Accept-Language"].ToString();
                    var firstLang = userLangs.Split(',').FirstOrDefault();
                    var culture = new CultureInfo(firstLang ?? "es-MX");
                    return Task.FromResult(new ProviderCultureResult(culture.Name, culture.Name));
                }));
            });

            builder.Services.AddScoped<TurnstileFilter>();
            builder.Services.AddDataAccessLayerTenant(builder.Configuration);
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.AddSignalR();

            // Jwt
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                        ValidAudience = builder.Configuration["JwtConfig:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // CORS Config
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    if (builder.Environment.IsDevelopment())
                        policy.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    else
                        policy.WithOrigins((builder.Configuration["CorsPolicyConfig:HostsAllowed"] ?? "").Split(","))
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });
            });

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ExceptionFilter>();
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "nubetico API",
                    Description = "Endpoints de nubetico"

                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Ingrese un token válido",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            builder.Services.AddHttpContextAccessor();

            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .Enrich.With(new UserIdEnricher(services.GetRequiredService<IHttpContextAccessor>()))
                    .WriteTo.MultitenantSqlServerSink(services)
                    .WriteTo.Console();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseWebAssemblyDebugging();
            }

            // Hubs de SignalR
            app.MapHub<NotificacionesHub>("/notificaciones");

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapFallbackToFile("index.html");

            var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseCors();
            app.UseMiddleware<TenantMiddleware>();
            app.UseMiddleware<LocalizationMiddleware>();

            app.Run();
        }
    }
}
