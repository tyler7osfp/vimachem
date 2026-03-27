using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations
{
    public class PartyConfiguration : IEntityTypeConfiguration<Party>
    {
        public void Configure(EntityTypeBuilder<Party> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            builder.HasIndex(p => p.Email).IsUnique();

            builder.HasMany(p => p.Roles)
                .WithOne()
                .HasForeignKey(r => r.PartyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
