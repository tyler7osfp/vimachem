using Library.Domain;
using Library.Domain.Entities;

namespace Library.Tests.Builders;

public class PartyBuilder
{
    private string _name = "Test Party";
    private string _email = "test@example.com";
    private RoleType _role = RoleType.Customer;

    public PartyBuilder WithName(string name) { _name = name; return this; }
    public PartyBuilder WithEmail(string email) { _email = email; return this; }
    public PartyBuilder WithRole(RoleType role) { _role = role; return this; }

    public Party Build() => Party.Create(_name, _email, _role);
}
