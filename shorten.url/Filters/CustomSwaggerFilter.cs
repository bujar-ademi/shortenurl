using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace shorten.url.Filters
{
    public class CustomSwaggerFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var routes = swaggerDoc.Paths.Where(x => x.Key.ToLower().StartsWith("/api/")).ToList();

            swaggerDoc.Paths.Clear();
            routes.ForEach(x => { swaggerDoc.Paths.Add(x.Key, x.Value);  });

        }
    }
}
