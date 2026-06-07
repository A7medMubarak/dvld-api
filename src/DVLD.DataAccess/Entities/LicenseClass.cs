namespace DVLD.DataAccess.Entities
{
    public class LicenseClass
    {
        public int LicenseClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;              
        public string ClassDescription { get; set; } = string.Empty;
        public byte MinimumAllowedAge { get; set; }
        public byte DefaultValidityLength { get; set; }
        public decimal ClassFees { get; set; }
    }
}
