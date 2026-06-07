using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Dtos.ApplicationType;
using DVLD.Contracts.Dtos.LicenseClass;
using DVLD.Contracts.Dtos.LocalLicenseApp;
using DVLD.Contracts.Requests.LocalLicenseApp;
using DVLD.Contracts.Requests.Application;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class LocalLicenseAppServiceTests
    {
        private readonly ILocalLicenseAppRepository _localRepo;
        private readonly IPersonRepository _peopleRepo;
        private readonly ILicenseClassRepository _licenseClassRepo;
        private readonly IApplicationTypeRepository _appTypeRepo;
        private readonly IApplicationRepository _appRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly LocalLicenseAppService _sut;

        public LocalLicenseAppServiceTests()
        {
            _localRepo = Substitute.For<ILocalLicenseAppRepository>();
            _peopleRepo = Substitute.For<IPersonRepository>();
            _licenseClassRepo = Substitute.For<ILicenseClassRepository>();
            _appTypeRepo = Substitute.For<IApplicationTypeRepository>();
            _appRepo = Substitute.For<IApplicationRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _sut = new LocalLicenseAppService(_localRepo, _peopleRepo, _licenseClassRepo, _appTypeRepo, _appRepo, _currentUser);
        }

        private static LocalLicenseAppDto CreateSampleApp(int id = 1) => new(id, 1, 1, 1, DateTime.UtcNow, 1, 1, DateTime.UtcNow, 100, 1);

        private static ApplicationTypeDto CreateSampleAppType(int id = 1) => new(id, "New Driving License", 200);

        private static LicenseClassDto CreateSampleLicenseClass(int id = 1) => new(id, "Ordinary", "Standard", 18, 10, 100);

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<LocalLicenseViewDto> { new(1, "Ordinary", "N1", "John Doe", DateTime.UtcNow, 3, "Completed") };
            _localRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByLocalLicenseAppIdAsync_WithValidId_ReturnsApp()
        {
            var expected = CreateSampleApp();
            _localRepo.GetByLocalLicenseAppIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByLocalLicenseAppIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByLocalLicenseAppIdAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetByLocalLicenseAppIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetByApplicationIdAsync_WithValidId_ReturnsApp()
        {
            var expected = CreateSampleApp();
            _localRepo.GetByApplicationIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByApplicationIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByApplicationIdAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetByApplicationIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreated()
        {
            var request = new CreateLocalLicenseAppRequest { ApplicantPersonId = 1, LicenseClassId = 1 };
            var appType = CreateSampleAppType();
            var licenseClass = CreateSampleLicenseClass();
            var createdApp = new ApplicationDto(1, 1, DateTime.UtcNow, 1, 1, DateTime.UtcNow, 300, 1);
            var createdLocal = CreateSampleApp();

            _currentUser.UserId.Returns(1);
            _peopleRepo.ExistsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _appTypeRepo.GetByIdAsync((int)enApplicationType.NewDrivingLicense, Arg.Any<CancellationToken>()).Returns(appType);
            _licenseClassRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(licenseClass);
            _appRepo.AddAsync(Arg.Any<CreateApplicationRequest>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(createdApp);
            _localRepo.CreateAsync(Arg.Any<CreateLocalLicenseAppDto>(), Arg.Any<CancellationToken>()).Returns(createdLocal);

            var result = await _sut.CreateAsync(request, CancellationToken.None);

            result.Should().Be(createdLocal);
        }

        [Fact]
        public async Task CreateAsync_WithNullBody_ThrowsArgumentNullException()
        {
            var act = () => _sut.CreateAsync(null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidPersonId_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateLocalLicenseAppRequest { ApplicantPersonId = 0, LicenseClassId = 1 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidLicenseClassId_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateLocalLicenseAppRequest { ApplicantPersonId = 1, LicenseClassId = 0 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WhenPersonNotExists_ThrowsArgumentException()
        {
            var request = new CreateLocalLicenseAppRequest { ApplicantPersonId = 99, LicenseClassId = 1 };
            _peopleRepo.ExistsByIdAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*does not exist*");
        }

        [Fact]
        public async Task CreateAsync_WhenAppTypeNotFound_ThrowsInvalidOperationException()
        {
            var request = new CreateLocalLicenseAppRequest { ApplicantPersonId = 1, LicenseClassId = 1 };
            _peopleRepo.ExistsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _appTypeRepo.GetByIdAsync((int)enApplicationType.NewDrivingLicense, Arg.Any<CancellationToken>()).ReturnsNull();

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Application type not found*");
        }

        [Fact]
        public async Task CreateAsync_WhenLicenseClassNotFound_ThrowsArgumentException()
        {
            var request = new CreateLocalLicenseAppRequest { ApplicantPersonId = 1, LicenseClassId = 99 };
            var appType = CreateSampleAppType();
            _peopleRepo.ExistsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _appTypeRepo.GetByIdAsync((int)enApplicationType.NewDrivingLicense, Arg.Any<CancellationToken>()).Returns(appType);
            _licenseClassRepo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*does not exist*");
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
        {
            _localRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.DeleteAsync(1, CancellationToken.None);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.DeleteAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            _localRepo.ExistsAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.DeleteAsync(99, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task ExistsAsync_WhenExists_ReturnsTrue()
        {
            _localRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.ExistsAsync(1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.ExistsAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task HasPassedTestAsync_WithValidIds_ReturnsResult()
        {
            _localRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _localRepo.HasPassedTestAsync(1, 1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.HasPassedTestAsync(1, 1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasPassedTestAsync_WithInvalidAppId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.HasPassedTestAsync(0, 1, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task HasPassedTestAsync_WithInvalidTestTypeId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.HasPassedTestAsync(1, 99, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task HasAttendedTestAsync_WithValidIds_ReturnsResult()
        {
            _localRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _localRepo.HasAttendedTestAsync(1, 1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.HasAttendedTestAsync(1, 1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasActiveTestAppointmentAsync_WithValidIds_ReturnsResult()
        {
            _localRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _localRepo.HasActiveTestAppointmentAsync(1, 1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.HasActiveTestAppointmentAsync(1, 1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetTestAttemptCountAsync_WithValidIds_ReturnsCount()
        {
            _localRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _localRepo.GetTestAttemptCountAsync(1, 1, Arg.Any<CancellationToken>()).Returns((byte)3);

            var result = await _sut.GetTestAttemptCountAsync(1, 1, CancellationToken.None);

            result.Should().Be(3);
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsUpdated()
        {
            var request = new UpdateLocalLicenseAppRequest { ApplicationStatus = (byte)enApplicationStatus.Completed };
            var existing = CreateSampleApp();
            _localRepo.GetByLocalLicenseAppIdAsync(1, Arg.Any<CancellationToken>()).Returns(existing);
            _localRepo.UpdateAsync(1, Arg.Any<LocalLicenseAppDto>(), Arg.Any<CancellationToken>()).Returns(existing);

            var result = await _sut.UpdateAsync(1, request, CancellationToken.None);

            result.Should().Be(existing);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var request = new UpdateLocalLicenseAppRequest();

            var act = () => _sut.UpdateAsync(0, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateAsync_WithNullBody_ThrowsArgumentNullException()
        {
            var act = () => _sut.UpdateAsync(1, null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidApplicationStatus_ThrowsArgumentOutOfRangeException()
        {
            var request = new UpdateLocalLicenseAppRequest { ApplicationStatus = 99 };

            var act = () => _sut.UpdateAsync(1, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            var request = new UpdateLocalLicenseAppRequest { ApplicationStatus = (byte)enApplicationStatus.New };
            _localRepo.GetByLocalLicenseAppIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var act = () => _sut.UpdateAsync(99, request, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}
