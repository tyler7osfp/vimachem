using Library.Domain.Events;

namespace Library.Domain.Entities
{
    public class Party : Entity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = null!;
        public bool IsDeleted { get; private set; }
        public ICollection<PartyRole> Roles { get; private set; } = new List<PartyRole>();

        private Party() { }

        public static Party Create(string name, string email, RoleType initialRole)
        {
            ValidateName(name);
        
            var party = new Party { Id = Guid.NewGuid(), Name = name, Email = email };
            party.Roles.Add(PartyRole.Create(party.Id, initialRole));
            party.RaiseDomainEvent(new PartyCreatedEvent(party.Id));
            return party;
        }

        public bool HasRole(RoleType role) => Roles.Any(r => r.Role == role);

        public void AddRole(RoleType role)
        {
            if (Roles.Any(r => r.Role == role))
                throw new DomainException($"Party already has the {role} role.");
            Roles.Add(PartyRole.Create(Id, role));
            RaiseDomainEvent(new PartyRoleAddedEvent(Id, role.ToString()));
        }

        public void RemoveRole(RoleType role)
        {
            var existing = Roles.FirstOrDefault(r => r.Role == role)
                ?? throw new DomainException($"Party does not have the {role} role.");

            if (Roles.Count <= 1)
                throw new DomainException("A party must have at least one role. Assign another role before removing this one.");

            Roles.Remove(existing);
            RaiseDomainEvent(new PartyRoleRemovedEvent(Id, role.ToString()));
        }

        public void Update(string name, string email)
        {
            ValidateName(name);
            Name = name;
            Email = email;
            RaiseDomainEvent(new PartyUpdatedEvent(Id));
        }

        public void MarkDeleted()
        {
            IsDeleted = true;
            RaiseDomainEvent(new PartyDeletedEvent(Id));
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required.");
        }
    }
}
