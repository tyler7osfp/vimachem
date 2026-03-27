using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Persistence.Configurations
{
    public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
    {
        public void Configure(EntityTypeBuilder<BookCopy> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasOne(c => c.Book)
                   .WithMany(b => b.Copies)
                   .HasForeignKey(c => c.BookId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
