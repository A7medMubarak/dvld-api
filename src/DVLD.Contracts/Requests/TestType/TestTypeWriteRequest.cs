namespace DVLD.Contracts.Requests.TestType
{
    public abstract class TestTypeWriteRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Fees { get; set; }
    }
}
