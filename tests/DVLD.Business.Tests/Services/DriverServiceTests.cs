using DVLD.Contracts.Dtos.Driver;
using DVLD.Contracts.Dtos.Person;
using DVLD.Contracts.Requests.Driver;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class DriverServiceTests
    {
        private readonly IDriverRepository _driversRepo;
        private readonly IPersonRepository _peopleRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly DriverService _sut;

        public DriverServiceTests()
        {
            _driversRepo = Substitute.For<IDriverRepository>();
            _peopleRepo = Substitute.For<IPersonRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _sut = new DriverService(_driversRepo, _peopleRepo, _currentUser);
        }

        private static DriverDto CreateSampleDriver(int id = 1) => new()
        {
            DriverId = id,
            PersonId = 1,
            CreatedByUserId = 1,
            CreatedDate = DateTime.UtcNow
        };

        private static PersonDto CreateSamplePerson() => new()
        {
            PersonId = 1,
            FirstName = "John",
            SecondName = "William",
            LastName = "Doe",
            NationalNo = "N123456",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = 0,
            Email = "john@example.com"
        };

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<DriverDto> { CreateSampleDriver(), CreateSampleDriver(2) };
            _driversRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByDriverIdAsync_WithValidId_ReturnsDriver()
        {
            var expected = CreateSampleDriver();
            _driversRepo.GetByDriverIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByDriverIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByDriverIdAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetByDriverIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*driverId must be greater than zero*");
        }

        [Fact]
        public async Task GetByDriverIdAsync_WhenNotFound_ReturnsNull()
        {
            _driversRepo.GetByDriverIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetByDriverIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByPersonIdAsync_WithValidId_ReturnsDriver()
        {
            var expected = CreateSampleDriver();
            _driversRepo.GetByPersonIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByPersonIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByPersonIdAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.GetByPersonIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*personId must be greater than zero*");
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreated()
        {
            var request = new CreateDriverRequest { PersonId = 1 };
            var created = CreateSampleDriver();
            _currentUser.UserId.Returns(1);
            _peopleRepo.ExistsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _driversRepo.CreateAsync(Arg.Any<DriverDto>(), Arg.Any<CancellationToken>()).Returns(created);

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
        public async Task CreateAsync_WithInvalidPersonId_ThrowsArgumentException()
        {
            var request = new CreateDriverRequest { PersonId = 0 };

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("PersonId must be greater than zero. (Parameter 'PersonId')");
        }

        [Fact]
        public async Task CreateAsync_WhenPersonNotExists_ThrowsKeyNotFoundException()
        {
            var request = new CreateDriverRequest { PersonId = 99 };
            _peopleRepo.ExistsByIdAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*Person not exists*");
        }

        [Fact]
        public async Task GetDetailedByDriverIdAsync_WithValidId_ReturnsEnrichedDriver()
        {
            var driver = CreateSampleDriver();
            var person = CreateSamplePerson();
            _driversRepo.GetByDriverIdAsync(1, Arg.Any<CancellationToken>()).Returns(driver);
            _peopleRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(person);

            var result = await _sut.GetDetailedByDriverIdAsync(1, CancellationToken.None);

            result.Should().NotBeNull();
            result!.DriverId.Should().Be(1);
            result.Person.Should().Be(person);
        }

        [Fact]
        public async Task GetDetailedByDriverIdAsync_WhenNotFound_ReturnsNull()
        {
            _driversRepo.GetByDriverIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetDetailedByDriverIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetDetailedByDriverIdAsync_WithInvalidId_ThrowsArgumentException()
        {
            var act = () => _sut.GetDetailedByDriverIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("driverId must be greater than zero. (Parameter 'driverId')");
        }

        [Fact]
        public async Task GetDetailedByPersonIdAsync_WithValidId_ReturnsEnrichedDriver()
        {
            var driver = CreateSampleDriver();
            var person = CreateSamplePerson();
            _driversRepo.GetByPersonIdAsync(1, Arg.Any<CancellationToken>()).Returns(driver);
            _peopleRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(person);

            var result = await _sut.GetDetailedByPersonIdAsync(1, CancellationToken.None);

            result.Should().NotBeNull();
            result!.PersonId.Should().Be(1);
        }

        [Fact]
        public async Task GetDetailedByPersonIdAsync_WhenNotFound_ReturnsNull()
        {
            _driversRepo.GetByPersonIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetDetailedByPersonIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task DriverExistsAsync_WhenExists_ReturnsTrue()
        {
            _driversRepo.ExistsByDriverIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.DriverExistsAsync(1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DriverExistsAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.DriverExistsAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task IsPersonDriverAsync_WhenExists_ReturnsTrue()
        {
            _driversRepo.ExistsByPersonIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.IsPersonDriverAsync(1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsPersonDriverAsync_WithInvalidId_ThrowsArgumentOutOfRangeException()
        {
            var act = () => _sut.IsPersonDriverAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
    }
}
