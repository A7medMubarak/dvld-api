namespace DVLD.Contracts.Requests.LicenseClass
{
    public abstract class LicenseClassWriteRequest
    {
        public string ClassName { get; set; } = string.Empty;
        public string ClassDescription { get; set; } = string.Empty;
        public byte MinimumAllowedAge { get; set; }
        public byte DefaultValidityLength { get; set; }
        public decimal ClassFees { get; set; }
    }
}
