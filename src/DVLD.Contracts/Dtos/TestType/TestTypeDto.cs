namespace DVLD.Contracts.Dtos.TestType
{
    public class TestTypeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Fees { get; set; }

        public TestTypeDto() { }

        public TestTypeDto(int id, string title, string description, decimal fees)
        {
            Id = id;
            Title = title;
            Description = description;
            Fees = fees;
        }
    }
}
