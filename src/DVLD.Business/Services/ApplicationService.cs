using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Exceptions;
using DVLD.Contracts.Requests.Application;
using DVLD.Business.Common.Validation;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;

namespace DVLD.Business.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _repo;
        private readonly ICurrentUserService _currentUser;

        public ApplicationService(IApplicationRepository repo, ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public async Task<ApplicationDto> CreateAsync(CreateApplicationRequest body, CancellationToken cancellationToken)
        {
            body.CreatedByUserId = _currentUser.UserId;

            ValidateCreate(body);
           
            var now = DateTime.UtcNow;

            return await _repo.AddAsync(body, now, cancellationToken);

        }

        public async Task DeleteAsync(int applicationId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(applicationId, nameof(applicationId));

            if (!await _repo.ExistsAsync(applicationId, cancellationToken))
                throw new KeyNotFoundException($"Application with id {applicationId} not found.");

            await _repo.DeleteAsync(applicationId, cancellationToken);
        }

        public async Task<ApplicationDto?> GetActiveAsync(int personId, int typeId, int? licenseClassId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(personId, nameof(personId));
            Guard.AgainstNonPositive(typeId, nameof(typeId));

            return licenseClassId.HasValue
                ? await _repo.GetActiveForLicenseClassAsync(personId, typeId, licenseClassId.Value, cancellationToken)
                : await _repo.GetActiveAsync(personId, typeId, cancellationToken);
        }

        public async Task<IReadOnlyList<ApplicationDto>> GetAllAsync(CancellationToken cancellationToken) 
            => await _repo.GetAllAsync(cancellationToken);

        public async Task<PagedResult<ApplicationDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken) 
            => await _repo.GetPagedAsync(paging, cancellationToken);

        public async Task<ApplicationDto?> GetByIdAsync(int applicationId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(applicationId, nameof(applicationId));
         
            return await _repo.GetByIdAsync(applicationId, cancellationToken);
        }

        public async Task<ApplicationDto> UpdateAsync(int applicationId, UpdateApplicationRequest body, CancellationToken cancellationToken)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Guard.AgainstNonPositive(applicationId, nameof(applicationId));
            
            if (!await _repo.ExistsAsync(applicationId, cancellationToken))
                throw new KeyNotFoundException($"Application with id {applicationId} not found.");

            ValidateUpdate(body);

            var updated = await _repo.UpdateAsync(applicationId, body, cancellationToken);
                
            return updated;
        }

        public async Task UpdateStatusAsync(int applicationId, short newStatus, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(applicationId, nameof(applicationId));
            Guard.AgainstInvalidEnum<enApplicationStatus>(newStatus, nameof(newStatus));


            if (!await _repo.ExistsAsync(applicationId, cancellationToken))
                throw new KeyNotFoundException("Application not found.");

            bool updated = await _repo.UpdateStatusAsync(applicationId, newStatus, cancellationToken);

            if (!updated)
                throw new ResourceConflictException("Cannot update the application status.");
        }

        // Helper Methods
        private static void ValidateCreate(CreateApplicationRequest body)
        {
            Guard.AgainstNonPositive(body.ApplicantPersonId, nameof(body.ApplicantPersonId));
            Guard.AgainstNonPositive(body.ApplicationTypeId, nameof(body.ApplicationTypeId));
            Guard.AgainstNonPositive(body.ApplicationStatus, nameof(body.ApplicationStatus));
            Guard.AgainstInvalidFees(body.PaidFees, nameof(body.PaidFees));
            Guard.AgainstInvalidEnum<enApplicationStatus>(body.ApplicationStatus, nameof(body.ApplicationStatus));
            Guard.AgainstInvalidEnum<enApplicationType>(body.ApplicationTypeId, nameof(body.ApplicationTypeId));
        }

        private static void ValidateUpdate(UpdateApplicationRequest body)
        {
            Guard.AgainstNonPositive(body.ApplicantPersonId, nameof(body.ApplicantPersonId));
            Guard.AgainstNonPositive(body.ApplicationTypeId, nameof(body.ApplicationTypeId));
            Guard.AgainstNonPositive(body.ApplicationStatus, nameof(body.ApplicationStatus));
            Guard.AgainstNonPositive(body.CreatedByUserId, nameof(body.CreatedByUserId));
            Guard.AgainstInvalidFees(body.PaidFees, nameof(body.PaidFees));
            Guard.AgainstInvalidEnum<enApplicationStatus>(body.ApplicationStatus, nameof(body.ApplicationStatus));
            Guard.AgainstInvalidEnum<enApplicationType>(body.ApplicationTypeId, nameof(body.ApplicationTypeId));

        }

    }
}
