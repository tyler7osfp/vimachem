using Contracts;
using EventService.Consumers;
using EventService.Models;
using EventService.Repos;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Events.Tests;

public class EventConsumerTests
{
    private static LibraryEventConsumer BuildConsumer(IEventWriteRepository? repo = null)
        => new(repo ?? Substitute.For<IEventWriteRepository>(), NullLogger<LibraryEventConsumer>.Instance);

    private static ConsumeContext<LibraryEvent> MakeContext(LibraryEvent message)
    {
        var ctx = Substitute.For<ConsumeContext<LibraryEvent>>();
        ctx.Message.Returns(message);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    [Fact]
    public async Task Consume_InsertsEventDocumentWithCorrectFields()
    {
        var repo = Substitute.For<IEventWriteRepository>();
        var consumer = BuildConsumer(repo);

        var message = new LibraryEvent
        {
            Id = Guid.NewGuid(),
            EntityId = Guid.NewGuid(),
            EntityType = EntityType.Book,
            Action = "Created",
            Timestamp = DateTime.UtcNow,
            Metadata = new Dictionary<string, string> { ["AuthorId"] = Guid.NewGuid().ToString() }
        };

        await consumer.Consume(MakeContext(message));

        await repo.Received(1).InsertAsync(
            Arg.Is<EventDocument>(d =>
                d.Id == message.Id &&
                d.EntityId == message.EntityId &&
                d.EntityType == message.EntityType &&
                d.Action == message.Action &&
                d.Timestamp == message.Timestamp),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_PropagatesMetadata()
    {
        var repo = Substitute.For<IEventWriteRepository>();
        var consumer = BuildConsumer(repo);

        var authorId = Guid.NewGuid().ToString();
        var message = new LibraryEvent
        {
            EntityType = EntityType.Book,
            Action = "Created",
            Metadata = new Dictionary<string, string> { ["AuthorId"] = authorId }
        };

        await consumer.Consume(MakeContext(message));

        await repo.Received(1).InsertAsync(
            Arg.Is<EventDocument>(d => d.Metadata["AuthorId"] == authorId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_BorrowingEvent_InsertsWithBorrowingEntityType()
    {
        var repo = Substitute.For<IEventWriteRepository>();
        var consumer = BuildConsumer(repo);

        var message = new LibraryEvent
        {
            EntityType = EntityType.Borrowing,
            Action = "Borrowed",
            Metadata = []
        };

        await consumer.Consume(MakeContext(message));

        await repo.Received(1).InsertAsync(
            Arg.Is<EventDocument>(d => d.EntityType == EntityType.Borrowing),
            Arg.Any<CancellationToken>());
    }
}
