using FluentAssertions;
using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Tests
{
    public class BorrowingTests
    {
        [Fact]
        public void Create_SetsBorrowedAtToUtcNow()
        {
            var borrowing = Borrowing.Start(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            borrowing.BorrowedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            borrowing.IsActive().Should().BeTrue();
        }

        [Fact]
        public void Return_SetsReturnedAt()
        {
            var borrowing = Borrowing.Start(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            borrowing.CompleteReturn();
            borrowing.ReturnedAt.Should().NotBeNull();
            borrowing.IsActive().Should().BeFalse();
        }

        [Fact]
        public void Return_WhenAlreadyReturned_ThrowsInvalidOperationException()
        {
            var borrowing = Borrowing.Start(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            borrowing.CompleteReturn();
            var act = () => borrowing.CompleteReturn();
            act.Should().Throw<DomainException>();
        }
    }
}
