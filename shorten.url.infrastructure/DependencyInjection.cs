using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using shorten.url.application.Contracts;
using shorten.url.infrastructure.ApiAuthentication;
using shorten.url.infrastructure.Persistence;
using shorten.url.infrastructure.Providers;
using shorten.url.infrastructure.Services;

namespace shorten.url.infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddSingleton<ICacheProvider, CacheProvider>();

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                options.TableName = "DistributedCache";
                options.SchemaName = "dbo";
            });

            services.AddScoped<ApiKeyAuthenticationHandler>();

            services.AddScoped<IRepository, GenericRepository>();
            services.AddScoped<IUrlService, UrlService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            }).AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, null);

            return services;
        }
    }
}
