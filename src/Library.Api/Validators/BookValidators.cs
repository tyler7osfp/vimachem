using FluentValidation;
using Library.Application.Requests;

namespace Library.Api.Validators;

internal static class BookRules
{
    internal static IRuleBuilderOptions<T, string> TitleRules<T>(this IRuleBuilder<T, string> r)
        => r.NotEmpty().MaximumLength(200);
}

public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        RuleFor(x => x.Title).TitleRules();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.AuthorPartyId).NotEmpty();
        RuleFor(x => x.Copies).GreaterThanOrEqualTo(1);
    }
}

public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        RuleFor(x => x.Title).TitleRules();
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
