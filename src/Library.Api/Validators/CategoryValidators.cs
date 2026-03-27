using FluentValidation;
using Library.Application.Requests;

namespace Library.Api.Validators;

internal static class CategoryRules
{
    internal static IRuleBuilderOptions<T, string> CategoryNameRules<T>(this IRuleBuilder<T, string> r)
        => r.NotEmpty().MaximumLength(100);
}

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).CategoryNameRules();
    }
}

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).CategoryNameRules();
    }
}
