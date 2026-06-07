
using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.InternationalLicense;
using DVLD.Contracts.Requests.Application;
using DVLD.Contracts.Requests.InternationalLicense;
using DVLD.Business.Common.Validation;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;

namespace DVLD.Business.Services
{
    public class InternationalLicenseService : IInternationalLicenseService
    {
        private readonly ILocalLicenseAppRepository _localLicenseAppRepository;
        private readonly IInternationalLicenseRepository _internationalLicenseRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IApplicationTypeRepository _applicationTypeRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICurrentUserService _currentUser;

        public InternationalLicenseService(ILocalLicenseAppRepository localLicenseAppRepository, IInternationalLicenseRepository internationalLicenseRepository,
            IDriverRepository driverRepository, IApplicationTypeRepository applicationTypeRepository, IApplicationRepository applicationRepository,
            ICurrentUserService currentUser)
        {
            _localLicenseAppRepository = localLicenseAppRepository;
            _internationalLicenseRepository = internationalLicenseRepository;
            _driverRepository = driverRepository;
            _applicationTypeRepository = applicationTypeRepository;
            _applicationRepository = applicationRepository;
            _currentUser = currentUser;
        }

        public async Task<InternationalLicenseDto> CreateAsync(CreateInternationalLicenseRequest request, CancellationToken ct)
        {
            // Validation
            Guard.AgainstNull(request, nameof(request));
            Guard.AgainstNonPositive(request.IssuedUsingLocalLicenseId, nameof(request.IssuedUsingLocalLicenseId));
            Guard.AgainstNonPositive(request.DriverId, nameof(request.DriverId));

            var driver = await _driverRepository.GetByDriverIdAsync(request.DriverId, ct);
            if (driver == null)
                throw new ArgumentException($"There is no driver found with id {request.DriverId}");

            var localDrivingLicense = await _localLicenseAppRepository.GetByLocalLicenseAppIdAsync(request.IssuedUsingLocalLicenseId, ct);         
            if (localDrivingLicense == null)
                throw new ArgumentException("There is no local driving license found to create an internationl license.",
                    nameof(request.IssuedUsingLocalLicenseId));

            if (localDrivingLicense.LicenseClassId != (int)enLicenseClasses.OrdinaryDriving)
                throw new ArgumentException("International licenses issued by only ordinary driving license.");
                   
            if (await _internationalLicenseRepository.GetActiveByDriverIdAsync(request.DriverId, ct) != null)
                throw new ArgumentException($"There is an active international license for driver with id {request.DriverId}",
                    nameof(request.DriverId));

            var appType = await _applicationTypeRepository.GetByIdAsync((int)enApplicationType.NewInternationalLicense, ct)        
                ?? throw new InvalidOperationException("Failed to load application type info.");

            decimal fees = appType.ApplicationTypeFees;
            var now = DateTime.UtcNow;

            //Create Application
            var applicationDto = new CreateApplicationRequest
                (                 
                    driver.PersonId,
                    (int)enApplicationType.NewInternationalLicense,
                    (int)enApplicationStatus.Completed,
                    fees,
                    _currentUser.UserId
                );

            var createdApplicationDto = await _applicationRepository.AddAsync(applicationDto, now, ct);
            int applicationId = createdApplicationDto.ApplicationId;

            if (applicationId == -1)
                throw new InvalidOperationException("Failed to create application for international license.");

            //Create international license
            var internationalLicenceDto = new InternationalLicenseDto
                (
                    0,
                    applicationId,
                    driver.DriverId,
                    request.IssuedUsingLocalLicenseId,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddYears(1),
                    true,
                    _currentUser.UserId
                );

            return await _internationalLicenseRepository.CreateAsync(internationalLicenceDto, ct);
        }

        public async Task <InternationalLicenseDto?> GetActiveByDriverIdAsync(int driverId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(driverId, nameof(driverId));
                 
            return await _internationalLicenseRepository.GetActiveByDriverIdAsync(driverId, ct);
        }

        public async Task <IReadOnlyList<InternationalLicenseDto>> GetAllAsync(CancellationToken ct) 
            => await _internationalLicenseRepository.GetAllAsync(ct);  

        public async Task <PagedResult<InternationalLicenseDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct) 
            => await _internationalLicenseRepository.GetPagedAsync(paging, ct);

        public async Task <IReadOnlyList<InternationalLicenseDto>> GetAllDriverLicensesAsync(int driverId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(driverId, nameof(driverId));
            
            return await _internationalLicenseRepository.GetAllDriverLicensesAsync(driverId, ct);

        }

        public async Task <PagedResult<InternationalLicenseDto>> GetPagedDriverLicensesAsync(int driverId, PaginationParams paging, CancellationToken ct)
        {
            Guard.AgainstNonPositive(driverId, nameof(driverId));

            return await _internationalLicenseRepository.GetPagedDriverLicensesAsync(driverId, paging, ct);
        }
        
        public async Task <InternationalLicenseDto?> GetByIdAsync(int internationalLicenseId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(internationalLicenseId, nameof(internationalLicenseId));

            return await _internationalLicenseRepository.GetByIdAsync(internationalLicenseId, ct);
        }

        public async Task <InternationalLicenseDto> UpdateAsync(int internationalLicenseId, InternationalLicenseDto request, CancellationToken ct)
        {
            Guard.AgainstNull(request, nameof(request));
            Guard.AgainstNonPositive(internationalLicenseId, nameof(internationalLicenseId));
                                    
            var updated = await _internationalLicenseRepository.UpdateAsync(internationalLicenseId, request, ct);
       
            if (updated == null)
                throw new InvalidOperationException("International license couldn't be updated.");

            return updated;  
        }
    }
}
