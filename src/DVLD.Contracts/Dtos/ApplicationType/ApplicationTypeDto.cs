namespace DVLD.Contracts.Dtos.ApplicationType
{
    public class ApplicationTypeDto
    {
        public int ApplicationTypeId { get; set; }
        public string ApplicationTypeTitle { get; set; } = string.Empty;
        public decimal ApplicationTypeFees { get; set; }

        public ApplicationTypeDto() { }

        public ApplicationTypeDto(int id, string title, decimal fees)
        {
            ApplicationTypeId = id;
            ApplicationTypeTitle = title;
            ApplicationTypeFees = fees;
        }
    }
}
