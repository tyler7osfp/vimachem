using FluentAssertions;
using Library.Domain;
using Library.Domain.Entities;

namespace Books.Tests
{
    public class BookTests
    {
        private static Party AnAuthor()
            => Party.Create("Author", "author@test.com", RoleType.Author);

        private static Book CreateBook(int copies = 2)
            => Book.Create("Test Book", AnAuthor(), Guid.NewGuid(), copies);

        private static Party ACustomer()
            => Party.Create("Customer", "customer@test.com", RoleType.Customer);

        [Fact]
        public void Create_Sets_TotalCopies_And_AllAvailable()
        {
            var book = CreateBook(3);
            book.TotalCopies.Should().Be(3);
            book.AvailableCopies.Should().Be(3);
        }

        [Fact]
        public void Create_ZeroCopies_Throws_DomainException()
        {
            var act = () => CreateBook(0);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Create_EmptyTitle_Throws_DomainException()
        {
            var act = () => Book.Create("", AnAuthor(), Guid.NewGuid(), 1);
            act.Should().Throw<DomainException>().WithMessage("*Title is required*");
        }

        [Fact]
        public void Update_EmptyTitle_Throws_DomainException()
        {
            var book = CreateBook();
            var act = () => book.Update("", Guid.NewGuid());
            act.Should().Throw<DomainException>().WithMessage("*Title is required*");
        }

        [Fact]
        public void BorrowCopy_Returns_BookCopy_And_DecreasesAvailability()
        {
            var book = CreateBook(2);
            var copy = book.BorrowCopy(ACustomer());
            copy.Should().NotBeNull();
            copy.IsAvailable.Should().BeFalse();
            book.AvailableCopies.Should().Be(1);
        }

        [Fact]
        public void BorrowCopy_PartyWithoutCustomerRole_Throws_DomainException()
        {
            var book = CreateBook(2);
            var authorOnly = Party.Create("Author", "author@test.com", RoleType.Author);
            var act = () => book.BorrowCopy(authorOnly);
            act.Should().Throw<DomainException>().WithMessage("*not a Customer*");
        }

        [Fact]
        public void BorrowCopy_WhenNoneAvailable_Throws_DomainException()
        {
            var book = CreateBook(1);
            book.BorrowCopy(ACustomer());
            var act = () => book.BorrowCopy(ACustomer());
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void ReturnCopy_MarksSpecificCopyAvailable()
        {
            var book = CreateBook(2);
            var copy = book.BorrowCopy(ACustomer());
            book.ReturnCopy(copy.Id);
            book.AvailableCopies.Should().Be(2);
            copy.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public void ReturnCopy_WhenNotBorrowed_Throws_DomainException()
        {
            var book = CreateBook(1);
            var copyId = book.Copies.First().Id;
            var act = () => book.ReturnCopy(copyId);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void ReturnCopy_UnknownId_Throws_DomainException()
        {
            var book = CreateBook(2);
            var act = () => book.ReturnCopy(Guid.NewGuid());
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void IsAvailable_WhenCopiesRemain_ReturnsTrue()
        {
            var book = CreateBook(2);
            book.BorrowCopy(ACustomer());
            book.IsAvailable().Should().BeTrue();
        }

        [Fact]
        public void IsAvailable_WhenAllBorrowed_ReturnsFalse()
        {
            var book = CreateBook(1);
            book.BorrowCopy(ACustomer());
            book.IsAvailable().Should().BeFalse();
        }

        [Fact]
        public void AddCopy_IncreasesTotalCopies()
        {
            var book = CreateBook(2);
            book.AddCopy();
            book.TotalCopies.Should().Be(3);
            book.AvailableCopies.Should().Be(3);
        }

        [Fact]
        public void AddCopies_AddsMultipleCopies()
        {
            var book = CreateBook(1);
            var added = book.AddCopies(3);
            added.Should().HaveCount(3);
            book.TotalCopies.Should().Be(4);
            book.AvailableCopies.Should().Be(4);
        }

        [Fact]
        public void AddCopies_AllReturnedCopiesAreAvailable()
        {
            var book = CreateBook(1);
            var added = book.AddCopies(2);
            added.Should().AllSatisfy(c => c.IsAvailable.Should().BeTrue());
        }

        [Fact]
        public void AddCopies_ZeroCount_Throws_DomainException()
        {
            var book = CreateBook(1);
            var act = () => book.AddCopies(0);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void AddCopies_NegativeCount_Throws_DomainException()
        {
            var book = CreateBook(1);
            var act = () => book.AddCopies(-1);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void RemoveCopy_SoftDeletesAndReducesTotalCopies()
        {
            var book = CreateBook(2);
            var copyId = book.Copies.First().Id;
            book.RemoveCopy(copyId);
            book.TotalCopies.Should().Be(1);
            book.Copies.First(c => c.Id == copyId).IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void RemoveCopy_LastActiveCopy_Throws_DomainException()
        {
            var book = CreateBook(1);
            var copyId = book.Copies.First().Id;
            var act = () => book.RemoveCopy(copyId);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void RemoveCopy_BorrowedCopy_Throws_DomainException()
        {
            var book = CreateBook(2);
            var copy = book.BorrowCopy(ACustomer());
            var act = () => book.RemoveCopy(copy.Id);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void BorrowCopy_SkipsDeletedCopies()
        {
            var book = CreateBook(2);
            var firstCopyId = book.Copies.First().Id;
            book.RemoveCopy(firstCopyId);
            var borrowed = book.BorrowCopy(ACustomer());
            borrowed.Id.Should().NotBe(firstCopyId);
        }

        [Fact]
        public void TotalCopies_ExcludesDeletedCopies()
        {
            var book = CreateBook(3);
            book.RemoveCopy(book.Copies.First().Id);
            book.TotalCopies.Should().Be(2);
        }

        [Fact]
        public void AvailableCopies_ExcludesDeletedAndBorrowed()
        {
            var book = CreateBook(3);
            book.BorrowCopy(ACustomer());
            book.RemoveCopy(book.Copies.First(c => !c.IsDeleted && c.IsAvailable).Id);
            book.AvailableCopies.Should().Be(1);
        }
    }
}
