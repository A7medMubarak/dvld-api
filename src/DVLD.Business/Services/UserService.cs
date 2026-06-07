using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Auth;
using DVLD.Contracts.Dtos.User;
using DVLD.Contracts.Exceptions;
using DVLD.Contracts.Requests.User;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Common.Validation;

namespace DVLD.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IPersonRepository _people;

        public UserService(IUserRepository users, IPersonRepository people)
        {
            _users = users;
            _people = people;
        }

        public async Task ChangePasswordAsync(int userId, string newPassword, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(userId, nameof(userId));
            Guard.AgainstNullOrWhiteSpace(newPassword, nameof(newPassword));

            if (!await _users.ExistsByUserIdAsync(userId, cancellationToken))
                throw new KeyNotFoundException("User not found.");

            if (newPassword.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.");

            if (!HasComplexity(newPassword)) 
                throw new ArgumentException("Password must contain upper, lower, digit, and special character.");

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            if (!await _users.ChangePasswordAsync(userId, newPasswordHash,cancellationToken)) 
                throw new InvalidOperationException("Change password failed.");          
        }

        public async Task <UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(dto, nameof(dto));

            ValidateWrite(dto);

            // Check if the user already exists by personId.
            if (await _users.ExistsByPersonIdAsync(dto.PersonId, cancellationToken))           
                throw new ResourceConflictException("User already exists.");

            // Check existance of person before assign to user.
            if (!await _people.ExistsByIdAsync(dto.PersonId, cancellationToken))
                throw new KeyNotFoundException("There is no person, create a new person then create user.");

            // Hash the password HERE in the Service
            dto.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            return await _users.AddAsync(dto, cancellationToken);
        }

        public async Task DeleteAsync(int userId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(userId, nameof(userId));

            if (!await _users.ExistsByUserIdAsync(userId, cancellationToken))
                throw new KeyNotFoundException("User not found.");

            await _users.DeleteAsync(userId, cancellationToken);
        }

        public async Task< bool> ExistsByPersonIdAsync(int personId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(personId, nameof(personId));

            return await _users.ExistsByPersonIdAsync(personId, cancellationToken);
        }

        public async Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(userId, nameof(userId));

            return await _users.ExistsByUserIdAsync(userId, cancellationToken);
        }

        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            Guard.AgainstNullOrWhiteSpace(username, nameof(username));

            return await _users.ExistsByUsernameAsync(username,cancellationToken);
        }

        public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken)
            => await _users.GetAllAsync(cancellationToken);

        public async Task<PagedResult<UserDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken)
            => await _users.GetPagedAsync(paging, cancellationToken);

        public async Task<UserDto?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(personId, nameof(personId));

            return await _users.GetByPersonIdAsync(personId,cancellationToken);
        }

        public async Task<UserDto?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(userId, nameof(userId));

            return await _users.GetByUserIdAsync(userId, cancellationToken);
        }

        //public async Task<UserCredentialsDto?> GetCredentialsByUsernameAsync(string username, CancellationToken cancellationToken)
        //  => await _users.GetCredentialsByUsernameAsync(username, cancellationToken);
    
        //public async Task<UserClaimsDto?> GetClaimsByUsernameAsync(string username, CancellationToken cancellationToken)
        //  => await _users.GetClaimsByUsernameAsync(username, cancellationToken);
        

        public async Task <UserDetailedDto?> GetDetailedByPersonIdAsync(int personId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(personId, nameof(personId));

            var user = await _users.GetByPersonIdAsync(personId, cancellationToken);

            if (user == null)
                return null;

            return await Enrich(user);
        }

        public async Task <UserDetailedDto?> GetDetailedByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(userId, nameof(userId));

            var user = await _users.GetByUserIdAsync(userId, cancellationToken);

            if (user == null)
                return null;

            return await Enrich(user);
        }

        public async Task <UserDto> UpdateAsync(int userId, UpdateUserDto dto, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(userId, nameof(userId));
            Guard.AgainstNull(dto, nameof(dto));

            ValidateWrite(dto);

            
            return await _users.UpdateAsync(userId, dto, cancellationToken);
        }


        // Helpers     
        private static bool HasComplexity(string password)
        {
            return password.Any(char.IsUpper)
                && password.Any(char.IsLower)
                && password.Any(char.IsDigit)
                && password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        private async Task< UserDetailedDto >Enrich(UserDto u)
        {
            var person = await _people.GetByIdAsync(u.PersonId);

            return new UserDetailedDto
            {
                PersonId = u.PersonId,
                UserId = u.UserId,
                UserName = u.UserName,
                IsActive = u.IsActive,
                Person = person
            };
        }

        private static void ValidateWrite(UpdateUserDto dto)
        {           
            if (string.IsNullOrWhiteSpace(dto.UserName))
                throw new ArgumentException("Username is required.");

            if (!Enum.IsDefined(typeof(enRole), dto.Role))
                throw new ArgumentException("Invalid role.");
        } 
        
        private static void ValidateWrite(CreateUserDto dto)
        {
            if (dto.PersonId < 1)
                throw new ArgumentException("Invalid personId.");

            if (string.IsNullOrWhiteSpace(dto.UserName))
                throw new ArgumentException("Username is required.");

            if (!Enum.IsDefined(typeof(enRole), dto.Role))
                throw new ArgumentException("Invalid role.");

            if (string.IsNullOrEmpty(dto.Password) || dto.Password.Length < 8 || !HasComplexity(dto.Password))
                throw new ArgumentException("Password must meet complexity requirements.");



        }

        
    }
}
