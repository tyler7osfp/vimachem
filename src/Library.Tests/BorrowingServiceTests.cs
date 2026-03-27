using FluentAssertions;
using Library.Application.Services;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Tests.Builders;
using NSubstitute;

namespace Library.Tests
{
    public class BorrowingServiceTests
    {
        [Fact]
        public async Task BorrowAsync_CustomerWithoutRole_Throws_DomainException()
        {
            var authorOnly = new PartyBuilder().WithRole(RoleType.Author).Build();

            var partyRepo = Substitute.For<IPartyRepository>();
            partyRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(authorOnly);

            var book = new BookBuilder().WithCopies(2).Build();
            var bookRepo = Substitute.For<IBookRepository>();
            bookRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(book);

            var svc = new BorrowingCommandService(
                Substitute.For<IBorrowingRepository>(),
                bookRepo,
                partyRepo,
                Substitute.For<IUnitOfWork>());

            var act = () => svc.BorrowAsync(book.Id, authorOnly.Id, default);
            await act.Should().ThrowAsync<DomainException>().WithMessage("*not a Customer*");
        }

        [Fact]
        public async Task BorrowAsync_BookNotFound_Throws_DomainException()
        {
            var customer = new PartyBuilder().WithRole(RoleType.Customer).Build();

            var partyRepo = Substitute.For<IPartyRepository>();
            partyRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(customer);

            var bookRepo = Substitute.For<IBookRepository>();
            bookRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Book?)null);

            var svc = new BorrowingCommandService(
                Substitute.For<IBorrowingRepository>(),
                bookRepo,
                partyRepo,
                Substitute.For<IUnitOfWork>());

            var act = () => svc.BorrowAsync(Guid.NewGuid(), customer.Id, default);
            await act.Should().ThrowAsync<DomainException>().WithMessage("*Book not found*");
        }

        [Fact]
        public async Task BorrowAsync_NoCopiesAvailable_Throws_DomainException()
        {
            var customer = new PartyBuilder().WithRole(RoleType.Customer).Build();
            var book = new BookBuilder().WithCopies(1).Build();
            book.BorrowCopy(customer); // exhaust the one copy

            var partyRepo = Substitute.For<IPartyRepository>();
            partyRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(customer);

            var bookRepo = Substitute.For<IBookRepository>();
            bookRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(book);

            var svc = new BorrowingCommandService(
                Substitute.For<IBorrowingRepository>(),
                bookRepo,
                partyRepo,
                Substitute.For<IUnitOfWork>());

            var act = () => svc.BorrowAsync(book.Id, customer.Id, default);
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task ReturnAsync_BorrowingNotFound_Returns_Null()
        {
            var borrowingRepo = Substitute.For<IBorrowingRepository>();
            borrowingRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                         .Returns((Borrowing?)null);

            var svc = new BorrowingCommandService(
                borrowingRepo,
                Substitute.For<IBookRepository>(),
                Substitute.For<IPartyRepository>(),
                Substitute.For<IUnitOfWork>());

            var result = await svc.ReturnAsync(Guid.NewGuid(), default);
            result.Should().BeNull();
        }
    }
}
