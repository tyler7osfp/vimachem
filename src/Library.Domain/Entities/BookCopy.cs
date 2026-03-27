using Library.Domain.Events;

namespace Library.Domain.Entities
{
    public class BookCopy : Entity
    {
        public Guid Id { get; private set; }
        public Guid BookId { get; private set; }
        public bool IsAvailable { get; private set; }
        public bool IsDeleted { get; private set; }
        public Book Book { get; private set; } = null!;
        public ICollection<Borrowing> Borrowings { get; private set; } = new List<Borrowing>();

        private BookCopy() { }

        public static BookCopy Create(Guid bookId)
            => new() { BookId = bookId, IsAvailable = true, Id = Guid.NewGuid() };

        public void Borrow()
        {
            if (IsDeleted) throw new DomainException("This copy has been removed from the pool.");
            if (!IsAvailable) throw new DomainException($"Book copy {Id} is not available.");
            IsAvailable = false;
        }

        public void Return()
        {
            if (IsAvailable) throw new DomainException($"Book copy {Id} is already available.");
            IsAvailable = true;
        }

        public void SoftDelete()
        {
            if (!IsAvailable) throw new DomainException("Cannot remove a copy that is currently borrowed.");
            IsAvailable = false;
            IsDeleted = true;
        }
    }
}
