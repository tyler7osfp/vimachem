using FluentAssertions;
using Library.Application.Services;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Tests.Builders;
using NSubstitute;

namespace Library.Tests
{
    public class PartyServiceTests
    {
        private static PartyCommandService BuildCommandService(
            IPartyRepository? partyRepo = null,
            IBookRepository? bookRepo = null,
            IBorrowingRepository? borrowingRepo = null)
            => new(
                partyRepo ?? Substitute.For<IPartyRepository>(),
                bookRepo ?? Substitute.For<IBookRepository>(),
                borrowingRepo ?? Substitute.For<IBorrowingRepository>(),
                Substitute.For<IUnitOfWork>());

        private static PartyRoleCommandService BuildRoleService(
            IPartyRepository? partyRepo = null,
            IBookRepository? bookRepo = null,
            IBorrowingRepository? borrowingRepo = null)
            => new(
                partyRepo ?? Substitute.For<IPartyRepository>(),
                bookRepo ?? Substitute.For<IBookRepository>(),
                borrowingRepo ?? Substitute.For<IBorrowingRepository>(),
                Substitute.For<IUnitOfWork>());

        [Fact]
        public async Task DeleteAsync_PartyWithBooks_Throws_DomainException()
        {
            var party = new PartyBuilder().WithName("Homer").WithEmail("homer@library.gr").WithRole(RoleType.Author).Build();

            var partyRepo = Substitute.For<IPartyRepository>();
            partyRepo.GetByIdAsync(party.Id, Arg.Any<CancellationToken>()).Returns(party);

            var bookRepo = Substitute.For<IBookRepository>();
            bookRepo.HasBooksAsAuthorAsync(party.Id, Arg.Any<CancellationToken>()).Returns(true);

            var svc = BuildCommandService(partyRepo: partyRepo, bookRepo: bookRepo);
            Func<Task> act = () => svc.DeleteAsync(party.Id, default);
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task DeleteAsync_PartyWithActiveBorrowings_Throws_DomainException()
        {
            var party = new PartyBuilder().WithName("Nikos").WithEmail("nikos@library.gr").WithRole(RoleType.Customer).Build();

            var partyRepo = Substitute.For<IPartyRepository>();
            partyRepo.GetByIdAsync(party.Id, Arg.Any<CancellationToken>()).Returns(party);

            var bookRepo = Substitute.For<IBookRepository>();
            bookRepo.HasBooksAsAuthorAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

            var borrowingRepo = Substitute.For<IBorrowingRepository>();
            borrowingRepo.HasActiveBorrowingsForCustomerAsync(party.Id, Arg.Any<CancellationToken>()).Returns(true);

            var svc = BuildCommandService(partyRepo: partyRepo, bookRepo: bookRepo, borrowingRepo: borrowingRepo);
            Func<Task> act = () => svc.DeleteAsync(party.Id, default);
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task DeleteAsync_PartyNotFound_Returns_False()
        {
            var partyRepo = Substitute.For<IPartyRepository>();
            partyRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Party?)null);

            var svc = BuildCommandService(partyRepo: partyRepo);
            var result = await svc.DeleteAsync(Guid.NewGuid(), default);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveRoleAsync_AuthorWithBooks_Throws_DomainException()
        {
            var party = new PartyBuilder().WithName("Homer").WithEmail("homer@library.gr").WithRole(RoleType.Author).Build();
            party.AddRole(RoleType.Customer); // second role so last-role guard doesn't fire first

            var partyRepo = Substitute.For<IPartyRepository>();
            partyRepo.GetByIdAsync(party.Id, Arg.Any<CancellationToken>()).Returns(party);

            var bookRepo = Substitute.For<IBookRepository>();
            bookRepo.HasBooksAsAuthorAsync(party.Id, Arg.Any<CancellationToken>()).Returns(true);

            var svc = BuildRoleService(partyRepo: partyRepo, bookRepo: bookRepo);
            Func<Task> act = () => svc.RemoveRoleAsync(party.Id, nameof(RoleType.Author), default);
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task RemoveRoleAsync_CustomerWithActiveBorrowings_Throws_DomainException()
        {
            var party = new PartyBuilder().WithName("Nikos").WithEmail("nikos@library.gr").WithRole(RoleType.Customer).Build();
            party.AddRole(RoleType.Author); // second role

            var partyRepo = Substitute.For<IPartyRepository>();
            partyRepo.GetByIdAsync(party.Id, Arg.Any<CancellationToken>()).Returns(party);

            var bookRepo = Substitute.For<IBookRepository>();
            bookRepo.HasBooksAsAuthorAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

            var borrowingRepo = Substitute.For<IBorrowingRepository>();
            borrowingRepo.HasActiveBorrowingsForCustomerAsync(party.Id, Arg.Any<CancellationToken>()).Returns(true);

            var svc = BuildRoleService(partyRepo: partyRepo, bookRepo: bookRepo, borrowingRepo: borrowingRepo);
            Func<Task> act = () => svc.RemoveRoleAsync(party.Id, nameof(RoleType.Customer), default);
            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task RemoveRoleAsync_PartyNotFound_Returns_False()
        {
            var partyRepo = Substitute.For<IPartyRepository>();
            partyRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Party?)null);

            var svc = BuildRoleService(partyRepo: partyRepo);
            var result = await svc.RemoveRoleAsync(Guid.NewGuid(), nameof(RoleType.Author), default);
            result.Should().BeFalse();
        }
    }
}
