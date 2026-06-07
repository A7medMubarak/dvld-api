using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Dtos.ApplicationType;
using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Dtos.TestAppointment;
using DVLD.Contracts.Exceptions;
using DVLD.Contracts.Requests.TestAppointment;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class TestAppointmentServiceTests
    {
        private readonly ITestAppointmentRepository _testAppointmentRepo;
        private readonly ILocalLicenseAppRepository _localLicenseAppRepo;
        private readonly IApplicationRepository _appRepo;
        private readonly IApplicationTypeRepository _appTypeRepo;
        private readonly ITestTypeRepository _testTypeRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly TestAppointmentService _sut;

        public TestAppointmentServiceTests()
        {
            _testAppointmentRepo = Substitute.For<ITestAppointmentRepository>();
            _localLicenseAppRepo = Substitute.For<ILocalLicenseAppRepository>();
            _appRepo = Substitute.For<IApplicationRepository>();
            _appTypeRepo = Substitute.For<IApplicationTypeRepository>();
            _testTypeRepo = Substitute.For<ITestTypeRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _sut = new TestAppointmentService(_testAppointmentRepo, _localLicenseAppRepo, _appRepo, _appTypeRepo, _testTypeRepo, _currentUser);
        }

        private static TestAppointmentDto CreateSampleAppointment(int id = 1) => new(id, 1, 1, DateTime.UtcNow.AddDays(1), 100, 1, false, null);

        private static TestAppointmentViewDto CreateSampleView() => new(1, 1, "Vision", "Ordinary", DateTime.UtcNow.AddDays(1), 100, "John Doe", false);

        private static TestDto CreateSampleTest() => new(1, 1, false, null, 1);

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<TestAppointmentViewDto> { CreateSampleView() };
            _testAppointmentRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsAppointment()
        {
            var expected = CreateSampleAppointment();
            _testAppointmentRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

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
            _testAppointmentRepo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetByIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetTestAsync_WithValidId_ReturnsTest()
        {
            var expected = CreateSampleTest();
            _testAppointmentRepo.GetTestAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetTestAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetTestAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetTestAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetLastAsync_WithValidIds_ReturnsAppointment()
        {
            var expected = CreateSampleAppointment();
            _localLicenseAppRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _testAppointmentRepo.GetLastAsync(1, 1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetLastAsync(1, 1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetAllByTestTypeAsync_WithValidIds_ReturnsList()
        {
            var expected = new List<TestAppointmentByTestTypeDto> { new() { TestAppointmentId = 1, AppointmentDate = DateTime.UtcNow.AddDays(1), PaidFees = 100 } };
            _localLicenseAppRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _testAppointmentRepo.GetAllByTestTypeAsync(1, 1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllByTestTypeAsync(1, 1, CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetAllByTestTypeAsync_WithInvalidLocalLicenseAppId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetAllByTestTypeAsync(0, 1, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetLastAsync_WithInvalidTestTypeId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetLastAsync(1, 99, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreated()
        {
            var request = new CreateTestAppointmentRequest { LocalDrivingLicenseApplicationId = 1, TestTypeId = (int)enTestType.VisionTest, AppointmentDate = DateTime.UtcNow.AddDays(1) };
            var testType = new DVLD.Contracts.Dtos.TestType.TestTypeDto(1, "Vision", "Test", 50);
            var created = CreateSampleAppointment();

            _currentUser.UserId.Returns(1);
            _localLicenseAppRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _localLicenseAppRepo.HasActiveTestAppointmentAsync(1, (int)enTestType.VisionTest, Arg.Any<CancellationToken>()).Returns(false);
            _testAppointmentRepo.GetLastAsync(1, (int)enTestType.VisionTest, Arg.Any<CancellationToken>()).ReturnsNull();
            _testTypeRepo.GetByIdAsync((int)enTestType.VisionTest, Arg.Any<CancellationToken>()).Returns(testType);
            _testAppointmentRepo.CreateAsync(Arg.Any<TestAppointmentDto>(), Arg.Any<CancellationToken>()).Returns(created);

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
        public async Task CreateAsync_WithInvalidLocalLicenseAppId_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateTestAppointmentRequest { LocalDrivingLicenseApplicationId = 0, TestTypeId = (int)enTestType.VisionTest, AppointmentDate = DateTime.UtcNow.AddDays(1) };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidTestTypeId_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateTestAppointmentRequest { LocalDrivingLicenseApplicationId = 1, TestTypeId = 99, AppointmentDate = DateTime.UtcNow.AddDays(1) };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithPastDate_ThrowsArgumentException()
        {
            var request = new CreateTestAppointmentRequest { LocalDrivingLicenseApplicationId = 1, TestTypeId = (int)enTestType.VisionTest, AppointmentDate = DateTime.UtcNow.AddDays(-1) };
            _localLicenseAppRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Appointment date must be in the future*");
        }

        [Fact]
        public async Task CreateAsync_WhenLocalAppNotExists_ThrowsArgumentException()
        {
            var request = new CreateTestAppointmentRequest { LocalDrivingLicenseApplicationId = 99, TestTypeId = (int)enTestType.VisionTest, AppointmentDate = DateTime.UtcNow.AddDays(1) };
            _localLicenseAppRepo.ExistsAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*does not exist*");
        }

        [Fact]
        public async Task CreateAsync_WhenActiveAppointmentExists_ThrowsArgumentException()
        {
            var request = new CreateTestAppointmentRequest { LocalDrivingLicenseApplicationId = 1, TestTypeId = (int)enTestType.VisionTest, AppointmentDate = DateTime.UtcNow.AddDays(1) };
            _localLicenseAppRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _localLicenseAppRepo.HasActiveTestAppointmentAsync(1, (int)enTestType.VisionTest, Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*He has an active appointment*");
        }

        [Fact]
        public async Task CreateAsync_WrittenTestRequiresVisionPassedFirst()
        {
            var request = new CreateTestAppointmentRequest { LocalDrivingLicenseApplicationId = 1, TestTypeId = (int)enTestType.WrittenTest, AppointmentDate = DateTime.UtcNow.AddDays(1) };
            _localLicenseAppRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _localLicenseAppRepo.HasActiveTestAppointmentAsync(1, (int)enTestType.WrittenTest, Arg.Any<CancellationToken>()).Returns(false);
            _localLicenseAppRepo.HasPassedTestAsync(1, (int)enTestType.VisionTest, Arg.Any<CancellationToken>()).Returns(false);
            _testAppointmentRepo.GetLastAsync(1, (int)enTestType.WrittenTest, Arg.Any<CancellationToken>()).ReturnsNull();

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Vision Test must be passed first*");
        }

        [Fact]
        public async Task CreateAsync_StreetTestRequiresWrittenPassedFirst()
        {
            var request = new CreateTestAppointmentRequest { LocalDrivingLicenseApplicationId = 1, TestTypeId = (int)enTestType.StreetTest, AppointmentDate = DateTime.UtcNow.AddDays(1) };
            _localLicenseAppRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _localLicenseAppRepo.HasActiveTestAppointmentAsync(1, (int)enTestType.StreetTest, Arg.Any<CancellationToken>()).Returns(false);
            _localLicenseAppRepo.HasPassedTestAsync(1, (int)enTestType.WrittenTest, Arg.Any<CancellationToken>()).Returns(false);
            _testAppointmentRepo.GetLastAsync(1, (int)enTestType.StreetTest, Arg.Any<CancellationToken>()).ReturnsNull();

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Written Test must be passed first*");
        }

        [Fact]
        public async Task CreateAsync_WithRetake_WhenAlreadyPassed_ThrowsArgumentException()
        {
            var request = new CreateTestAppointmentRequest { LocalDrivingLicenseApplicationId = 1, TestTypeId = (int)enTestType.VisionTest, AppointmentDate = DateTime.UtcNow.AddDays(1) };
            var lastAppointment = CreateSampleAppointment();
            var lastTest = new TestDto(1, 1, true, "Passed", 1);

            _localLicenseAppRepo.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _localLicenseAppRepo.HasActiveTestAppointmentAsync(1, (int)enTestType.VisionTest, Arg.Any<CancellationToken>()).Returns(false);
            _testAppointmentRepo.GetLastAsync(1, (int)enTestType.VisionTest, Arg.Any<CancellationToken>()).Returns(lastAppointment);
            _testAppointmentRepo.GetTestAsync(1, Arg.Any<CancellationToken>()).Returns(lastTest);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*He passed this test before*");
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsUpdated()
        {
            var request = new UpdateTestAppointmentRequest { AppointmentDate = DateTime.UtcNow.AddDays(2) };
            var existing = CreateSampleAppointment();
            var updated = new TestAppointmentDto(1, 1, 1, DateTime.UtcNow.AddDays(2), 100, 1, false, null);

            _testAppointmentRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existing);
            _testAppointmentRepo.UpdateAsync(1, Arg.Any<TestAppointmentDto>(), Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _sut.UpdateAsync(1, request, CancellationToken.None);

            result.Should().Be(updated);
        }

        [Fact]
        public async Task UpdateAsync_WithPastDate_ThrowsArgumentException()
        {
            var request = new UpdateTestAppointmentRequest { AppointmentDate = DateTime.UtcNow.AddDays(-1) };

            var act = () => _sut.UpdateAsync(1, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Appointment date must be in the future*");
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var request = new UpdateTestAppointmentRequest { AppointmentDate = DateTime.UtcNow.AddDays(1) };

            var act = () => _sut.UpdateAsync(0, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            var request = new UpdateTestAppointmentRequest { AppointmentDate = DateTime.UtcNow.AddDays(1) };
            _testAppointmentRepo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var act = () => _sut.UpdateAsync(99, request, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task UpdateAsync_WhenLocked_ThrowsResourceConflictException()
        {
            var request = new UpdateTestAppointmentRequest { AppointmentDate = DateTime.UtcNow.AddDays(1) };
            var locked = new TestAppointmentDto(1, 1, 1, DateTime.UtcNow.AddDays(1), 100, 1, true, null);
            _testAppointmentRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(locked);

            var act = () => _sut.UpdateAsync(1, request, CancellationToken.None);

            await act.Should().ThrowAsync<ResourceConflictException>()
                .WithMessage("*Cannot update a locked appointment*");
        }
    }
}
