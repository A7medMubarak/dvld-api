using DVLD.Contracts.Common.Enums;

namespace DVLD.Contracts.Dtos.Auth
{
    public class UserClaimsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public enRole Role { get; set; }
    }
}
