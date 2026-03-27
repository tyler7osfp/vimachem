using FluentAssertions;
using Library.Application.Services;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Tests.Builders;
using NSubstitute;

namespace Library.Tests;

public class BookQueryServiceTests
{
    private static BookQueryService BuildService(IBookRepository? repo = null)
        => new(repo ?? Substitute.For<IBookRepository>());

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        var book = new BookBuilder().WithCopies(2).Build();
        var repo = Substitute.For<IBookRepository>();
        repo.GetAllAsync(Arg.Any<CancellationToken>()).Returns([book]);

        var result = await BuildService(repo).GetAllAsync();

        result.Should().ContainSingle(d => d.Id == book.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNull()
    {
        var repo = Substitute.For<IBookRepository>();
        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Book?)null);

        var result = await BuildService(repo).GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Found_ReturnsMappedDto()
    {
        var book = new BookBuilder().WithCopies(1).Build();
        var repo = Substitute.For<IBookRepository>();
        repo.GetByIdAsync(book.Id, Arg.Any<CancellationToken>()).Returns(book);

        var result = await BuildService(repo).GetByIdAsync(book.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(book.Id);
        result.TotalCopies.Should().Be(1);
    }

    [Fact]
    public async Task GetAvailabilityByIdAsync_NotFound_ReturnsNull()
    {
        var repo = Substitute.For<IBookRepository>();
        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Book?)null);

        var result = await BuildService(repo).GetAvailabilityByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAvailabilityByIdAsync_Found_ReflectsCorrectAvailability()
    {
        var book = new BookBuilder().WithCopies(2).Build();
        var customer = Party.Create("Customer", "customer@test.com", RoleType.Customer);
        book.BorrowCopy(customer);

        var repo = Substitute.For<IBookRepository>();
        repo.GetByIdAsync(book.Id, Arg.Any<CancellationToken>()).Returns(book);

        var result = await BuildService(repo).GetAvailabilityByIdAsync(book.Id);

        result.Should().NotBeNull();
        result!.TotalCopies.Should().Be(2);
        result.AvailableCopies.Should().Be(1);
        result.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task GetAvailabilityByTitleAsync_NotFound_ReturnsNull()
    {
        var repo = Substitute.For<IBookRepository>();
        repo.GetByTitleAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Book?)null);

        var result = await BuildService(repo).GetAvailabilityByTitleAsync("Unknown");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_EmptyRepo_ReturnsEmptyCollection()
    {
        var repo = Substitute.For<IBookRepository>();
        repo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Enumerable.Empty<Book>());

        var result = await BuildService(repo).GetAllAsync();

        result.Should().BeEmpty();
    }
}
