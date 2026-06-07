using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Person;

namespace DVLD.Contracts.Dtos.User
{
    public class UserDetailedDto
    {
        public int UserId { get; set; }
        public int PersonId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public enRole Role { get; set; }
        public PersonDto? Person { get; set; }
    }
}
