namespace DVLD.Contracts.Dtos.Auth
{
    public class UserCredentialsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
