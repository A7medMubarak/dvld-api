using DVLD.Contracts.Dtos.ApplicationType;
using DVLD.Contracts.Requests.ApplicationType;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class ApplicationTypeServiceTests
    {
        private readonly IApplicationTypeRepository _repo;
        private readonly ApplicationTypeService _sut;

        public ApplicationTypeServiceTests()
        {
            _repo = Substitute.For<IApplicationTypeRepository>();
            _sut = new ApplicationTypeService(_repo);
        }

        private static ApplicationTypeDto CreateSampleAppType(int id = 1) => new(id, "New License", 200);

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<ApplicationTypeDto> { CreateSampleAppType(), CreateSampleAppType(2) };
            _repo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsAppType()
        {
            var expected = CreateSampleAppType();
            _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetByIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*Id must be greater than zero*");
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
            var request = new CreateApplicationTypeRequest { ApplicationTypeTitle = "New License", ApplicationTypeFees = 200 };
            var created = CreateSampleAppType();
            _repo.CreateAsync(Arg.Any<ApplicationTypeDto>(), Arg.Any<CancellationToken>()).Returns(created);

            var result = await _sut.CreateAsync(request, CancellationToken.None);

            result.Should().Be(created);
        }

        [Fact]
        public async Task CreateAsync_WithNullBody_ThrowsArgumentNullException()
        {
            var act = () => _sut.CreateAsync(null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("body cannot be null (Parameter 'body')");
        }

        [Fact]
        public async Task CreateAsync_WithEmptyTitle_ThrowsArgumentException()
        {
            var request = new CreateApplicationTypeRequest { ApplicationTypeTitle = "", ApplicationTypeFees = 200 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("ApplicationTypeTitle cannot be empty. (Parameter 'ApplicationTypeTitle')");
        }

        [Fact]
        public async Task CreateAsync_WithInvalidFees_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateApplicationTypeRequest { ApplicationTypeTitle = "Test", ApplicationTypeFees = 0 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("ApplicationTypeFees must be at least 5. (Parameter 'ApplicationTypeFees')");
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsUpdated()
        {
            var request = new UpdateApplicationTypeRequest { ApplicationTypeTitle = "Updated", ApplicationTypeFees = 300 };
            var updated = new ApplicationTypeDto(1, "Updated", 300);
            _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(CreateSampleAppType());
            _repo.UpdateAsync(1, Arg.Any<ApplicationTypeDto>(), Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _sut.UpdateAsync(1, request, CancellationToken.None);

            result.Should().Be(updated);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var request = new UpdateApplicationTypeRequest();

            var act = () => _sut.UpdateAsync(0, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*Id must be greater than zero*");
        }

        [Fact]
        public async Task UpdateAsync_WithNullBody_ThrowsArgumentNullException()
        {
            var act = () => _sut.UpdateAsync(1, null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("body cannot be null (Parameter 'body')");
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            var request = new UpdateApplicationTypeRequest { ApplicationTypeTitle = "Test", ApplicationTypeFees = 100 };
            _repo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var act = () => _sut.UpdateAsync(99, request, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*not found*");
        }
    }
}
