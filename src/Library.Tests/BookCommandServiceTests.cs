using FluentAssertions;
using Library.Application.Services;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Tests.Builders;
using NSubstitute;

namespace Library.Tests;

public class BookCommandServiceTests
{
    private static BookCommandService BuildService(
        IBookRepository? books = null,
        IPartyRepository? parties = null,
        ICategoryRepository? categories = null,
        IUnitOfWork? uow = null)
        => new(
            books      ?? Substitute.For<IBookRepository>(),
            parties    ?? Substitute.For<IPartyRepository>(),
            categories ?? Substitute.For<ICategoryRepository>(),
            uow        ?? Substitute.For<IUnitOfWork>());

    [Fact]
    public async Task CreateAsync_AuthorPartyNotFound_Throws_DomainException()
    {
        var parties = Substitute.For<IPartyRepository>();
        parties.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Party?)null);

        var sut = BuildService(parties: parties);
        var act = () => sut.CreateAsync("Title", Guid.NewGuid(), Guid.NewGuid(), 1);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Author party not found*");
    }

    [Fact]
    public async Task CreateAsync_PartyLacksAuthorRole_Throws_DomainException()
    {
        var author = new PartyBuilder().WithRole(RoleType.Customer).Build();
        var parties = Substitute.For<IPartyRepository>();
        parties.GetByIdAsync(author.Id, Arg.Any<CancellationToken>()).Returns(author);

        var category = new CategoryBuilder().Build();
        var categories = Substitute.For<ICategoryRepository>();
        categories.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        var sut = BuildService(parties: parties, categories: categories);
        var act = () => sut.CreateAsync("Title", category.Id, author.Id, 1);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Author role*");
    }

    [Fact]
    public async Task CreateAsync_CategoryNotFound_Throws_DomainException()
    {
        var author = new PartyBuilder().WithRole(RoleType.Author).Build();
        var parties = Substitute.For<IPartyRepository>();
        parties.GetByIdAsync(author.Id, Arg.Any<CancellationToken>()).Returns(author);

        var cats = Substitute.For<ICategoryRepository>();
        cats.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Category?)null);

        var sut = BuildService(parties: parties, categories: cats);
        var act = () => sut.CreateAsync("Title", Guid.NewGuid(), author.Id, 1);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Category not found*");
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        var books = Substitute.For<IBookRepository>();
        books.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Book?)null);

        var result = await BuildService(books: books).DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_Found_SetsIsDeletedAndReturnsTrue()
    {
        var book = new BookBuilder().WithCopies(1).Build();
        var books = Substitute.For<IBookRepository>();
        books.GetByIdAsync(book.Id, Arg.Any<CancellationToken>()).Returns(book);

        var result = await BuildService(books: books).DeleteAsync(book.Id);

        result.Should().BeTrue();
        book.IsDeleted.Should().BeTrue();
        books.Received(1).Delete(book);
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ReturnsNull()
    {
        var books = Substitute.For<IBookRepository>();
        books.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Book?)null);

        var result = await BuildService(books: books).UpdateAsync(Guid.NewGuid(), "Title", Guid.NewGuid());

        result.Should().BeNull();
    }
}
