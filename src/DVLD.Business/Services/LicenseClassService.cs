using DVLD.Contracts.Dtos.LicenseClass;
using DVLD.Contracts.Requests.LicenseClass;
using DVLD.Business.Common.Validation;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;

namespace DVLD.Business.Services
{
    public class LicenseClassService : ILicenseClassService
    {
        private readonly ILicenseClassRepository _repo;

        public LicenseClassService(ILicenseClassRepository repo)
        {
            _repo = repo;
        }

        // Queries
        public async Task<LicenseClassDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(id, nameof(id));

            return await _repo.GetByIdAsync(id,cancellationToken);

        }
        
        public async Task <LicenseClassDto?> GetByClassNameAsync(string className, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(className))
                throw new ArgumentException("Class name is reuired", nameof(className));

            return await _repo.GetByClassNameAsync(className,cancellationToken);
        }
      
        public async Task<IReadOnlyList<LicenseClassDto>> GetAllAsync(CancellationToken cancellationToken)
            => await _repo.GetAllAsync(cancellationToken);

        // Commands
        public async Task <LicenseClassDto> CreateAsync(CreateLicenseClassRequest body, CancellationToken cancellationToken)
        {          
            ValidateWrite(body);

            var Dto = ToDto(body);

            return await _repo.CreateAsync(Dto, cancellationToken);
        }

        public async Task<LicenseClassDto> UpdateAsync(int id, UpdateLicenseClassRequest body, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(id, nameof(id));
            
            ValidateWrite(body);

            return await _repo.UpdateAsync(id, ToDto(body, id), cancellationToken);

        }

        // Helpers
        private static void ValidateWrite(LicenseClassWriteRequest body)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            if (string.IsNullOrWhiteSpace(body.ClassName))
                throw new ArgumentException("Class name is required.", nameof(body.ClassName));

            if (string.IsNullOrWhiteSpace(body.ClassDescription))
                throw new ArgumentException("Class description is required.", nameof(body.ClassDescription));
          
            Guard.AgainstInvalidFees(body.ClassFees, nameof(body.ClassFees));
           
            Guard.AgainstNonPositive(body.DefaultValidityLength, nameof(body.DefaultValidityLength));
            
            if (body.MinimumAllowedAge < 18)
                throw new ArgumentOutOfRangeException(nameof(body.MinimumAllowedAge), "Minimum allowed age must be at least 18.");
        }

        private static LicenseClassDto ToDto(LicenseClassWriteRequest body, int licenseClassId = 0) => new LicenseClassDto
            (
                licenseClassId,
                body.ClassName,
                body.ClassDescription,
                body.MinimumAllowedAge,
                body.DefaultValidityLength,
                body.ClassFees
            );

    }
}
