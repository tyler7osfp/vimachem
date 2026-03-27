using FluentAssertions;
using Library.Domain.Events;
using Library.Tests.Builders;

namespace Library.Tests.DomainEvents;

public class CategoryDomainEventTests
{
    [Fact]
    public void Create_RaisesCategoryCreatedEvent()
    {
        var category = new CategoryBuilder().Build();
        category.DomainEvents.Should().ContainSingle(e => e is CategoryCreatedEvent);
    }

    [Fact]
    public void Update_RaisesCategoryUpdatedEvent()
    {
        var category = new CategoryBuilder().Build();
        category.PopDomainEvents();

        category.Update("New Name");

        category.DomainEvents.Should().ContainSingle(e => e is CategoryUpdatedEvent);
    }

    [Fact]
    public void MarkDeleted_SetsIsDeletedAndRaisesCategoryDeletedEvent()
    {
        var category = new CategoryBuilder().Build();
        category.PopDomainEvents();

        category.MarkDeleted();

        category.IsDeleted.Should().BeTrue();
        category.DomainEvents.Should().ContainSingle(e => e is CategoryDeletedEvent);
    }
}
