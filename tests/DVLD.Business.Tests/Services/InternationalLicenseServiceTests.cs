using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Dtos.ApplicationType;
using DVLD.Contracts.Dtos.Driver;
using DVLD.Contracts.Dtos.InternationalLicense;
using DVLD.Contracts.Dtos.LocalLicenseApp;
using DVLD.Contracts.Requests.Application;
using DVLD.Contracts.Requests.InternationalLicense;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class InternationalLicenseServiceTests
    {
        private readonly ILocalLicenseAppRepository _localLicenseAppRepo;
        private readonly IInternationalLicenseRepository _internationalLicenseRepo;
        private readonly IDriverRepository _driverRepo;
        private readonly IApplicationTypeRepository _applicationTypeRepo;
        private readonly IApplicationRepository _applicationRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly InternationalLicenseService _sut;

        public InternationalLicenseServiceTests()
        {
            _localLicenseAppRepo = Substitute.For<ILocalLicenseAppRepository>();
            _internationalLicenseRepo = Substitute.For<IInternationalLicenseRepository>();
            _driverRepo = Substitute.For<IDriverRepository>();
            _applicationTypeRepo = Substitute.For<IApplicationTypeRepository>();
            _applicationRepo = Substitute.For<IApplicationRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _sut = new InternationalLicenseService(_localLicenseAppRepo, _internationalLicenseRepo,
                _driverRepo, _applicationTypeRepo, _applicationRepo, _currentUser);
        }

        private static InternationalLicenseDto CreateSampleLicense(int id = 1) => new(id, 1, 1, 1, DateTime.UtcNow, DateTime.UtcNow.AddYears(1), true, 1);

        private static DriverDto CreateSampleDriver(int id = 1) => new() { DriverId = id, PersonId = 1, CreatedByUserId = 1, CreatedDate = DateTime.UtcNow };

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<InternationalLicenseDto> { CreateSampleLicense(), CreateSampleLicense(2) };
            _internationalLicenseRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsLicense()
        {
            var expected = CreateSampleLicense();
            _internationalLicenseRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

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
            _internationalLicenseRepo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetByIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetActiveByDriverIdAsync_WithValidId_ReturnsLicense()
        {
            var expected = CreateSampleLicense();
            _internationalLicenseRepo.GetActiveByDriverIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetActiveByDriverIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetActiveByDriverIdAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetActiveByDriverIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetAllDriverLicensesAsync_WithValidId_ReturnsList()
        {
            var expected = new List<InternationalLicenseDto> { CreateSampleLicense() };
            _internationalLicenseRepo.GetAllDriverLicensesAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllDriverLicensesAsync(1, CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetAllDriverLicensesAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetAllDriverLicensesAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreated()
        {
            var request = new CreateInternationalLicenseRequest { IssuedUsingLocalLicenseId = 1, DriverId = 1 };
            var driver = CreateSampleDriver();
            var localLicense = new LocalLicenseAppDto(1, 1, (int)enLicenseClasses.OrdinaryDriving, 1, DateTime.UtcNow, 1, 1, DateTime.UtcNow, 100, 1);
            var appType = new ApplicationTypeDto((int)enApplicationType.NewInternationalLicense, "International License", 500);
            var createdApp = new ApplicationDto(1, 1, DateTime.UtcNow, (int)enApplicationType.NewInternationalLicense, (byte)enApplicationStatus.Completed, DateTime.UtcNow, 500, 1);
            var createdLicense = CreateSampleLicense();

            _currentUser.UserId.Returns(1);
            _driverRepo.GetByDriverIdAsync(1, Arg.Any<CancellationToken>()).Returns(driver);
            _localLicenseAppRepo.GetByLocalLicenseAppIdAsync(1, Arg.Any<CancellationToken>()).Returns(localLicense);
            _internationalLicenseRepo.GetActiveByDriverIdAsync(1, Arg.Any<CancellationToken>()).ReturnsNull();
            _applicationTypeRepo.GetByIdAsync((int)enApplicationType.NewInternationalLicense, Arg.Any<CancellationToken>()).Returns(appType);
            _applicationRepo.AddAsync(Arg.Any<CreateApplicationRequest>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(createdApp);
            _internationalLicenseRepo.CreateAsync(Arg.Any<InternationalLicenseDto>(), Arg.Any<CancellationToken>()).Returns(createdLicense);

            var result = await _sut.CreateAsync(request, CancellationToken.None);

            result.Should().Be(createdLicense);
        }

        [Fact]
        public async Task CreateAsync_WithNullRequest_ThrowsArgumentNullException()
        {
            var act = () => _sut.CreateAsync(null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidLocalLicenseId_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateInternationalLicenseRequest { IssuedUsingLocalLicenseId = 0, DriverId = 1 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidDriverId_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateInternationalLicenseRequest { IssuedUsingLocalLicenseId = 1, DriverId = 0 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WhenDriverNotFound_ThrowsArgumentException()
        {
            var request = new CreateInternationalLicenseRequest { IssuedUsingLocalLicenseId = 1, DriverId = 99 };
            _driverRepo.GetByDriverIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*There is no driver found*");
        }

        [Fact]
        public async Task CreateAsync_WhenLocalLicenseNotFound_ThrowsArgumentException()
        {
            var request = new CreateInternationalLicenseRequest { IssuedUsingLocalLicenseId = 99, DriverId = 1 };
            var driver = CreateSampleDriver();
            _driverRepo.GetByDriverIdAsync(1, Arg.Any<CancellationToken>()).Returns(driver);
            _localLicenseAppRepo.GetByLocalLicenseAppIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*There is no local driving license found*");
        }

        [Fact]
        public async Task CreateAsync_WhenNotOrdinaryLicense_ThrowsArgumentException()
        {
            var request = new CreateInternationalLicenseRequest { IssuedUsingLocalLicenseId = 1, DriverId = 1 };
            var driver = CreateSampleDriver();
            var localLicense = new LocalLicenseAppDto(1, 1, 99, 1, DateTime.UtcNow, 1, 1, DateTime.UtcNow, 100, 1);
            _driverRepo.GetByDriverIdAsync(1, Arg.Any<CancellationToken>()).Returns(driver);
            _localLicenseAppRepo.GetByLocalLicenseAppIdAsync(1, Arg.Any<CancellationToken>()).Returns(localLicense);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*International licenses issued by only ordinary driving license*");
        }

        [Fact]
        public async Task CreateAsync_WhenActiveLicenseExists_ThrowsArgumentException()
        {
            var request = new CreateInternationalLicenseRequest { IssuedUsingLocalLicenseId = 1, DriverId = 1 };
            var driver = CreateSampleDriver();
            var localLicense = new LocalLicenseAppDto(1, 1, (int)enLicenseClasses.OrdinaryDriving, 1, DateTime.UtcNow, 1, 1, DateTime.UtcNow, 100, 1);
            _driverRepo.GetByDriverIdAsync(1, Arg.Any<CancellationToken>()).Returns(driver);
            _localLicenseAppRepo.GetByLocalLicenseAppIdAsync(1, Arg.Any<CancellationToken>()).Returns(localLicense);
            _internationalLicenseRepo.GetActiveByDriverIdAsync(1, Arg.Any<CancellationToken>()).Returns(CreateSampleLicense());

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*There is an active international license*");
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsUpdated()
        {
            var request = new InternationalLicenseDto(1, 1, 1, 1, DateTime.UtcNow, DateTime.UtcNow.AddYears(1), true, 1);
            var updated = request;
            _internationalLicenseRepo.UpdateAsync(1, Arg.Any<InternationalLicenseDto>(), Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _sut.UpdateAsync(1, request, CancellationToken.None);

            result.Should().Be(updated);
        }

        [Fact]
        public async Task UpdateAsync_WithNullRequest_ThrowsArgumentNullException()
        {
            var act = () => _sut.UpdateAsync(1, null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var request = new InternationalLicenseDto();

            var act = () => _sut.UpdateAsync(0, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
    }
}
