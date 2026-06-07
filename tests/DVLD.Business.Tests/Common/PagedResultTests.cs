using DVLD.Contracts.Common;
using FluentAssertions;

namespace DVLD.Business.Tests.Common
{
    public class PagedResultTests
    {
        [Fact]
        public void TotalPages_CalculatesCorrectly()
        {
            var result = new PagedResult<string>
            {
                Items = new[] { "a", "b", "c" },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 25
            };

            result.TotalPages.Should().Be(3);
        }

        [Fact]
        public void TotalPages_WithZeroItems_ReturnsZero()
        {
            var result = new PagedResult<string>
            {
                Items = Array.Empty<string>(),
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 0
            };

            result.TotalPages.Should().Be(0);
        }

        [Fact]
        public void TotalPages_ExactDivision_ReturnsQuotient()
        {
            var result = new PagedResult<string>
            {
                Items = new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 20
            };

            result.TotalPages.Should().Be(2);
        }

        [Fact]
        public void HasPreviousPage_WhenPage1_ReturnsFalse()
        {
            var result = new PagedResult<string>
            {
                Items = new[] { "a" },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1
            };

            result.HasPreviousPage.Should().BeFalse();
        }

        [Fact]
        public void HasPreviousPage_WhenPage2_ReturnsTrue()
        {
            var result = new PagedResult<string>
            {
                Items = new[] { "a" },
                PageNumber = 2,
                PageSize = 10,
                TotalCount = 25
            };

            result.HasPreviousPage.Should().BeTrue();
        }

        [Fact]
        public void HasNextPage_WhenLastPage_ReturnsFalse()
        {
            var result = new PagedResult<string>
            {
                Items = new[] { "a", "b", "c", "d", "e" },
                PageNumber = 3,
                PageSize = 10,
                TotalCount = 25
            };

            result.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public void HasNextPage_WhenNotLastPage_ReturnsTrue()
        {
            var result = new PagedResult<string>
            {
                Items = new[] { "a" },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 25
            };

            result.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public void Items_DefaultsToEmptyArray()
        {
            var result = new PagedResult<string>();

            result.Items.Should().NotBeNull();
            result.Items.Should().BeEmpty();
        }
    }
}
