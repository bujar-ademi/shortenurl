using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using shorten.url.application;
using shorten.url.application.Contracts;
using shorten.url.application.Features.Query;
using shorten.url.Filters;
using shorten.url.infrastructure;
using shorten.url.infrastructure.ApiAuthentication;
using shorten.url.infrastructure.Persistence;
using shorten.url.infrastructure.Persistence.Seeds;
using shorten.url.Services;

namespace shorten.url
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var configuration = builder.Configuration;

            builder.Services.AddApplication(configuration)
                .AddInfrastructure(configuration);

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(setup =>
            {
                setup.DocumentFilter<CustomSwaggerFilter>();

                setup.ResolveConflictingActions(apiDescription => apiDescription.First());

                setup.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Name = "x-api-key",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Add ApiKey in the textbox",
                    Scheme = ApiKeyAuthenticationOptions.DefaultScheme
                });

                var key = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header
                };
                var requirement = new OpenApiSecurityRequirement
                {
                    { key, new List<string>() }
                };

                setup.AddSecurityRequirement(requirement);
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    if (context.Database.IsSqlServer())
                    {
                        await context.Database.MigrateAsync();
                    }

                    var repository = services.GetRequiredService<IRepository>();

                    await DefaultApiClient.SeedAsync(repository);
                }
                catch (Exception) { }
            }

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stylr API");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/{id}", async (string id, IMediator mediator) =>
            {
                var url = await mediator.Send(new GetRedirectAddressQuery { UniqueId = id }).ConfigureAwait(false);

                return Results.Redirect(url);
            });

            app.MapGet("/not-found/{id}", (string id) =>
            {
                var html = @"<!DOCTYPE html>
<html lang='en'>

    <head>
        <meta charset='UTF-8'>
        <meta http-equiv='X-UA-Compatible' content='IE=edge'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Link not found!</title>
        <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.2.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    </head>

    <body>
        <div class='vh-100 d-flex justify-content-center align-items-center'>
            <div class='col-md-4'>
                <div class='border border-2 border-danger'></div>
                <div class='card  bg-white shadow p-5'>
                    <div class='mb-4 text-center'>
                        <svg xmlns='http://www.w3.org/2000/svg' class='text-danger' width='75' height='75'
                            fill='currentColor' class='bi bi-x-circle' viewBox='0 0 16 16'>
                            <path d='M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z' />
                            <path
                                d='M10.97 4.97a.235.235 0 0 0-.02.022L7.477 9.417 5.384 7.323a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-1.071-1.05z' />
                        </svg>
                    </div>
                    <div class='text-center'>
                        <h1>Link not found!</h1>
                        <p>The link you requested does not exists!</p>
                    </div>
                </div>
            </div>
        </div>
    </body>
</html>";

                return Results.Content(html, "text/html");
            });

            await app.RunAsync();
        }
    }
}