using DVLD.Contracts.Dtos.User;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class UserMappingExtensions
    {
        public static IQueryable<UserDto> ProjectToDto(this IQueryable<User> query)
        => query.Select(u => new UserDto
        (
            u.UserId,
            u.PersonId,
            u.UserName,
            u.IsActive,
            u.Role
        ));

        public static UserDto ToDto(this User entity)
            => new UserDto
            {
                UserId = entity.UserId,
                PersonId = entity.PersonId,
                UserName = entity.UserName,
                IsActive = entity.IsActive,
                Role = entity.Role
            };
    }
}
