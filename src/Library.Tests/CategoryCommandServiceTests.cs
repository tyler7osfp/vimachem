using FluentAssertions;
using Library.Application.Services;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Tests.Builders;
using NSubstitute;

namespace Library.Tests;

public class CategoryCommandServiceTests
{
    private static CategoryCommandService BuildService(
        ICategoryRepository? repo = null,
        IUnitOfWork? uow = null)
        => new(
            repo ?? Substitute.For<ICategoryRepository>(),
            uow  ?? Substitute.For<IUnitOfWork>());

    [Fact]
    public async Task CreateAsync_ReturnsDto_WithCorrectName()
    {
        var repo = Substitute.For<ICategoryRepository>();
        var sut = BuildService(repo: repo);

        var result = await sut.CreateAsync("Fiction");

        result.Name.Should().Be("Fiction");
        await repo.Received(1).AddAsync(Arg.Is<Category>(c => c.Name == "Fiction"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ReturnsNull()
    {
        var repo = Substitute.For<ICategoryRepository>();
        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Category?)null);

        var result = await BuildService(repo: repo).UpdateAsync(Guid.NewGuid(), "New Name");

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_Found_ReturnsUpdatedDto()
    {
        var category = new CategoryBuilder().WithName("OldName").Build();
        var repo = Substitute.For<ICategoryRepository>();
        repo.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        var result = await BuildService(repo: repo).UpdateAsync(category.Id, "NewName");

        result.Should().NotBeNull();
        result!.Name.Should().Be("NewName");
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        var repo = Substitute.For<ICategoryRepository>();
        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Category?)null);

        var result = await BuildService(repo: repo).DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_Found_SetsIsDeletedAndReturnsTrue()
    {
        var category = new CategoryBuilder().Build();
        var repo = Substitute.For<ICategoryRepository>();
        repo.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        var result = await BuildService(repo: repo).DeleteAsync(category.Id);

        result.Should().BeTrue();
        category.IsDeleted.Should().BeTrue();
        repo.Received(1).Delete(category);
    }
}
