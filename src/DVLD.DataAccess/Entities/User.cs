using DVLD.Contracts.Common.Enums;

namespace DVLD.DataAccess.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public int PersonId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public enRole Role { get; set; } = enRole.Viewer;

        public Person Person { get; set; } = null!;

    }
}
