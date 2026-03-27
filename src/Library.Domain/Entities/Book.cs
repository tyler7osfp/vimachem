using Library.Domain.Events;

namespace Library.Domain.Entities
{
    public class Book : Entity
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public Guid CategoryId { get; private set; }
        public Guid AuthorPartyId { get; private set; }
        public bool IsDeleted { get; private set; }
        public Category Category { get; private set; } = null!;
        public Party Author { get; private set; } = null!;
        public ICollection<BookCopy> Copies { get; private set; } = new List<BookCopy>();

        public int TotalCopies => Copies.Count(c => !c.IsDeleted);
        public int AvailableCopies => Copies.Count(c => !c.IsDeleted && c.IsAvailable);

        private Book() { }

        public static Book Create(string title, Party author, Guid categoryId, int copies)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Title is required.");
            if (!author.HasRole(RoleType.Author))
                throw new DomainException("Party does not have the Author role.");
            if (copies < 1)
                throw new DomainException("Copies must be at least 1.");

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = title,
                CategoryId = categoryId,
                AuthorPartyId = author.Id
            };

            for (var i = 0; i < copies; i++)
                book.Copies.Add(BookCopy.Create(book.Id));

            book.RaiseDomainEvent(new BookCreatedEvent(book.Id, author.Id));
            return book;
        }

        public void Update(string title, Guid categoryId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Title is required.");
            Title = title;
            CategoryId = categoryId;
            RaiseDomainEvent(new BookUpdatedEvent(Id));
        }

        public bool IsAvailable() => Copies.Any(c => c.IsAvailable && !c.IsDeleted);

        public BookCopy BorrowCopy(Party customer)
        {
            if (!customer.HasRole(RoleType.Customer))
                throw new DomainException("Party is not a Customer.");

            var copy = Copies.FirstOrDefault(c => c.IsAvailable && !c.IsDeleted)
                ?? throw new DomainException("No available copies to borrow.");
            copy.Borrow();
            return copy;
        }

        public void ReturnCopy(Guid copyId)
        {
            var copy = Copies.FirstOrDefault(c => c.Id == copyId)
                ?? throw new DomainException("Copy not found on this book.");
            copy.Return();
        }

        public IReadOnlyList<BookCopy> AddCopies(int count)
        {
            if (count < 1)
                throw new DomainException("Count must be at least 1.");

            var added = new List<BookCopy>(count);
            for (var i = 0; i < count; i++)
            {
                var copy = BookCopy.Create(Id);
                Copies.Add(copy);
                RaiseDomainEvent(new BookCopyAddedEvent(Id, copy.Id));
                added.Add(copy);
            }
            return added;
        }

        public BookCopy AddCopy() => AddCopies(1)[0];

        public void RemoveCopy(Guid copyId)
        {
            var copy = Copies.FirstOrDefault(c => c.Id == copyId && !c.IsDeleted)
                ?? throw new DomainException("Copy not found or already removed.");

            if (TotalCopies <= 1)
                throw new DomainException("Cannot remove the last active copy of a book.");

            copy.SoftDelete();
            RaiseDomainEvent(new BookCopyRemovedEvent(Id, copyId));
        }

        public void MarkDeleted()
        {
            IsDeleted = true;
            RaiseDomainEvent(new BookDeletedEvent(Id));
        }
    }
}
