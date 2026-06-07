using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Dtos.ApplicationType;
using DVLD.Contracts.Dtos.DetainedLicense;
using DVLD.Contracts.Dtos.License;
using DVLD.Contracts.Exceptions;
using DVLD.Contracts.Requests.DetainedLicense;
using DVLD.Contracts.Requests.Application;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class DetainedLicenseServiceTests
    {
        private readonly IDetainedLicenseRepository _detainedRepo;
        private readonly ILicenseRepository _licenseRepo;
        private readonly IApplicationRepository _appRepo;
        private readonly IApplicationTypeRepository _appTypeRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly DetainedLicenseService _sut;

        public DetainedLicenseServiceTests()
        {
            _detainedRepo = Substitute.For<IDetainedLicenseRepository>();
            _licenseRepo = Substitute.For<ILicenseRepository>();
            _appRepo = Substitute.For<IApplicationRepository>();
            _appTypeRepo = Substitute.For<IApplicationTypeRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _sut = new DetainedLicenseService(_detainedRepo, _licenseRepo, _appRepo, _appTypeRepo, _currentUser);
        }

        private static DetainedLicenseDto CreateSampleDetained(int id = 1) => new(id, 1, DateTime.UtcNow.AddDays(-5), 200, 1, false, null, null, null);

        private static DetainedLicenseViewDto CreateSampleView() => new(1, 1, DateTime.UtcNow.AddDays(-5), false, 200, null, "N123", "John Doe", null);

        private static LicenseDto CreateSampleLicense(int id = 1) => new(id, 1, 1, 1, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(335), null, 100, true, enIssueReason.FirstTime, 1);

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<DetainedLicenseViewDto> { CreateSampleView() };
            _detainedRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByDetainIdAsync_WithValidId_ReturnsDetained()
        {
            var expected = CreateSampleDetained();
            _detainedRepo.GetByDetainIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByDetainIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByDetainIdAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetByDetainIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetByDetainIdAsync_WhenNotFound_ReturnsNull()
        {
            _detainedRepo.GetByDetainIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetByDetainIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByLicenseIdAsync_WithValidId_ReturnsDetained()
        {
            var expected = CreateSampleDetained();
            _detainedRepo.GetByLicenseIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByLicenseIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByLicenseIdAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetByLicenseIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreated()
        {
            var request = new CreateDetainedLicenseRequest { LicenseId = 1, FineFees = 200 };
            var license = CreateSampleLicense();
            var created = CreateSampleDetained();

            _currentUser.UserId.Returns(1);
            _licenseRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(license);
            _detainedRepo.IsLicenseDetainedAsync(1, Arg.Any<CancellationToken>()).Returns(false);
            _detainedRepo.CreateAsync(Arg.Any<DetainedLicenseDto>(), Arg.Any<CancellationToken>()).Returns(created);

            var result = await _sut.CreateAsync(request, CancellationToken.None);

            result.Should().Be(created);
        }

        [Fact]
        public async Task CreateAsync_WithNullRequest_ThrowsArgumentNullException()
        {
            var act = () => _sut.CreateAsync(null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidLicenseId_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateDetainedLicenseRequest { LicenseId = 0, FineFees = 200 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidFees_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateDetainedLicenseRequest { LicenseId = 1, FineFees = 0 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WhenLicenseNotExists_ThrowsArgumentException()
        {
            var request = new CreateDetainedLicenseRequest { LicenseId = 99, FineFees = 200 };
            _licenseRepo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*does not exist*");
        }

        [Fact]
        public async Task CreateAsync_WhenAlreadyDetained_ThrowsResourceConflictException()
        {
            var request = new CreateDetainedLicenseRequest { LicenseId = 1, FineFees = 200 };
            var license = CreateSampleLicense();
            _licenseRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(license);
            _detainedRepo.IsLicenseDetainedAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ResourceConflictException>()
                .WithMessage("*already detained*");
        }

        [Fact]
        public async Task IsLicenseDetainedAsync_WhenDetained_ReturnsTrue()
        {
            _licenseRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _detainedRepo.IsLicenseDetainedAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.IsLicenseDetainedAsync(1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsLicenseDetainedAsync_WhenNotDetained_ReturnsFalse()
        {
            _licenseRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _detainedRepo.IsLicenseDetainedAsync(1, Arg.Any<CancellationToken>()).Returns(false);

            var result = await _sut.IsLicenseDetainedAsync(1, CancellationToken.None);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsLicenseDetainedAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.IsLicenseDetainedAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task IsLicenseDetainedAsync_WhenLicenseNotExists_ThrowsKeyNotFoundException()
        {
            _licenseRepo.ExistsAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.IsLicenseDetainedAsync(99, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*does not exist*");
        }

        [Fact]
        public async Task ReleaseDetainedLicenseAsync_WithValidId_ReleasesSuccessfully()
        {
            var detained = CreateSampleDetained();
            var license = CreateSampleLicense();
            var app = new ApplicationDto(1, 1, DateTime.UtcNow, 1, 1, DateTime.UtcNow, 100, 1);
            var appType = new ApplicationTypeDto((int)enApplicationType.ReleaseDetainedDrivingLicsense, "Release Detained", 100);
            var createdApp = new ApplicationDto(2, 1, DateTime.UtcNow, (int)enApplicationType.ReleaseDetainedDrivingLicsense, 1, DateTime.UtcNow, 100, 1);

            _currentUser.UserId.Returns(1);
            _detainedRepo.GetByDetainIdAsync(1, Arg.Any<CancellationToken>()).Returns(detained);
            _licenseRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(license);
            _appRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(app);
            _appTypeRepo.GetByIdAsync((int)enApplicationType.ReleaseDetainedDrivingLicsense, Arg.Any<CancellationToken>()).Returns(appType);
            _appRepo.AddAsync(Arg.Any<CreateApplicationRequest>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(createdApp);
            _detainedRepo.ReleaseDetainedLicenseAsync(1, 1, 2, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.ReleaseDetainedLicenseAsync(1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ReleaseDetainedLicenseAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.ReleaseDetainedLicenseAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task ReleaseDetainedLicenseAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            _detainedRepo.GetByDetainIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var act = () => _sut.ReleaseDetainedLicenseAsync(99, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*not found*");
        }

        [Fact]
        public async Task ReleaseDetainedLicenseAsync_WhenAlreadyReleased_ThrowsResourceConflictException()
        {
            var detained = new DetainedLicenseDto(1, 1, DateTime.UtcNow, 200, 1, true, DateTime.UtcNow, 1, 1);
            _detainedRepo.GetByDetainIdAsync(1, Arg.Any<CancellationToken>()).Returns(detained);

            var act = () => _sut.ReleaseDetainedLicenseAsync(1, CancellationToken.None);

            await act.Should().ThrowAsync<ResourceConflictException>()
                .WithMessage("*released already*");
        }
    }
}
