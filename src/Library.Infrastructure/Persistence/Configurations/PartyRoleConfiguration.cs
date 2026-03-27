using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations
{
    public class PartyRoleConfiguration : IEntityTypeConfiguration<PartyRole>
    {
        public void Configure(EntityTypeBuilder<PartyRole> builder)
        {
            builder.HasKey(r => new { r.PartyId, r.Role });
            builder.Property(r => r.Role).HasConversion<string>();
        }
    }
}
