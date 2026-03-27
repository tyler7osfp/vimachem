using EventService.Models;
using FluentValidation;

namespace EventService.Validators;

public class PagedListQueryValidator : AbstractValidator<PagedListQuery>
{
    public PagedListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
