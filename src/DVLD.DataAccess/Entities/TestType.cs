namespace DVLD.DataAccess.Entities
{
    public class TestType
    {
        public int TestTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Fees { get; set; }
    }
}
