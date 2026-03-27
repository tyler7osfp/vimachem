using Library.Domain.Entities;

namespace Library.Domain.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<Book?> GetByTitleAsync(string title, CancellationToken ct = default);
        Task<IEnumerable<Book>> GetByAuthorPartyIdAsync(Guid authorPartyId, CancellationToken ct = default);
        void Delete(Book entity);
        Task<bool> HasBooksAsAuthorAsync(Guid authorPartyId, CancellationToken ct = default);

        void TrackNewCopy(BookCopy copy);
    }
}
