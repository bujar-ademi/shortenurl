using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using shorten.url.domain;

namespace shorten.url.infrastructure.Persistence.Configurations
{
    internal abstract class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T: BaseEntity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            ConfigureEntity(builder);
        }

        public abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
    }
}
