using DVLD.Contracts.Dtos.Country;
using DVLD.Contracts.Dtos.Person;
using DVLD.Contracts.Requests.Person;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class PersonServiceTests
    {
        private readonly IPersonRepository _personRepo;
        private readonly ICountryRepository _countryRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly PersonService _sut;

        public PersonServiceTests()
        {
            _personRepo = Substitute.For<IPersonRepository>();
            _countryRepo = Substitute.For<ICountryRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _sut = new PersonService(_personRepo, _countryRepo, _currentUser);
        }

        private static PersonDto CreateSamplePerson(int id = 1) => new()
        {
            PersonId = id,
            FirstName = "John",
            SecondName = "William",
            LastName = "Doe",
            NationalNo = "N123456",
            DateOfBirth = new DateTime(1990, 1, 1),
            Address = "123 Main St",
            Phone = "555-1234",
            Gender = 0,
            Email = "john@example.com",
            NationalityCountryId = 1,
            ImagePath = null,
            CreatedByUserId = 1
        };

        private static CreatePersonRequest CreateSampleRequest() => new()
        {
            FirstName = "John",
            SecondName = "William",
            LastName = "Doe",
            NationalNo = "N123456",
            DateOfBirth = new DateTime(1990, 1, 1),
            Address = "123 Main St",
            Phone = "555-1234",
            Gender = 0,
            Email = "john@example.com",
            NationalityCountryId = 1
        };

        // ===================== GetByIdAsync =====================

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsPerson()
        {
            var expected = CreateSamplePerson();
            _personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsArgumentException()
        {
            var act = () => _sut.GetByIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid personId.*");
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            _personRepo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetByIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        // ===================== GetByNationalNoAsync =====================

        [Fact]
        public async Task GetByNationalNoAsync_WithValidNationalNo_ReturnsPerson()
        {
            var expected = CreateSamplePerson();
            _personRepo.GetByNationalNoAsync("N123456", Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByNationalNoAsync("N123456", CancellationToken.None);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task GetByNationalNoAsync_WithInvalidInput_ThrowsArgumentException(string? nationalNo)
        {
            var act = () => _sut.GetByNationalNoAsync(nationalNo!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("nationalNo cannot be empty. (Parameter 'nationalNo')");
        }

        // ===================== GetAllAsync =====================

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<PersonDto> { CreateSamplePerson(), CreateSamplePerson(2) };
            _personRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        // ===================== CreateAsync =====================

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreatedPerson()
        {
            var request = CreateSampleRequest();
            var createdPerson = CreateSamplePerson();

            _currentUser.UserId.Returns(1);
            _personRepo.ExistsByNationalNoAsync(request.NationalNo, Arg.Any<CancellationToken>()).Returns(false);
            _personRepo.AddAsync(Arg.Any<PersonDto>(), Arg.Any<CancellationToken>()).Returns(createdPerson);

            var result = await _sut.CreateAsync(request, CancellationToken.None);

            result.Should().Be(createdPerson);
        }

        [Fact]
        public async Task CreateAsync_WithNullBody_ThrowsArgumentNullException()
        {
            var act = () => _sut.CreateAsync(null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("body is required.*");
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateNationalNo_ThrowsArgumentException()
        {
            var request = CreateSampleRequest();
            _personRepo.ExistsByNationalNoAsync(request.NationalNo, Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("NationalNo already exists.*");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task CreateAsync_WithMissingRequiredFields_ThrowsArgumentException(string? emptyValue)
        {
            var request = CreateSampleRequest();
            request.FirstName = emptyValue!;

            var act = () => _sut.CreateAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("FirstName cannot be empty. (Parameter 'FirstName')");
        }

        // ===================== UpdateAsync =====================

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsUpdatedPerson()
        {
            var existing = CreateSamplePerson();
            var request = new UpdatePersonRequest
            {
                FirstName = "Jane",
                SecondName = "William",
                LastName = "Doe",
                NationalNo = "N123456",
                DateOfBirth = existing.DateOfBirth,
                Address = "456 Oak St",
                Phone = "555-5678",
                Gender = 0,
                Email = "jane@example.com",
                NationalityCountryId = 1
            };

            _personRepo.ExistsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existing);
            _personRepo.UpdateAsync(1, Arg.Any<PersonDto>(), Arg.Any<CancellationToken>()).Returns(existing);

            var result = await _sut.UpdateAsync(1, request, CancellationToken.None);

            result.Should().Be(existing);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsArgumentException()
        {
            var request = new UpdatePersonRequest();

            var act = () => _sut.UpdateAsync(0, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid personId.*");
        }

        [Fact]
        public async Task UpdateAsync_WithNullBody_ThrowsArgumentNullException()
        {
            var act = () => _sut.UpdateAsync(1, null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Body is required.*");
        }

        [Fact]
        public async Task UpdateAsync_WhenPersonNotFound_ThrowsKeyNotFoundException()
        {
            var request = new UpdatePersonRequest { FirstName = "Jane", SecondName = "A", LastName = "B", NationalNo = "X", NationalityCountryId = 1 };
            _personRepo.ExistsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.UpdateAsync(1, request, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Person not found.*");
        }

        [Fact]
        public async Task UpdateAsync_WithNationalNoConflict_ThrowsArgumentException()
        {
            var existing = CreateSamplePerson();
            var request = new UpdatePersonRequest
            {
                FirstName = "Jane",
                SecondName = "A",
                LastName = "B",
                NationalNo = "DIFFERENT_NO",
                NationalityCountryId = 1
            };

            _personRepo.ExistsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existing);
            _personRepo.ExistsByNationalNoAsync("DIFFERENT_NO", Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.UpdateAsync(1, request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("nationalNo already exists.*");
        }

        // ===================== DeleteAsync =====================

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
        {
            _personRepo.ExistsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.DeleteAsync(1, CancellationToken.None);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ThrowsArgumentException()
        {
            var act = () => _sut.DeleteAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid personId.*");
        }

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            _personRepo.ExistsByIdAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.DeleteAsync(99, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Person not found.*");
        }

        // ===================== ExistsByIdAsync =====================

        [Fact]
        public async Task ExistsByIdAsync_WhenPersonExists_ReturnsTrue()
        {
            _personRepo.ExistsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.ExistsByIdAsync(1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByIdAsync_WhenPersonNotFound_ReturnsFalse()
        {
            _personRepo.ExistsByIdAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var result = await _sut.ExistsByIdAsync(99, CancellationToken.None);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsByIdAsync_WithInvalidId_ThrowsArgumentException()
        {
            var act = () => _sut.ExistsByIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid personId.*");
        }

        // ===================== ExistsByNationalNoAsync =====================

        [Fact]
        public async Task ExistsByNationalNoAsync_WhenPersonExists_ReturnsTrue()
        {
            _personRepo.ExistsByNationalNoAsync("N123456", Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.ExistsByNationalNoAsync("N123456", CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByNationalNoAsync_WithInvalidInput_ThrowsArgumentException()
        {
            var act = () => _sut.ExistsByNationalNoAsync("", CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("nationalNo cannot be empty. (Parameter 'nationalNo')");
        }

        // ===================== GetDetailsByIdAsync =====================

        [Fact]
        public async Task GetDetailsByIdAsync_WithValidId_ReturnsEnrichedPerson()
        {
            var person = CreateSamplePerson();
            var country = new CountryDto(1, "Egypt");

            _personRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(person);
            _countryRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(country);

            var result = await _sut.GetDetailsByIdAsync(1, CancellationToken.None);

            result.Should().NotBeNull();
            result!.CountryName.Should().Be("Egypt");
            result.PersonId.Should().Be(1);
        }

        [Fact]
        public async Task GetDetailsByIdAsync_WhenNotFound_ReturnsNull()
        {
            _personRepo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetDetailsByIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetDetailsByIdAsync_WithInvalidId_ThrowsArgumentException()
        {
            var act = () => _sut.GetDetailsByIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid personId.*");
        }

        // ===================== GetDetailsByNationalNoAsync =====================

        [Fact]
        public async Task GetDetailsByNationalNoAsync_WithValidNationalNo_ReturnsEnrichedPerson()
        {
            var person = CreateSamplePerson();
            var country = new CountryDto(1, "Egypt");

            _personRepo.GetByNationalNoAsync("N123456", Arg.Any<CancellationToken>()).Returns(person);
            _countryRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(country);

            var result = await _sut.GetDetailsByNationalNoAsync("N123456", CancellationToken.None);

            result.Should().NotBeNull();
            result!.CountryName.Should().Be("Egypt");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task GetDetailsByNationalNoAsync_WithInvalidInput_ThrowsArgumentException(string? nationalNo)
        {
            var act = () => _sut.GetDetailsByNationalNoAsync(nationalNo!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("nationalNo cannot be empty. (Parameter 'nationalNo')");
        }

        [Fact]
        public async Task GetDetailsByNationalNoAsync_WhenNotFound_ReturnsNull()
        {
            _personRepo.GetByNationalNoAsync("UNKNOWN", Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetDetailsByNationalNoAsync("UNKNOWN", CancellationToken.None);

            result.Should().BeNull();
        }
    }
}
