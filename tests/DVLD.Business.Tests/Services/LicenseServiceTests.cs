using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.License;
using DVLD.Contracts.Requests.License;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class LicenseServiceTests
    {
        private readonly ILicenseRepository _licensesRepo;
        private readonly ILicenseClassRepository _licenseClassesRepo;
        private readonly IPersonRepository _peopleRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly LicenseService _sut;

        public LicenseServiceTests()
        {
            _licensesRepo = Substitute.For<ILicenseRepository>();
            _licenseClassesRepo = Substitute.For<ILicenseClassRepository>();
            _peopleRepo = Substitute.For<IPersonRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _sut = new LicenseService(_licensesRepo, _licenseClassesRepo, _peopleRepo, _currentUser);
        }

        private static LicenseDto CreateSampleLicense(int id = 1) => new(id, 1, 1, 1, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(335), null, 100, true, enIssueReason.FirstTime, 1);

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsLicense()
        {
            var expected = CreateSampleLicense();
            _licensesRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

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
            _licensesRepo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetByIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<LicenseDto> { CreateSampleLicense(), CreateSampleLicense(2) };
            _licensesRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetActiveLicenseForPersonAsync_WithValidIds_ReturnsLicense()
        {
            var expected = CreateSampleLicense();
            _licensesRepo.GetActiveLicenseForPersonAsync(1, 1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetActiveLicenseForPersonAsync(1, 1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetActiveLicenseForPersonAsync_WithInvalidPersonId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetActiveLicenseForPersonAsync(0, 1, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetActiveLicenseForPersonAsync_WithInvalidLicenseClass_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetActiveLicenseForPersonAsync(1, 0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetDriverLicensesAsync_WithValidId_ReturnsList()
        {
            var expected = new List<DriverLicensesDto> { new(1, 1, "Ordinary", DateTime.UtcNow, DateTime.UtcNow.AddYears(1), true) };
            _licensesRepo.GetDriverLicensesAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetDriverLicensesAsync(1, CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetDriverLicensesAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetDriverLicensesAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreated()
        {
            var request = new CreateLicenseRequest { ApplicationId = 1, DriverId = 1, LicenseClassId = 1, IssueDate = DateTime.UtcNow, ExpirationDate = DateTime.UtcNow.AddYears(1), PaidFees = 100, IsActive = true, IssueReason = (byte)enIssueReason.FirstTime };
            var created = CreateSampleLicense();
            _currentUser.UserId.Returns(1);
            _licensesRepo.CreateAsync(Arg.Any<LicenseDto>(), Arg.Any<CancellationToken>()).Returns(created);

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
        public async Task CreateAsync_WithInvalidApplicationId_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateLicenseRequest { ApplicationId = 0, DriverId = 1, LicenseClassId = 1, IssueDate = DateTime.UtcNow, ExpirationDate = DateTime.UtcNow.AddYears(1), PaidFees = 100, IsActive = true, IssueReason = (byte)enIssueReason.FirstTime };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithExpirationBeforeIssue_ThrowsArgumentException()
        {
            var request = new CreateLicenseRequest { ApplicationId = 1, DriverId = 1, LicenseClassId = 1, IssueDate = DateTime.UtcNow, ExpirationDate = DateTime.UtcNow.AddDays(-1), PaidFees = 100, IsActive = true, IssueReason = (byte)enIssueReason.FirstTime };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Expiration date must be after issue date*");
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsUpdated()
        {
            var request = new UpdateLicenseRequest { ApplicationId = 1, DriverId = 1, LicenseClassId = 1, IssueDate = DateTime.UtcNow, ExpirationDate = DateTime.UtcNow.AddYears(1), PaidFees = 150, IsActive = true, IssueReason = (byte)enIssueReason.Renew };
            var updated = new LicenseDto(1, 1, 1, 1, DateTime.UtcNow, DateTime.UtcNow.AddYears(1), null, 150, true, enIssueReason.Renew, 1);
            _currentUser.UserId.Returns(1);
            _licensesRepo.UpdateAsync(1, Arg.Any<LicenseDto>(), Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _sut.UpdateAsync(1, request, CancellationToken.None);

            result.Should().Be(updated);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var request = new UpdateLicenseRequest();

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
        public async Task DeactivateAsync_WithValidId_ReturnsTrue()
        {
            _licensesRepo.DeactivateAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.DeactivateAsync(1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeactivateAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.DeactivateAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task ExistsAsync_WhenExists_ReturnsTrue()
        {
            _licensesRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.ExistsAsync(1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_WhenNotExists_ReturnsFalse()
        {
            _licensesRepo.ExistsAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var result = await _sut.ExistsAsync(99, CancellationToken.None);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.ExistsAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
    }
}
