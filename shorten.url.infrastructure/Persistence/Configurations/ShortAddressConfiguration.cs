using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using shorten.url.domain;

namespace shorten.url.infrastructure.Persistence.Configurations
{
    internal class ShortAddressConfiguration : BaseEntityTypeConfiguration<ShortAddress>
    {
        public override void ConfigureEntity(EntityTypeBuilder<ShortAddress> builder)
        {
            builder.ToTable("Addresses");

            builder.Property(x => x.Domain).HasMaxLength(150);
            builder.Property(x => x.UniqueId).HasMaxLength(100);
            builder.Property(x => x.ShortUrl).HasMaxLength(128);
            builder.Property(x => x.RedirectUrl).HasMaxLength(2500);

            builder.HasOne(x => x.ApiClient).WithMany().HasForeignKey(x => x.ApiClientId);
            builder.HasIndex(x => x.ShortUrl).IsUnique();
            builder.HasIndex(x => x.UniqueId).IsUnique();
        }
    }
}
