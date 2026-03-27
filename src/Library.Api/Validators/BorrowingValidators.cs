using FluentValidation;
using Library.Application.Requests;

namespace Library.Api.Validators;

public class CreateBorrowingRequestValidator : AbstractValidator<CreateBorrowingRequest>
{
    public CreateBorrowingRequestValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
        RuleFor(x => x.CustomerPartyId).NotEmpty();
    }
}
