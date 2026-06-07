namespace DVLD.Contracts.Requests.ApplicationType
{
    public abstract class WriteApplicationTypeRequest
    {
        public string ApplicationTypeTitle { get; set; } = string.Empty;
        public decimal ApplicationTypeFees { get; set; }
    }
}
