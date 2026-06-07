using DVLD.Contracts.Common.Enums;

namespace DVLD.DataAccess.Entities
{
    public class Person
    {
        public int PersonId { get; set; }
        public string NationalNo { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string? ThirdName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public enGender Gender { get; set; }
        public string? Email { get; set; }
        public int NationalityCountryId { get; set; }
        public string? ImagePath { get; set; }
        public int CreatedByUserId { get; set; }

        public Country Country { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;
        public Driver? Driver { get; set; }
        public User? User { get; set; }

        public string Fullname
           => string.Join(" ", new[] { FirstName, SecondName, ThirdName, LastName }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
