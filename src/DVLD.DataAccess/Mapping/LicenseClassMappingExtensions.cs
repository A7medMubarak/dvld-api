using DVLD.Contracts.Dtos.LicenseClass;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class LicenseClassMappingExtensions
    {
        public static IQueryable<LicenseClassDto> ProjectToDto(this IQueryable<LicenseClass> query)
        => query.Select(l => new LicenseClassDto
        (
            l.LicenseClassId,
            l.ClassName,
            l.ClassDescription,
            l.MinimumAllowedAge,
            l.DefaultValidityLength,
            l.ClassFees
        ));

        public static LicenseClass ToEntity(this LicenseClassDto licenseClassDto)
            => new LicenseClass
            {
                ClassName = licenseClassDto.ClassName,
                ClassDescription = licenseClassDto.ClassDescription,
                MinimumAllowedAge = licenseClassDto.MinimumAllowedAge,
                DefaultValidityLength = licenseClassDto.DefaultValidityLength,
                ClassFees = licenseClassDto.ClassFees
            };

        public static LicenseClassDto ToDto(this LicenseClass licenseClass)
           => new LicenseClassDto
           {
               LicenseClassId = licenseClass.LicenseClassId,
               ClassName = licenseClass.ClassName,
               ClassDescription = licenseClass.ClassDescription,
               MinimumAllowedAge = licenseClass.MinimumAllowedAge,
               DefaultValidityLength = licenseClass.DefaultValidityLength,
               ClassFees = licenseClass.ClassFees
           };
    }
}




