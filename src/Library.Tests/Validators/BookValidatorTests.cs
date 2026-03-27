using FluentAssertions;
using Library.Application.Requests;
using Library.Api.Validators;

namespace Library.Tests.Validators;

public class BookValidatorTests
{
    private readonly CreateBookRequestValidator _validator = new();

    [Fact]
    public void Valid_Request_Passes()
    {
        var req = new CreateBookRequest("Valid Title", Guid.NewGuid(), Guid.NewGuid(), 1);
        var result = _validator.Validate(req);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyTitle_Fails(string title)
    {
        var req = new CreateBookRequest(title, Guid.NewGuid(), Guid.NewGuid(), 1);
        var result = _validator.Validate(req);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateBookRequest.Title));
    }

    [Fact]
    public void ZeroCopies_Fails()
    {
        var req = new CreateBookRequest("Title", Guid.NewGuid(), Guid.NewGuid(), 0);
        var result = _validator.Validate(req);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateBookRequest.Copies));
    }

    [Fact]
    public void EmptyCategoryId_Fails()
    {
        var req = new CreateBookRequest("Title", Guid.Empty, Guid.NewGuid(), 1);
        var result = _validator.Validate(req);
        result.IsValid.Should().BeFalse();
    }
}
