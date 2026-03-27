using Library.Domain.Entities;

namespace Library.Domain.Interfaces
{
    public interface IBorrowingRepository : IRepository<Borrowing>
    {
        Task<IEnumerable<Borrowing>> GetActiveBorrowingsAsync(CancellationToken ct = default);
        Task<IEnumerable<Borrowing>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default);
        Task<IEnumerable<Borrowing>> GetActiveBorrowingsForCustomerAsync(Guid customerPartyId, CancellationToken ct = default);
        Task<IEnumerable<Borrowing>> GetPastBorrowingsForCustomerAsync(Guid customerPartyId, CancellationToken ct = default);
        Task<bool> HasActiveBorrowingsForCustomerAsync(Guid customerPartyId, CancellationToken ct = default);
    }
}
