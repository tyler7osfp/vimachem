using FluentAssertions;
using Library.Application.Requests;
using Library.Api.Validators;

namespace Library.Tests.Validators;

public class PartyValidatorTests
{
    private readonly CreatePartyRequestValidator _validator = new();

    [Fact]
    public void Valid_Request_Passes()
    {
        var req = new CreatePartyRequest("John", "john@example.com", "Customer");
        _validator.Validate(req).IsValid.Should().BeTrue();
    }

    [Fact]
    public void InvalidEmail_Fails()
    {
        var req = new CreatePartyRequest("John", "not-an-email", "Customer");
        var result = _validator.Validate(req);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePartyRequest.Email));
    }

    [Fact]
    public void EmptyName_Fails()
    {
        var req = new CreatePartyRequest("", "john@example.com", "Customer");
        var result = _validator.Validate(req);
        result.IsValid.Should().BeFalse();
    }
}
