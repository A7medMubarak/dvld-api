using DVLD.Contracts.Common.Enums;

namespace DVLD.Contracts.Requests.User
{
    public class CreateUserRequest
    {
        public int PersonId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public enRole Role { get; set; }
    }
}
