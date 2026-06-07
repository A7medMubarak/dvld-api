using DVLD.Contracts.Common.Enums;

namespace DVLD.Contracts.Response.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public enRole Role { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
