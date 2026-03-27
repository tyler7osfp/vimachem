using Library.Domain.Events;

namespace Library.Domain.Entities
{
    public class Borrowing:Entity
    {
        public Guid Id { get; private set; }
        public Guid BookId { get; private set; }
        public Guid BookCopyId { get; private set; }
        public Guid CustomerPartyId { get; private set; }
        public DateTime BorrowedAt { get; private set; }
        public DateTime? ReturnedAt { get; private set; } = null!;
        public Party Customer { get; private set; } = null!;
        public BookCopy BookCopy { get; private set; } = null!;

        private Borrowing() { }

        public static Borrowing Start(Guid bookId, Guid bookCopyId, Guid customerPartyId)
        {
            var borrowing = new Borrowing
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                BookCopyId = bookCopyId,
                CustomerPartyId = customerPartyId,
                BorrowedAt = DateTime.UtcNow
            };
            borrowing.RaiseDomainEvent(new BorrowingCreatedEvent(borrowing.Id, bookId, bookCopyId, customerPartyId));
            return borrowing;
        }

        public void CompleteReturn()
        {
            if (!IsActive())
                throw new DomainException("This books has already been returned.");
            ReturnedAt = DateTime.UtcNow;
            RaiseDomainEvent(new BorrowingReturnedEvent(Id, BookId, BookCopyId, CustomerPartyId));
        }
        public bool IsActive() => ReturnedAt == null;

   
    }
}
