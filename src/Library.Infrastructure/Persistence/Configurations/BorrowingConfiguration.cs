using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations
{
    public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
    {
        public void Configure(EntityTypeBuilder<Borrowing> builder)
        {
            builder.HasKey(b => b.Id);

            builder
                .HasIndex(b => b.BookCopyId)
                .IsUnique()
                .HasFilter("\"ReturnedAt\" IS NULL");

            builder.HasOne(b => b.BookCopy)
                              .WithMany(c => c.Borrowings)
                              .HasForeignKey(b => b.BookCopyId)
                              .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(z => z.Customer).WithMany().HasForeignKey(z => z.CustomerPartyId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
