using DVLD.Contracts.Common.Enums;

namespace DVLD.Contracts.Dtos.User
{
    public class UserDto
    {
        public int UserId { get; set; }
        public int PersonId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public enRole Role { get; set; }

        public UserDto() { }

        public UserDto(int userId, int personId, string userName, bool isActive, enRole role)
        {
            UserId = userId;
            PersonId = personId;
            UserName = userName;
            IsActive = isActive;
            Role = role;
        }
    }
}
