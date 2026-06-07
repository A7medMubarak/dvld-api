using DVLD.Contracts.Dtos.ApplicationType;

namespace DVLD.DataAccess.Entities
{
    public class ApplicationType
    {
        public int ApplicationTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Fees { get; set; }

        
    }
}
