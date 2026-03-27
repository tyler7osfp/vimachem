using FluentAssertions;
using Library.Domain;
using Library.Domain.Events;
using Library.Tests.Builders;

namespace Library.Tests.DomainEvents;

public class BookDomainEventTests
{
    [Fact]
    public void Create_RaisesBookCreatedEvent()
    {
        var author = new PartyBuilder().WithRole(RoleType.Author).Build();
        var book = new BookBuilder().WithAuthor(author).Build();

        var events = book.DomainEvents;

        events.Should().ContainSingle(e => e is BookCreatedEvent);
        events.OfType<BookCreatedEvent>().Single().AuthorPartyId.Should().Be(author.Id);
    }

    [Fact]
    public void Update_RaisesBookUpdatedEvent()
    {
        var book = new BookBuilder().Build();
        book.PopDomainEvents();

        book.Update("New Title", Guid.NewGuid());

        book.DomainEvents.Should().ContainSingle(e => e is BookUpdatedEvent);
    }

    [Fact]
    public void AddCopy_RaisesBookCopyAddedEvent()
    {
        var book = new BookBuilder().Build();
        book.PopDomainEvents();

        var copy = book.AddCopy();

        book.DomainEvents.Should().ContainSingle(e => e is BookCopyAddedEvent);
        book.DomainEvents.OfType<BookCopyAddedEvent>().Single().CopyId.Should().Be(copy.Id);
    }

    [Fact]
    public void AddCopies_RaisesOneEventPerCopy()
    {
        var book = new BookBuilder().Build();
        book.PopDomainEvents();

        var copies = book.AddCopies(3);

        var events = book.DomainEvents.OfType<BookCopyAddedEvent>().ToList();
        events.Should().HaveCount(3);
        events.Select(e => e.CopyId).Should().BeEquivalentTo(copies.Select(c => c.Id));
    }

    [Fact]
    public void RemoveCopy_WithMultipleCopies_RaisesBookCopyRemovedEvent()
    {
        var book = new BookBuilder().WithCopies(2).Build();
        book.PopDomainEvents();
        var copyId = book.Copies.First().Id;

        book.RemoveCopy(copyId);

        book.DomainEvents.Should().ContainSingle(e => e is BookCopyRemovedEvent);
    }

    [Fact]
    public void PopDomainEvents_ClearsEvents()
    {
        var book = new BookBuilder().Build();

        var popped = book.PopDomainEvents();

        book.DomainEvents.Should().BeEmpty();
        popped.Should().NotBeEmpty();
    }
}
