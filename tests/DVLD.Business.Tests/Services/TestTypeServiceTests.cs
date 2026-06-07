using DVLD.Contracts.Dtos.TestType;
using DVLD.Contracts.Requests.TestType;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class TestTypeServiceTests
    {
        private readonly ITestTypeRepository _repo;
        private readonly TestTypeService _sut;

        public TestTypeServiceTests()
        {
            _repo = Substitute.For<ITestTypeRepository>();
            _sut = new TestTypeService(_repo);
        }

        private static TestTypeDto CreateSampleTestType(int id = 1) => new(id, "Vision Test", "Checks vision", 50);

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<TestTypeDto> { CreateSampleTestType(), CreateSampleTestType(2) };
            _repo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsTestType()
        {
            var expected = CreateSampleTestType();
            _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetByIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            _repo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetByIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreated()
        {
            var request = new CreateTestTypeRequest { Title = "New Test", Description = "Desc", Fees = 100 };
            var created = new TestTypeDto(1, "New Test", "Desc", 100);
            _repo.CreateAsync(Arg.Any<TestTypeDto>(), Arg.Any<CancellationToken>()).Returns(created);

            var result = await _sut.CreateAsync(request, CancellationToken.None);

            result.Should().Be(created);
        }

        [Fact]
        public async Task CreateAsync_WithNullBody_ThrowsArgumentNullException()
        {
            var act = () => _sut.CreateAsync(null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateAsync_WithEmptyTitle_ThrowsArgumentException()
        {
            var request = new CreateTestTypeRequest { Title = "", Description = "Desc", Fees = 100 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateAsync_WithEmptyDescription_ThrowsArgumentException()
        {
            var request = new CreateTestTypeRequest { Title = "Title", Description = "", Fees = 100 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidFees_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateTestTypeRequest { Title = "Title", Description = "Desc", Fees = 0 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsUpdated()
        {
            var request = new UpdateTestTypeRequest { Title = "Updated", Description = "Desc", Fees = 150 };
            var updated = new TestTypeDto(1, "Updated", "Desc", 150);
            _repo.UpdateAsync(1, Arg.Any<TestTypeDto>(), Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _sut.UpdateAsync(1, request, CancellationToken.None);

            result.Should().Be(updated);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var request = new UpdateTestTypeRequest();

            var act = () => _sut.UpdateAsync(0, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
    }
}
