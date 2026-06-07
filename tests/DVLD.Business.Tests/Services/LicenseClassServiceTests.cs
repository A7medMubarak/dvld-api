using DVLD.Contracts.Dtos.LicenseClass;
using DVLD.Contracts.Requests.LicenseClass;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class LicenseClassServiceTests
    {
        private readonly ILicenseClassRepository _repo;
        private readonly LicenseClassService _sut;

        public LicenseClassServiceTests()
        {
            _repo = Substitute.For<ILicenseClassRepository>();
            _sut = new LicenseClassService(_repo);
        }

        private static LicenseClassDto CreateSampleLicenseClass(int id = 1) => new(id, "Ordinary", "Standard license", 18, 10, 100);

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<LicenseClassDto> { CreateSampleLicenseClass(), CreateSampleLicenseClass(2) };
            _repo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsLicenseClass()
        {
            var expected = CreateSampleLicenseClass();
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
        public async Task GetByClassNameAsync_WithValidName_ReturnsLicenseClass()
        {
            var expected = CreateSampleLicenseClass();
            _repo.GetByClassNameAsync("Ordinary", Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByClassNameAsync("Ordinary", CancellationToken.None);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task GetByClassNameAsync_WithInvalidInput_ThrowsArgumentException(string? className)
        {
            var act = () => _sut.GetByClassNameAsync(className!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Class name is reuired*");
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreated()
        {
            var request = new CreateLicenseClassRequest { ClassName = "New", ClassDescription = "Desc", MinimumAllowedAge = 18, DefaultValidityLength = 10, ClassFees = 100 };
            var created = CreateSampleLicenseClass();
            _repo.CreateAsync(Arg.Any<LicenseClassDto>(), Arg.Any<CancellationToken>()).Returns(created);

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
        public async Task CreateAsync_WithMissingClassName_ThrowsArgumentException()
        {
            var request = new CreateLicenseClassRequest { ClassName = "", ClassDescription = "Desc", MinimumAllowedAge = 18, DefaultValidityLength = 10, ClassFees = 100 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Class name is required*");
        }

        [Fact]
        public async Task CreateAsync_WithMissingDescription_ThrowsArgumentException()
        {
            var request = new CreateLicenseClassRequest { ClassName = "Valid", ClassDescription = "", MinimumAllowedAge = 18, DefaultValidityLength = 10, ClassFees = 100 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Class description is required*");
        }

        [Fact]
        public async Task CreateAsync_WithAgeBelow18_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateLicenseClassRequest { ClassName = "Valid", ClassDescription = "Desc", MinimumAllowedAge = 16, DefaultValidityLength = 10, ClassFees = 100 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*Minimum allowed age must be at least 18*");
        }

        [Fact]
        public async Task CreateAsync_WithInvalidFees_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateLicenseClassRequest { ClassName = "Valid", ClassDescription = "Desc", MinimumAllowedAge = 18, DefaultValidityLength = 10, ClassFees = 0 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidValidityLength_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateLicenseClassRequest { ClassName = "Valid", ClassDescription = "Desc", MinimumAllowedAge = 18, DefaultValidityLength = 0, ClassFees = 100 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsUpdated()
        {
            var request = new UpdateLicenseClassRequest { ClassName = "Updated", ClassDescription = "Desc", MinimumAllowedAge = 18, DefaultValidityLength = 10, ClassFees = 150 };
            var updated = new LicenseClassDto(1, "Updated", "Desc", 18, 10, 150);
            _repo.UpdateAsync(1, Arg.Any<LicenseClassDto>(), Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _sut.UpdateAsync(1, request, CancellationToken.None);

            result.Should().Be(updated);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var request = new UpdateLicenseClassRequest();

            var act = () => _sut.UpdateAsync(0, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
    }
}
