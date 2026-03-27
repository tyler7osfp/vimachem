using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence.Repos
{
    public class BookRepository(LibraryDbContext _ctx) : IBookRepository
    {
        private IQueryable<Book> WithIncludes() =>
            _ctx.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Copies);

        public async Task AddAsync(Book entity, CancellationToken ct = default)
            => await _ctx.Books.AddAsync(entity, ct);

        public void Delete(Book entity)
        {
            _ctx.Books.Update(entity);
        }

        public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default)
            => await WithIncludes().ToListAsync(ct);

        public Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => WithIncludes().FirstOrDefaultAsync(b => b.Id == id, ct);

        public Task<Book?> GetByTitleAsync(string title, CancellationToken ct = default)
            => WithIncludes().FirstOrDefaultAsync(
                b => EF.Functions.ILike(EF.Property<string>(b, "Title"), title), ct);

        public async Task<IEnumerable<Book>> GetByAuthorPartyIdAsync(Guid authorPartyId, CancellationToken ct = default)
            => await WithIncludes()
                .Where(b => b.AuthorPartyId == authorPartyId)
                .OrderBy(b => b.Title)
                .ToListAsync(ct);

        public void Update(Book entity)
            => _ctx.Books.Update(entity);

        public Task<bool> HasBooksAsAuthorAsync(Guid authorPartyId, CancellationToken ct = default)
            => _ctx.Books.AnyAsync(b => b.AuthorPartyId == authorPartyId, ct);


        //pesky bug
        public void TrackNewCopy(BookCopy copy)
            => _ctx.Entry(copy).State = EntityState.Added;
    }
}
