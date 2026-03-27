namespace Library.Domain.Events
{
    public abstract class Entity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void RaiseDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);

        public IReadOnlyList<IDomainEvent> PopDomainEvents()
        {
            var events = _domainEvents.ToList();
            _domainEvents.Clear();
            return events;
        }
    }
}
