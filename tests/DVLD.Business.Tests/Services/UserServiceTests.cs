using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.User;
using DVLD.Contracts.Exceptions;
using DVLD.Contracts.Requests.User;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class UserServiceTests
    {
        private readonly IUserRepository _usersRepo;
        private readonly IPersonRepository _peopleRepo;
        private readonly UserService _sut;

        public UserServiceTests()
        {
            _usersRepo = Substitute.For<IUserRepository>();
            _peopleRepo = Substitute.For<IPersonRepository>();
            _sut = new UserService(_usersRepo, _peopleRepo);
        }

        private static UserDto CreateSampleUser(int id = 1) => new(id, 1, "testuser", true, enRole.Admin);

        [Fact]
        public async Task GetByUserIdAsync_WithValidId_ReturnsUser()
        {
            var expected = CreateSampleUser();
            _usersRepo.GetByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByUserIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByUserIdAsync_WithInvalidId_ThrowsArgumentException()
        {
            var act = () => _sut.GetByUserIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("userId must be greater than zero. (Parameter 'userId')");
        }

        [Fact]
        public async Task GetByUserIdAsync_WhenNotFound_ReturnsNull()
        {
            _usersRepo.GetByUserIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetByUserIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByPersonIdAsync_WithValidId_ReturnsUser()
        {
            var expected = CreateSampleUser();
            _usersRepo.GetByPersonIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByPersonIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByPersonIdAsync_WithInvalidId_ThrowsArgumentException()
        {
            var act = () => _sut.GetByPersonIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<UserDto> { CreateSampleUser(), CreateSampleUser(2) };
            _usersRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsCreated()
        {
            var dto = new CreateUserDto { PersonId = 1, UserName = "newuser", Password = "StrongP@ss1", Role = enRole.Officer };
            var created = new UserDto(1, 1, "newuser", true, enRole.Officer);
            _usersRepo.ExistsByPersonIdAsync(1, Arg.Any<CancellationToken>()).Returns(false);
            _peopleRepo.ExistsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _usersRepo.AddAsync(Arg.Any<CreateUserDto>(), Arg.Any<CancellationToken>()).Returns(created);

            var result = await _sut.CreateAsync(dto, CancellationToken.None);

            result.Should().Be(created);
        }

        [Fact]
        public async Task CreateAsync_WithNullBody_ThrowsArgumentNullException()
        {
            var act = () => _sut.CreateAsync(null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateAsync_WhenUserAlreadyExists_ThrowsResourceConflictException()
        {
            var dto = new CreateUserDto { PersonId = 1, UserName = "newuser", Password = "StrongP@ss1", Role = enRole.Officer };
            _usersRepo.ExistsByPersonIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.CreateAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<ResourceConflictException>()
                .WithMessage("*User already exists*");
        }

        [Fact]
        public async Task CreateAsync_WhenPersonNotExists_ThrowsKeyNotFoundException()
        {
            var dto = new CreateUserDto { PersonId = 99, UserName = "newuser", Password = "StrongP@ss1", Role = enRole.Officer };
            _usersRepo.ExistsByPersonIdAsync(99, Arg.Any<CancellationToken>()).Returns(false);
            _peopleRepo.ExistsByIdAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.CreateAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*There is no person*");
        }

        [Fact]
        public async Task CreateAsync_WithWeakPassword_ThrowsArgumentException()
        {
            var dto = new CreateUserDto { PersonId = 1, UserName = "newuser", Password = "weak", Role = enRole.Officer };

            var act = () => _sut.CreateAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Password must meet complexity*");
        }

        [Fact]
        public async Task CreateAsync_WithEmptyUsername_ThrowsArgumentException()
        {
            var dto = new CreateUserDto { PersonId = 1, UserName = "", Password = "StrongP@ss1", Role = enRole.Officer };

            var act = () => _sut.CreateAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Username is required*");
        }

        [Fact]
        public async Task CreateAsync_WithInvalidRole_ThrowsArgumentException()
        {
            var dto = new CreateUserDto { PersonId = 1, UserName = "newuser", Password = "StrongP@ss1", Role = (enRole)99 };

            var act = () => _sut.CreateAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Invalid role*");
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsUpdated()
        {
            var dto = new UpdateUserDto { UserName = "updateduser", Role = enRole.Admin };
            var updated = new UserDto(1, 1, "updateduser", true, enRole.Admin);
            _usersRepo.UpdateAsync(1, Arg.Any<UpdateUserDto>(), Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _sut.UpdateAsync(1, dto, CancellationToken.None);

            result.Should().Be(updated);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsArgumentException()
        {
            var dto = new UpdateUserDto();

            var act = () => _sut.UpdateAsync(0, dto, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("userId must be greater than zero. (Parameter 'userId')");
        }

        [Fact]
        public async Task UpdateAsync_WithNullBody_ThrowsArgumentException()
        {
            var act = () => _sut.UpdateAsync(1, null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("dto cannot be null (Parameter 'dto')");
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
        {
            _usersRepo.ExistsByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.DeleteAsync(1, CancellationToken.None);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ThrowsArgumentException()
        {
            var act = () => _sut.DeleteAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            _usersRepo.ExistsByUserIdAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.DeleteAsync(99, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*User not found*");
        }

        [Fact]
        public async Task ChangePasswordAsync_WithValidInput_ChangesSuccessfully()
        {
            _usersRepo.ExistsByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            _usersRepo.ChangePasswordAsync(1, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

            var act = () => _sut.ChangePasswordAsync(1, "NewStr0ng!Pass", CancellationToken.None);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ChangePasswordAsync_WithShortPassword_ThrowsArgumentException()
        {
            _usersRepo.ExistsByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            var act = () => _sut.ChangePasswordAsync(1, "Short1!", CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Password must be at least 8 characters*");
        }

        [Fact]
        public async Task ChangePasswordAsync_WithoutComplexity_ThrowsArgumentException()
        {
            _usersRepo.ExistsByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);
            var act = () => _sut.ChangePasswordAsync(1, "alllowercase", CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Password must contain upper, lower, digit, and special character*");
        }

        [Fact]
        public async Task ChangePasswordAsync_WhenUserNotFound_ThrowsKeyNotFoundException()
        {
            _usersRepo.ExistsByUserIdAsync(99, Arg.Any<CancellationToken>()).Returns(false);

            var act = () => _sut.ChangePasswordAsync(99, "NewStr0ng!Pass", CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*User not found*");
        }

        [Fact]
        public async Task ExistsByUserIdAsync_WhenExists_ReturnsTrue()
        {
            _usersRepo.ExistsByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.ExistsByUserIdAsync(1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByUserIdAsync_WithInvalidId_ThrowsArgumentException()
        {
            var act = () => _sut.ExistsByUserIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task ExistsByPersonIdAsync_WhenExists_ReturnsTrue()
        {
            _usersRepo.ExistsByPersonIdAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.ExistsByPersonIdAsync(1, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByPersonIdAsync_WithInvalidId_ThrowsArgumentException()
        {
            var act = () => _sut.ExistsByPersonIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task ExistsByUsernameAsync_WhenExists_ReturnsTrue()
        {
            _usersRepo.ExistsByUsernameAsync("testuser", Arg.Any<CancellationToken>()).Returns(true);

            var result = await _sut.ExistsByUsernameAsync("testuser", CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByUsernameAsync_WithInvalidInput_ThrowsArgumentException()
        {
            var act = () => _sut.ExistsByUsernameAsync("", CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("username cannot be empty. (Parameter 'username')");
        }

        [Fact]
        public async Task GetDetailedByUserIdAsync_WithValidId_ReturnsEnrichedUser()
        {
            var user = CreateSampleUser();
            var person = new DVLD.Contracts.Dtos.Person.PersonDto { PersonId = 1, FirstName = "John", LastName = "Doe" };
            _usersRepo.GetByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
            _peopleRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(person);

            var result = await _sut.GetDetailedByUserIdAsync(1, CancellationToken.None);

            result.Should().NotBeNull();
            result!.UserId.Should().Be(1);
            result.Person.Should().Be(person);
        }

        [Fact]
        public async Task GetDetailedByUserIdAsync_WhenNotFound_ReturnsNull()
        {
            _usersRepo.GetByUserIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetDetailedByUserIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetDetailedByPersonIdAsync_WithValidId_ReturnsEnrichedUser()
        {
            var user = CreateSampleUser();
            var person = new DVLD.Contracts.Dtos.Person.PersonDto { PersonId = 1, FirstName = "John", LastName = "Doe" };
            _usersRepo.GetByPersonIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
            _peopleRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(person);

            var result = await _sut.GetDetailedByPersonIdAsync(1, CancellationToken.None);

            result.Should().NotBeNull();
        }
    }
}
