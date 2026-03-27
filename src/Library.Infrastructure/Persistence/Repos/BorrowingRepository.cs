using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence.Repos
{
    public class BorrowingRepository(LibraryDbContext _ctx) : IBorrowingRepository
    {
        public async Task AddAsync(Borrowing entity, CancellationToken ct = default)
        {
            await _ctx.Borrowings.AddAsync(entity, ct);
        }

        public async Task<IEnumerable<Borrowing>> GetActiveBorrowingsAsync(CancellationToken ct = default)
        {
            return await _ctx.Borrowings
                .Where(b => b.ReturnedAt == null)
                .Include(b => b.BookCopy).ThenInclude(c => c.Book)
                .Include(b => b.Customer)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Borrowing>> GetAllAsync(CancellationToken ct = default)
        {
            return await _ctx.Borrowings
                .Include(z => z.BookCopy).ThenInclude(c => c.Book)
                .Include(z => z.Customer)
                .ToListAsync(ct);
        }

        public Task<Borrowing?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return _ctx.Borrowings
                .Include(z => z.BookCopy).ThenInclude(c => c.Book)
                .Include(z => z.Customer)
                .FirstOrDefaultAsync(z => z.Id == id, ct);
        }

        public void Update(Borrowing entity)
        {
            _ctx.Borrowings.Update(entity);
        }

        public async Task<IEnumerable<Borrowing>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            return await _ctx.Borrowings
                .Where(b => b.BookCopy.BookId == bookId)
                .Include(b => b.BookCopy)
                .Include(b => b.Customer)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Borrowing>> GetActiveBorrowingsForCustomerAsync(
            Guid customerPartyId,
            CancellationToken ct = default)
        {
            return await _ctx.Borrowings
                .AsNoTracking()
                .Where(b => b.CustomerPartyId == customerPartyId && b.ReturnedAt == null)
                .OrderByDescending(b => b.BorrowedAt)
                .Include(b => b.BookCopy).ThenInclude(c => c.Book)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Borrowing>> GetPastBorrowingsForCustomerAsync(
            Guid customerPartyId,
            CancellationToken ct = default)
        {
            return await _ctx.Borrowings
                .AsNoTracking()
                .Where(b => b.CustomerPartyId == customerPartyId && b.ReturnedAt != null)
                .OrderByDescending(b => b.ReturnedAt)
                .Include(b => b.BookCopy).ThenInclude(c => c.Book)
                .ToListAsync(ct);
        }

        public Task<bool> HasActiveBorrowingsForCustomerAsync(Guid customerPartyId, CancellationToken ct = default)
        {
            return _ctx.Borrowings.AnyAsync(b => b.CustomerPartyId == customerPartyId && b.ReturnedAt == null, ct);
        }
    }
}
