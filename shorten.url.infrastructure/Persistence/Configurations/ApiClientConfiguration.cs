using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using shorten.url.domain;

namespace shorten.url.infrastructure.Persistence.Configurations
{
    internal class ApiClientConfiguration : BaseEntityTypeConfiguration<ApiClient>
    {
        public override void ConfigureEntity(EntityTypeBuilder<ApiClient> builder)
        {
            builder.ToTable("ApiClients");

            builder.Property(x => x.ApiKey).HasMaxLength(128).IsRequired();
            builder.Property(x => x.ClientName).HasMaxLength(128);

            builder.HasIndex(x => x.ApiKey).IsUnique();
        }
    }
}
