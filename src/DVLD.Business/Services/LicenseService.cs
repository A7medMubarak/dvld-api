using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.License;
using DVLD.Contracts.Requests.License;
using DVLD.Business.Common.Validation;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;

namespace DVLD.Business.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly ILicenseRepository _licensesRepo;
        private readonly ILicenseClassRepository _licenseClassesRepo;
        private readonly IPersonRepository _peopleRepo;
        private readonly ICurrentUserService _currentUser;

        public LicenseService(ILicenseRepository licenses, ILicenseClassRepository licenseClasses, IPersonRepository people, ICurrentUserService currentUser)
        {
            _licensesRepo = licenses;
            _licenseClassesRepo = licenseClasses;
            _peopleRepo = people;
            _currentUser = currentUser;
        }

        // Queries
        public async Task <LicenseDto?> GetByIdAsync(int licenseId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(licenseId, nameof(licenseId));

            return await _licensesRepo.GetByIdAsync(licenseId, ct);
        }

        public async Task<IReadOnlyList<LicenseDto>> GetAllAsync(CancellationToken ct)
            => await _licensesRepo.GetAllAsync(ct);

        public async Task<PagedResult<LicenseDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct)
            => await _licensesRepo.GetPagedAsync(paging, ct);

        public async Task <LicenseDto?> GetActiveLicenseForPersonAsync(int personId, int licenseClassId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(personId, nameof(personId));
            
            Guard.AgainstInvalidEnum<enLicenseClasses>(licenseClassId, nameof(licenseClassId));

            return await _licensesRepo.GetActiveLicenseForPersonAsync(personId, licenseClassId, ct);
        }

        public async Task<IReadOnlyList<DriverLicensesDto>> GetDriverLicensesAsync(int driverId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(driverId, nameof(driverId));

            return await _licensesRepo.GetDriverLicensesAsync(driverId,ct);
        }

        public async Task<PagedResult<DriverLicensesDto>> GetPagedDriverLicensesAsync(int driverId, PaginationParams paging, CancellationToken ct)
        {
            Guard.AgainstNonPositive(driverId, nameof(driverId));

            return await _licensesRepo.GetPagedDriverLicensesAsync(driverId, paging, ct);
        }

        // Commands
        public async Task <LicenseDto> CreateAsync(CreateLicenseRequest body, CancellationToken ct)
        {
            ValidateWrite(body);

            var ldto = ToDto(body);

            return await _licensesRepo.CreateAsync(ldto, ct);
        }

        public async Task<LicenseDto> UpdateAsync(int licenseId, UpdateLicenseRequest body, CancellationToken ct)
        {            
            Guard.AgainstNonPositive(licenseId, nameof(licenseId));

            ValidateWrite(body);
            
            var ldto = ToDto(body, licenseId);

            var updated = await _licensesRepo.UpdateAsync(licenseId, ldto, ct);
            
            return updated;
        }

        public async Task<bool>DeactivateAsync(int licenseId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(licenseId, nameof(licenseId));

            return await _licensesRepo.DeactivateAsync(licenseId,ct);
        }

        // Exists
        public async Task<bool> ExistsAsync(int licenseId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(licenseId, nameof(licenseId));

            return await _licensesRepo.ExistsAsync(licenseId,ct);
        }

        //Helper
        private LicenseDto ToDto(LicenseWriteRequest body, int licenseId = 0) 
            => new LicenseDto
            (
                licenseId,
                body.ApplicationId,
                body.DriverId,
                body.LicenseClassId,
                body.IssueDate,
                body.ExpirationDate,
                body.Notes,
                body.PaidFees,
                body.IsActive,
                (enIssueReason)body.IssueReason,
                _currentUser.UserId
            );
        
        private static void ValidateWrite(LicenseWriteRequest body)
        {
            Guard.AgainstNull(body, nameof(body));
            Guard.AgainstNonPositive(body.ApplicationId, nameof(body.ApplicationId));

            if (body.ExpirationDate <= body.IssueDate)
                throw new ArgumentException("Expiration date must be after issue date.", nameof(body.ExpirationDate));

            Guard.AgainstInvalidEnum<enIssueReason>(body.IssueReason, nameof(body.IssueReason));
            Guard.AgainstInvalidEnum<enLicenseClasses>(body.LicenseClassId, nameof(body.LicenseClassId));
        }
    }
}
