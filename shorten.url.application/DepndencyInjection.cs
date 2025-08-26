using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using shorten.url.application.Models;
using System.Reflection;

namespace shorten.url.application
{
    public static class DepndencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediator(config =>
            {
                config.Namespace = "shorten.url.Mediator";
                config.ServiceLifetime = ServiceLifetime.Scoped;
            });
           
            services.Configure<Domain>(configuration.GetSection("Domains"));

            return services;
        }
    }
}
