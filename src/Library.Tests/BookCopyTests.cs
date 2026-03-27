using FluentAssertions;
using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Tests
{
    public class BookCopyTests
    {
        private static BookCopy CreateCopy()
            => BookCopy.Create(Guid.NewGuid());

        [Fact]
        public void Create_SetsIsAvailableTrue()
        {
            var copy = CreateCopy();
            copy.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public void Borrow_SetsIsAvailableFalse()
        {
            var copy = CreateCopy();
            copy.Borrow();
            copy.IsAvailable.Should().BeFalse();
        }

        [Fact]
        public void Borrow_WhenAlreadyBorrowed_Throws_DomainException()
        {
            var copy = CreateCopy();
            copy.Borrow();
            var act = () => copy.Borrow();
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Return_SetsIsAvailableTrue()
        {
            var copy = CreateCopy();
            copy.Borrow();
            copy.Return();
            copy.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public void Return_WhenNotBorrowed_Throws_DomainException()
        {
            var copy = CreateCopy();
            var act = () => copy.Return();
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void SoftDelete_SetsIsDeletedTrue()
        {
            var copy = CreateCopy();
            copy.SoftDelete();
            copy.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void SoftDelete_WhenBorrowed_Throws_DomainException()
        {
            var copy = CreateCopy();
            copy.Borrow();
            var act = () => copy.SoftDelete();
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Borrow_WhenDeleted_Throws_DomainException()
        {
            var copy = CreateCopy();
            copy.SoftDelete();
            var act = () => copy.Borrow();
            act.Should().Throw<DomainException>();
        }
    }
}
