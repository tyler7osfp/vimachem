namespace Library.Domain.Events
{
    public record BookCreatedEvent(Guid BookId, Guid AuthorPartyId) : IDomainEvent;
    public record BookUpdatedEvent(Guid BookId) : IDomainEvent;
    public record BookDeletedEvent(Guid BookId) : IDomainEvent;
    public record BookCopyAddedEvent(Guid BookId, Guid CopyId) : IDomainEvent;
    public record BookCopyRemovedEvent(Guid BookId, Guid CopyId) : IDomainEvent;
    public record CategoryCreatedEvent(Guid CategoryId) : IDomainEvent;
    public record CategoryUpdatedEvent(Guid CategoryId) : IDomainEvent;
    public record CategoryDeletedEvent(Guid CategoryId) : IDomainEvent;
    public record PartyCreatedEvent(Guid PartyId) : IDomainEvent;
    public record PartyUpdatedEvent(Guid PartyId) : IDomainEvent;
    public record PartyDeletedEvent(Guid PartyId) : IDomainEvent;
    public record PartyRoleAddedEvent(Guid PartyId, string Role) : IDomainEvent;
    public record PartyRoleRemovedEvent(Guid PartyId, string Role) : IDomainEvent;
    public record BorrowingCreatedEvent(Guid BorrowingId, Guid BookId, Guid CopyId, Guid CustomerPartyId) : IDomainEvent;
    public record BorrowingReturnedEvent(Guid BorrowingId, Guid BookId, Guid CopyId, Guid CustomerPartyId) : IDomainEvent;

}
