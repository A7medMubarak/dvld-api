namespace DVLD.Contracts.Dtos.LicenseClass
{
    public class LicenseClassDto
    {
        public int LicenseClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string ClassDescription { get; set; } = string.Empty;
        public byte MinimumAllowedAge { get; set; }
        public byte DefaultValidityLength { get; set; }
        public decimal ClassFees { get; set; }

        public LicenseClassDto() { }

        public LicenseClassDto(int licenseClassId, string className, string classDescription,
            byte minimumAllowedAge, byte defaultValidityLength, decimal classFees)
        {
            LicenseClassId = licenseClassId;
            ClassName = className;
            ClassDescription = classDescription;
            MinimumAllowedAge = minimumAllowedAge;
            DefaultValidityLength = defaultValidityLength;
            ClassFees = classFees;
        }
    }
}
