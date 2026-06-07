using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.DetainedLicense;
using DVLD.Contracts.Exceptions;
using DVLD.Contracts.Requests.Application;
using DVLD.Contracts.Requests.DetainedLicense;
using DVLD.Business.Common.Validation;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;

namespace DVLD.Business.Services
{
    public class DetainedLicenseService : IDetainedLicenseService
    {
        private readonly IDetainedLicenseRepository _detainedLicenseRepository;
        private readonly ILicenseRepository _licenseRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationTypeRepository _applicationTypeRepository;
        private readonly ICurrentUserService _currentUser;

        public DetainedLicenseService(IDetainedLicenseRepository detainedLicenseRepository, ILicenseRepository licenseRepository,
            IApplicationRepository applicationRepository, IApplicationTypeRepository applicationTypeRepository, ICurrentUserService currentUser)
        {
            _detainedLicenseRepository = detainedLicenseRepository;
            _licenseRepository = licenseRepository;
            _applicationRepository = applicationRepository;
            _applicationTypeRepository = applicationTypeRepository;
            _currentUser = currentUser;
        }

        public async Task<DetainedLicenseDto> CreateAsync(CreateDetainedLicenseRequest request, CancellationToken ct)
        {
            // validation
            Guard.AgainstNull(request, nameof(request));
            Guard.AgainstInvalidFees(request.FineFees, nameof(request.FineFees));
            Guard.AgainstNonPositive(request.LicenseId, nameof(request.LicenseId));

            var license = await _licenseRepository.GetByIdAsync(request.LicenseId, ct)
                ?? throw new ArgumentException($"License with id {request.LicenseId} does not exist.",nameof(request.LicenseId));

            if (await _detainedLicenseRepository.IsLicenseDetainedAsync(request.LicenseId, ct))
                throw new ResourceConflictException($"This license with id {request.LicenseId} is already detained.");

            // make new detained license
            var detainedLicenseDto = new DetainedLicenseDto
                (
                    0,
                    request.LicenseId,
                    DateTime.UtcNow,
                    request.FineFees,
                    _currentUser.UserId,
                    false,
                    null,
                    null,
                    null
                );

            return await _detainedLicenseRepository.CreateAsync(detainedLicenseDto, ct);
        }

        public async Task <IReadOnlyList<DetainedLicenseViewDto>> GetAllAsync(CancellationToken ct) 
            => await _detainedLicenseRepository.GetAllAsync(ct);

        public async Task <PagedResult<DetainedLicenseViewDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct) 
            => await _detainedLicenseRepository.GetPagedAsync(paging, ct);

        public async Task< DetainedLicenseDto?> GetByDetainIdAsync(int detainId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(detainId, nameof(detainId));
            
            return await _detainedLicenseRepository.GetByDetainIdAsync(detainId, ct);
        }

        public async Task<DetainedLicenseDto?> GetByLicenseIdAsync(int licenseId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(licenseId, nameof(licenseId));

            return await _detainedLicenseRepository.GetByLicenseIdAsync(licenseId, ct);
        }

        public async Task <bool> IsLicenseDetainedAsync(int licenseId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(licenseId, nameof(licenseId));

            if (!await _licenseRepository.ExistsAsync(licenseId, ct))
                throw new KeyNotFoundException($"License with id {licenseId} does not exist.");

            return await _detainedLicenseRepository.IsLicenseDetainedAsync(licenseId, ct);

        }

        public async Task<bool> ReleaseDetainedLicenseAsync(int detainId, CancellationToken ct)
        {
            int releasedByUserId = _currentUser.UserId;

            Guard.AgainstNonPositive(detainId, nameof(detainId));

            var detainedLicense = await _detainedLicenseRepository.GetByDetainIdAsync(detainId, ct)
                ?? throw new KeyNotFoundException($"Detained license with id {detainId} not found.");

            if (detainedLicense.IsReleased)
                throw new ResourceConflictException("License is released already");

            // make new released application
            var license = await _licenseRepository.GetByIdAsync(detainedLicense.LicenseId, ct)
                ?? throw new InvalidOperationException("Failed to load license info.");

            var app = await _applicationRepository.GetByIdAsync(license.ApplicationId, ct)
                ?? throw new InvalidOperationException($"Failed to load application info of license with id {license.LicenseId}.");
            
            var appType = await _applicationTypeRepository.GetByIdAsync((int)enApplicationType.ReleaseDetainedDrivingLicsense, ct)
                ?? throw new InvalidOperationException($"Failed to load application type info for releasing license.");
           
            int applicantPersonId = app.ApplicantPersonId;
            decimal releaseApplicationFees = appType.ApplicationTypeFees;
            var now = DateTime.UtcNow;

                var appDto = new CreateApplicationRequest
                (
                      applicantPersonId,
                      (int)enApplicationType.ReleaseDetainedDrivingLicsense,
                      (byte)enApplicationStatus.New,
                      releaseApplicationFees,
                      releasedByUserId
                );

            var releaseApplicationDto = await _applicationRepository.AddAsync(appDto, now, ct);
            int releaseApplicationId = releaseApplicationDto.ApplicationId;

            if (releaseApplicationId == -1)
                throw new InvalidOperationException("Failed to create released application.");

            bool released = await _detainedLicenseRepository.ReleaseDetainedLicenseAsync(detainId, releasedByUserId, releaseApplicationId, ct);

            if (!released)
                throw new InvalidOperationException($"Failed to release detained license with id {detainId}");

            return released;
        }
    }
}
