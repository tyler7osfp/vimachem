namespace Library.Domain.Entities;

public record PartyRole
{
    public Guid PartyId { get; init; }
    public RoleType Role { get; init; }

    public static PartyRole Create(Guid partyId, RoleType role)
        => new() { PartyId = partyId, Role = role };
}
