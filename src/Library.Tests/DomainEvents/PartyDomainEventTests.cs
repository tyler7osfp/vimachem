using FluentAssertions;
using Library.Domain;
using Library.Domain.Events;
using Library.Tests.Builders;

namespace Library.Tests.DomainEvents;

public class PartyDomainEventTests
{
    [Fact]
    public void Create_RaisesPartyCreatedEvent()
    {
        var party = new PartyBuilder().Build();
        party.DomainEvents.Should().ContainSingle(e => e is PartyCreatedEvent);
    }

    [Fact]
    public void AddRole_RaisesPartyRoleAddedEvent()
    {
        var party = new PartyBuilder().WithRole(RoleType.Customer).Build();
        party.PopDomainEvents();

        party.AddRole(RoleType.Author);

        party.DomainEvents.Should().ContainSingle(e => e is PartyRoleAddedEvent);
    }

    [Fact]
    public void RemoveRole_WithMultipleRoles_RaisesPartyRoleRemovedEvent()
    {
        var party = new PartyBuilder().WithRole(RoleType.Customer).Build();
        party.AddRole(RoleType.Author);
        party.PopDomainEvents();

        party.RemoveRole(RoleType.Author);

        party.DomainEvents.Should().ContainSingle(e => e is PartyRoleRemovedEvent);
    }
}
