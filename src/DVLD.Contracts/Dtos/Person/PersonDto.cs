namespace DVLD.Contracts.Dtos.Person
{
    public class PersonDto
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string? ThirdName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string NationalNo { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public short Gender { get; set; }
        public string? Email { get; set; }
        public int NationalityCountryId { get; set; }
        public string? ImagePath { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
