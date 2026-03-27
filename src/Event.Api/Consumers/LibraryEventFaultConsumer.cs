using Contracts;
using MassTransit;

namespace EventService.Consumers;

public class LibraryEventFaultConsumer(ILogger<LibraryEventFaultConsumer> logger)
    : IConsumer<Fault<LibraryEvent>>
{
    public Task Consume(ConsumeContext<Fault<LibraryEvent>> context)
    {
        var fault = context.Message;
        logger.LogError(
            "LibraryEvent processing failed after all retries. " +
            "MessageId={MessageId} EntityId={EntityId} EntityType={EntityType} Action={Action} Exceptions={Exceptions}",
            fault.FaultedMessageId,
            fault.Message.EntityId,
            fault.Message.EntityType,
            fault.Message.Action,
            string.Join("; ", fault.Exceptions.Select(e => e.Message)));

        return Task.CompletedTask;
    }
}
