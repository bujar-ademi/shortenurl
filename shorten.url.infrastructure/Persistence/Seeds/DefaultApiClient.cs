using Microsoft.EntityFrameworkCore;
using shorten.url.application;
using shorten.url.application.Contracts;
using shorten.url.domain;

namespace shorten.url.infrastructure.Persistence.Seeds
{
    public static class DefaultApiClient
    {
        public static async Task SeedAsync(IRepository repository)
        {
            var testClient = await repository.ListGenericQueryable<ApiClient>(x => x.ClientName == "test").FirstOrDefaultAsync();
            if (testClient == null)
            {
                testClient = new ApiClient
                {
                    ApiKey = Randomizer.GenerateApiKey("st"),
                    ClientName = "test"
                };
                await repository.AddAsync(testClient);
                await repository.SaveChangesAsync();
            }
        }
    }
}
