using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(b => b.IsDeleted).HasDefaultValue(false);

            builder.HasOne(b => b.Category).WithMany(c => c.Books).HasForeignKey(b => b.CategoryId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Author).WithMany().HasForeignKey(b => b.AuthorPartyId).OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(b => b.TotalCopies);
            builder.Ignore(b => b.AvailableCopies);
        }
    }
}
