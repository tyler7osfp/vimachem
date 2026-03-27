using FluentValidation;
using FluentValidation.Validators;
using Library.Application.Requests;

namespace Library.Api.Validators;

internal static class PartyRules
{
    internal static IRuleBuilderOptions<T, string> NameRules<T>(this IRuleBuilder<T, string> r)
        => r.NotEmpty().MaximumLength(150);

    internal static IRuleBuilderOptions<T, string> EmailRules<T>(this IRuleBuilder<T, string> r)
        => r.NotEmpty().EmailAddress().MaximumLength(200);
}

public class CreatePartyRequestValidator : AbstractValidator<CreatePartyRequest>
{
    public CreatePartyRequestValidator()
    {
        RuleFor(x => x.Name).NameRules();
        RuleFor(x => x.Email).EmailRules();
    }
}

public class UpdatePartyRequestValidator : AbstractValidator<UpdatePartyRequest>
{
    public UpdatePartyRequestValidator()
    {
        RuleFor(x => x.Name).NameRules();
        RuleFor(x => x.Email).EmailRules();
    }
}
