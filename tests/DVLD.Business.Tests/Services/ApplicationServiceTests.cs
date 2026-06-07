using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Exceptions;
using DVLD.Contracts.Requests.Application;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class ApplicationServiceTests
    {
        private readonly IApplicationRepository _repo;
        private readonly ICurrentUserService _currentUser;
        private readonly ApplicationService _sut;

        public ApplicationServiceTests()
        {
            _repo = Substitute.For<IApplicationRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _sut = new ApplicationService(_repo, _currentUser);
        }

        private static ApplicationDto CreateSampleApp(int id = 1) => new(id, 1, DateTime.UtcNow, 1, (byte)enApplicationStatus.New, DateTime.UtcNow, 100, 1);

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<ApplicationDto> { CreateSampleApp(), CreateSampleApp(2) };
            _repo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsApplication()
        {
            var expected = CreateSampleApp();
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
        public async Task GetActiveAsync_WithLicenseClassId_CallsGetActiveForLicenseClass()
        {
            var expected = CreateSampleApp();
            _repo.GetActiveForLicenseClassAsync(1, 1, 1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetActiveAsync(1, 1, 1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetActiveAsync_WithoutLicenseClassId_CallsGetActive()
        {
            var expected = CreateSampleApp();
            _repo.GetActiveAsync(1, 1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetActiveAsync(1, 1, null, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetActiveAsync_WithInvalidPersonId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetActiveAsync(0, 1, null, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetActiveAsync_WithInvalidTypeId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetActiveAsync(1, 0, null, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreated()
        {
            var request = new CreateApplicationRequest(1, 1, (byte)enApplicationStatus.New, 100, 0);
            var created = CreateSampleApp();
            _currentUser.UserId.Returns(1);
            _repo.AddAsync(Arg.Any<CreateApplicationRequest>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(created);

            var result = await _sut.CreateAsync(request, CancellationToken.None);

            result.Should().Be(created);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidPersonId_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateApplicationRequest(0, 1, (byte)enApplicationStatus.New, 100, 1);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidAppTypeId_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateApplicationRequest(1, 0, (byte)enApplicationStatus.New, 100, 1);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidStatus_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateApplicationRequest(1, 1, 99, 100, 1);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidFees_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateApplicationRequest(1, 1, (byte)enApplicationStatus.New, 0, 1);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithApplicationStatusZero_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateApplicationRequest(1, 1, 0, 100, 1);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidApplicationTypeIdPositive_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateApplicationRequest(1, 99, (byte)enApplicationStatus.New, 100, 1);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsUpdated()
        {
            var request = new UpdateApplicationRequest { ApplicantPersonId = 1, ApplicationDate = DateTime.UtcNow, ApplicationTypeId = 1, ApplicationStatus = (byte)enApplicationStatus.Completed, LastStatusDate = DateTime.UtcNow, PaidFees = 200, CreatedByUserId = 1 };
            var updated = new ApplicationDto(1, 1, DateTime.UtcNow, 1, (byte)enApplicationStatus.Completed, DateTime.UtcNow, 200, 1);
            _repo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _repo.UpdateAsync(1, Arg.Any<UpdateApplicationRequest>(), Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _sut.UpdateAsync(1, request, CancellationToken.None);

            result.Should().Be(updated);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var request = new UpdateApplicationRequest();

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
        public async Task UpdateAsync_WithInvalidBodyFields_ThrowsArgumentOutOfRangeException()
        {
            var request = new UpdateApplicationRequest { ApplicantPersonId = 0 };
            _repo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.UpdateAsync(1, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            var request = new UpdateApplicationRequest { ApplicantPersonId = 1, ApplicationDate = DateTime.UtcNow, ApplicationTypeId = 1, ApplicationStatus = (byte)enApplicationStatus.Completed, LastStatusDate = DateTime.UtcNow, PaidFees = 200, CreatedByUserId = 1 };
            _repo.ExistsAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.UpdateAsync(99, request, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
        {
            _repo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

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
            _repo.ExistsAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.DeleteAsync(99, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task UpdateStatusAsync_WithValidIds_UpdatesSuccessfully()
        {
            _repo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _repo.UpdateStatusAsync(1, (short)enApplicationStatus.Completed, Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.UpdateStatusAsync(1, (short)enApplicationStatus.Completed, CancellationToken.None);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task UpdateStatusAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.UpdateStatusAsync(0, (short)enApplicationStatus.Completed, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateStatusAsync_WithInvalidStatus_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.UpdateStatusAsync(1, 99, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            _repo.ExistsAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.UpdateStatusAsync(99, (short)enApplicationStatus.Completed, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenUpdateFails_ThrowsResourceConflictException()
        {
            _repo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _repo.UpdateStatusAsync(1, (short)enApplicationStatus.Completed, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.UpdateStatusAsync(1, (short)enApplicationStatus.Completed, CancellationToken.None);

            await act.Should().ThrowAsync<ResourceConflictException>()
                .WithMessage("*Cannot update the application status*");
        }
    }
}
