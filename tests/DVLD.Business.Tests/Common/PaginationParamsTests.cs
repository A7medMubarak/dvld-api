using DVLD.Contracts.Common;
using FluentAssertions;

namespace DVLD.Business.Tests.Common
{
    public class PaginationParamsTests
    {
        [Fact]
        public void Defaults_AreCorrect()
        {
            var paging = new PaginationParams();

            paging.PageNumber.Should().Be(1);
            paging.PageSize.Should().Be(10);
        }

        [Fact]
        public void PageSize_ClampedToMax()
        {
            var paging = new PaginationParams { PageSize = 100 };

            paging.PageSize.Should().Be(50);
        }

        [Fact]
        public void PageSize_WithinRange_Stays()
        {
            var paging = new PaginationParams { PageSize = 20 };

            paging.PageSize.Should().Be(20);
        }

        [Fact]
        public void PageNumber_IsPreserved()
        {
            var paging = new PaginationParams { PageNumber = 5 };

            paging.PageNumber.Should().Be(5);
        }
    }
}
