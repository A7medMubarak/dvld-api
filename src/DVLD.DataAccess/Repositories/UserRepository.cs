using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Auth;
using DVLD.Contracts.Dtos.User;
using DVLD.Contracts.Requests.User;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> AddAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
        {
            var entity = new User
            {
                PersonId = dto.PersonId,
                UserName = dto.UserName,
                PasswordHash = dto.Password, // Already hashed by service
                IsActive = dto.IsActive,
                Role = dto.Role
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDto();
        }

        //public async Task<bool> ChangePasswordAsync(int userId, string newPasswordHash, CancellationToken cancellationToken = default)
        //{
        //    var entity = await _context.Users.FindAsync(userId,cancellationToken);

        //    if (entity == null) return false;

        //    entity.PasswordHash = newPasswordHash;

        //    await _context.SaveChangesAsync(cancellationToken); 

        //    return true;  
        //}

        public async Task<bool> ChangePasswordAsync(int userId, string newPasswordHash, CancellationToken cancellationToken = default)
        {
            var rowsAffected = await _context.Users
               .Where(u => u.UserId == userId)
               .ExecuteUpdateAsync(setters => setters.SetProperty(u => u.PasswordHash, newPasswordHash), cancellationToken);
           
            return rowsAffected > 0;
        }
             
        public async Task DeleteAsync(int userId, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Users.FindAsync(userId, cancellationToken);

            if (entity == null) return;

            _context.Users.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);
        }
  
        public async Task<bool> ExistsByPersonIdAsync(int personId, CancellationToken cancellationToken = default)
            => await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.PersonId == personId, cancellationToken);

        public async Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
            => await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.UserId == userId, cancellationToken);

        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
            => await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.UserName == username, cancellationToken);

        public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.Users
                .AsNoTracking()
                .ProjectToDto()
                .ToListAsync(cancellationToken);

        public async Task<PagedResult<UserDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default)
            => await _context.Users
                .AsNoTracking()
                .ProjectToDto()
                .OrderBy(u => u.UserId)
                .ToPagedListAsync(paging, cancellationToken);

        public async Task<UserDto?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default)
            => await _context.Users
                .AsNoTracking()
                .Where(u => u.PersonId == personId)
                .ProjectToDto()
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<UserDto?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
            => await _context.Users
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .ProjectToDto()
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<UserCredentialsDto?> GetCredentialsByUsernameAsync(string username, CancellationToken cancellationToken = default)
           => await _context.Users
               .AsNoTracking()
               .Where(u => u.UserName == username)
               .Select(u => new UserCredentialsDto
               {
                   UserId = u.UserId,
                   UserName = u.UserName,
                   PasswordHash = u.PasswordHash,
                   IsActive = u.IsActive

               })
               .FirstOrDefaultAsync(cancellationToken);

        public async Task<UserClaimsDto?> GetClaimsByUsernameAsync(string username, CancellationToken cancellationToken = default)
            => await _context.Users
               .AsNoTracking()
               .Where(u => u.UserName == username)
               .Select(u => new UserClaimsDto
               {
                   UserId = u.UserId,
                   UserName = u.UserName,
                   Role = u.Role                   
               })
               .FirstOrDefaultAsync(cancellationToken);


        public async Task<UserDto> UpdateAsync(int userId, UpdateUserDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) 
                throw new ArgumentNullException(nameof(dto));

            var entity = await _context.Users.FindAsync(userId, cancellationToken)
                ?? throw new KeyNotFoundException($"User with id {userId} not found");

            entity.UserName = dto.UserName;
            entity.IsActive = dto.IsActive;
            entity.Role = dto.Role;

            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDto();
        }

    }
}
