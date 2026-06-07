using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Requests.Test;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class TestServiceTests
    {
        private readonly ITestRepository _testRepo;
        private readonly IPersonRepository _peopleRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly TestService _sut;

        public TestServiceTests()
        {
            _testRepo = Substitute.For<ITestRepository>();
            _peopleRepo = Substitute.For<IPersonRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            var licenseClassRepo = Substitute.For<ILicenseClassRepository>();
            var localLicenseAppRepo = Substitute.For<ILocalLicenseAppRepository>();
            var appointmentRepo = Substitute.For<ITestAppointmentRepository>();
            _sut = new TestService(_testRepo, licenseClassRepo, _peopleRepo, localLicenseAppRepo, appointmentRepo, _currentUser);
        }

        private static TestDto CreateSampleTest(int id = 1) => new(id, 1, true, "Passed", 1);

        private static CreateTestRequest CreateSampleRequest() => new()
        {
            TestAppointmentId = 1,
            TestResult = true,
            Notes = "Passed"
        };

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<TestDto> { CreateSampleTest(), CreateSampleTest(2) };
            _testRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsTest()
        {
            var expected = CreateSampleTest();
            _testRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

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
            _testRepo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetByIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetLatestAsync_WithValidIds_ReturnsTest()
        {
            var expected = new TestWithApplicantDto(1, 1, true, "Passed", 1, 1);
            _peopleRepo.ExistsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _testRepo.GetLatestAsync(1, 1, 1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetLatestAsync(1, 1, 1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetLatestAsync_WithInvalidPersonId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetLatestAsync(0, 1, 1, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetLatestAsync_WhenPersonNotFound_ThrowsArgumentException()
        {
            _peopleRepo.ExistsByIdAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.GetLatestAsync(99, 1, 1, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*does not exist*");
        }

        [Fact]
        public async Task GetLatestAsync_WithInvalidTestTypeId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetLatestAsync(1, 1, 0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetLatestAsync_WithInvalidLicenseClassId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetLatestAsync(1, 0, 1, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetPassedCountAsync_WithValidId_ReturnsCount()
        {
            _testRepo.GetPassedCountAsync(1, Arg.Any<CancellationToken>()).Returns(3);

            var result = await _sut.GetPassedCountAsync(1, CancellationToken.None);

            result.Should().Be(3);
        }

        [Fact]
        public async Task GetPassedCountAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetPassedCountAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreated()
        {
            var request = CreateSampleRequest();
            var created = CreateSampleTest();
            _currentUser.UserId.Returns(1);
            _testRepo.CreateAsync(Arg.Any<TestDto>(), Arg.Any<CancellationToken>()).Returns(created);

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
        public async Task CreateAsync_WithInvalidAppointmentId_ThrowsArgumentOutOfRangeException()
        {
            var request = new CreateTestRequest { TestAppointmentId = 0 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsUpdated()
        {
            var request = new UpdateTestRequest { TestAppointmentId = 1, TestResult = false, Notes = "Failed" };
            var updated = new TestDto(1, 1, false, "Failed", 1);
            _currentUser.UserId.Returns(1);
            _testRepo.UpdateAsync(1, Arg.Any<TestDto>(), Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _sut.UpdateAsync(1, request, CancellationToken.None);

            result.Should().Be(updated);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var request = new UpdateTestRequest();

            var act = () => _sut.UpdateAsync(0, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateAsync_WithNullBody_ThrowsArgumentNullException()
        {
            var act = () => _sut.UpdateAsync(1, null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
