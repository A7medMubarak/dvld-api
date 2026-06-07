using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Auth;
using DVLD.Contracts.Dtos.User;
using DVLD.Contracts.Requests.User;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto?> GetByUserIdAsync(int userId,CancellationToken cancellationToken = default);
        Task<UserDto?> GetByPersonIdAsync(int personId,CancellationToken cancellationToken = default);
        Task<UserCredentialsDto?> GetCredentialsByUsernameAsync(string username,CancellationToken cancellationToken = default);
        Task<UserClaimsDto?> GetClaimsByUsernameAsync(string username, CancellationToken ct = default);

        Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PagedResult<UserDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default);

        Task<UserDto> AddAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
        Task<UserDto> UpdateAsync(int userId, UpdateUserDto dto, CancellationToken cancellationToken = default);
       
        Task DeleteAsync(int userId, CancellationToken cancellationToken = default);
      
        Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByPersonIdAsync(int personId,CancellationToken cancellationToken = default);
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);

        Task<bool >ChangePasswordAsync(int userId, string newPasswordHash, CancellationToken cancellationToken=default);


    }

}
