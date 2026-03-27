using Library.Domain.Events;

namespace Library.Domain.Entities
{
    public class Category: Entity
    {
        public Guid Id { get;private set; }
        public string Name { get; private set; } = string.Empty;
        public bool IsDeleted { get; private set; }
        public ICollection<Book> Books { get; private set; } = new List<Book>();

        private Category() { }

        public static Category Create(string name)
        {
            Validate(name);
            var category = new Category() { Id = Guid.NewGuid(), Name = name };
            category.RaiseDomainEvent(new CategoryCreatedEvent(category.Id));
            return category;
        }


        public void Update(string name)
        {
            Validate(name);
            Name = name;
            RaiseDomainEvent(new CategoryUpdatedEvent(Id));
        }

        public void MarkDeleted()
        {
            IsDeleted = true;
            RaiseDomainEvent(new CategoryDeletedEvent(Id));
        }

        private static void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required.");
        }
    }
}
