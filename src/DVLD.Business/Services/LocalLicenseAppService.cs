using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Dtos.LocalLicenseApp;
using DVLD.Contracts.Requests.Application;
using DVLD.Contracts.Requests.LocalLicenseApp;
using DVLD.Business.Common.Validation;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;

namespace DVLD.Business.Services
{

    public class LocalLicenseAppService : ILocalLicenseAppService
    {
        private readonly ILocalLicenseAppRepository _localLicenseAppsRepository;
        private readonly IPersonRepository _peopleRepository;
        private readonly ILicenseClassRepository _licenseClassesRepository;
        private readonly IApplicationTypeRepository _applicationTypeRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICurrentUserService _currentUser;


        public LocalLicenseAppService(ILocalLicenseAppRepository localLicenseAppsRepository, IPersonRepository peopleRepository,
             ILicenseClassRepository licenseClassesRepository,IApplicationTypeRepository applicationTypeRepository, IApplicationRepository applicationRepository,
             ICurrentUserService currentUser)
        {
            _localLicenseAppsRepository = localLicenseAppsRepository;
            _peopleRepository = peopleRepository;
            _licenseClassesRepository = licenseClassesRepository;
            _applicationTypeRepository = applicationTypeRepository;
            _applicationRepository = applicationRepository;
            _currentUser = currentUser;
        }

        public async Task <LocalLicenseAppDto> CreateAsync(CreateLocalLicenseAppRequest body, CancellationToken ct)
        {
            Guard.AgainstNull(body, nameof(body));
            Guard.AgainstNonPositive(body.ApplicantPersonId, nameof(body.ApplicantPersonId));
            Guard.AgainstNonPositive(body.LicenseClassId, nameof(body.LicenseClassId));

            if (!await _peopleRepository.ExistsByIdAsync(body.ApplicantPersonId, ct))
                throw new ArgumentException($"Person with id {body.ApplicantPersonId} does not exist.");

            // Get app type and license class to calculate fees.
            var appTypeTask = _applicationTypeRepository.GetByIdAsync((int)enApplicationType.NewDrivingLicense,ct); // 1: New Local Driving License                
            var licenseClassTask = _licenseClassesRepository.GetByIdAsync(body.LicenseClassId, ct);
            
            await Task.WhenAll(appTypeTask,licenseClassTask);

            var appType = await appTypeTask
                ?? throw new InvalidOperationException("Application type not found.");

            var licenseClass = await licenseClassTask
                ?? throw new ArgumentException($"License class with id {body.LicenseClassId} does not exist.");

            decimal fees = CalculateTotalFees(appType.ApplicationTypeFees , licenseClass.ClassFees);           
            var now = DateTime.UtcNow;
            
            // Create application.
            var appDto = new CreateApplicationRequest
                (
                    body.ApplicantPersonId, appType.ApplicationTypeId, (byte)enApplicationStatus.New, fees, _currentUser.UserId
                );

            var newAppDto = await _applicationRepository.AddAsync(appDto, now, ct);

            // Create local driving license application.
            var localLicenseDto = CreateLocalLicenseDto(newAppDto, body.LicenseClassId);

            return await _localLicenseAppsRepository.CreateAsync(localLicenseDto, ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            Guard.AgainstNonPositive(id, nameof(id));

            if (!await _localLicenseAppsRepository.ExistsAsync(id, ct))
                throw new KeyNotFoundException($"Local license app with id {id} does not exist.");

            await _localLicenseAppsRepository.DeleteAsync(id, ct);
        }

        public async Task <bool> ExistsAsync(int id, CancellationToken ct)
        {
            Guard.AgainstNonPositive(id, nameof(id));           
            return await _localLicenseAppsRepository.ExistsAsync(id, ct);
        }

        public async Task<IReadOnlyList<LocalLicenseViewDto>> GetAllAsync(CancellationToken ct) 
            => await _localLicenseAppsRepository.GetAllAsync(ct);

        public async Task<PagedResult<LocalLicenseViewDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct)
            => await _localLicenseAppsRepository.GetPagedAsync(paging, ct);

        public async Task <LocalLicenseAppDto?> GetByApplicationIdAsync(int applicationId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(applicationId, nameof(applicationId));
            return await _localLicenseAppsRepository.GetByApplicationIdAsync(applicationId, ct);

        }

        public async Task <LocalLicenseAppDto?> GetByLocalLicenseAppIdAsync(int localLicenseAppId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(localLicenseAppId, nameof(localLicenseAppId));
            return await _localLicenseAppsRepository.GetByLocalLicenseAppIdAsync(localLicenseAppId, ct);
        }

        public async Task<byte> GetTestAttemptCountAsync(int localLicenseAppId, int testTypeId, CancellationToken ct)
        {
            await CheckAppIdAndTestTypeId(localLicenseAppId, testTypeId, ct);
            return await _localLicenseAppsRepository.GetTestAttemptCountAsync(localLicenseAppId, testTypeId, ct);
        }

        public async Task <bool> HasActiveTestAppointmentAsync(int localLicenseAppId, int testTypeId, CancellationToken ct)
        {
            await CheckAppIdAndTestTypeId(localLicenseAppId, testTypeId, ct);
            return await _localLicenseAppsRepository.HasActiveTestAppointmentAsync(localLicenseAppId, testTypeId, ct);
        }

        public async Task <bool> HasAttendedTestAsync(int localLicenseAppId, int testTypeId, CancellationToken ct)
        {
            await CheckAppIdAndTestTypeId(localLicenseAppId, testTypeId, ct);
            return await _localLicenseAppsRepository.HasAttendedTestAsync(localLicenseAppId, testTypeId, ct);  
        }

        public async Task <bool> HasPassedTestAsync(int localLicenseAppId, int testTypeId, CancellationToken ct)
        {
            await CheckAppIdAndTestTypeId(localLicenseAppId, testTypeId, ct);
            return await _localLicenseAppsRepository.HasPassedTestAsync(localLicenseAppId, testTypeId, ct);
        }

        public async Task <LocalLicenseAppDto> UpdateAsync(int id, UpdateLocalLicenseAppRequest body, CancellationToken ct)
        {
            
            Guard.AgainstNonPositive(id, nameof(id));
            Guard.AgainstNull(body, nameof(body));
            Guard.AgainstInvalidEnum<enApplicationStatus>(body.ApplicationStatus, nameof(body.ApplicationStatus));

            var localLicenseDto = await _localLicenseAppsRepository.GetByLocalLicenseAppIdAsync(id, ct);

            if (localLicenseDto == null)
                throw new KeyNotFoundException($"Local license app with id {id} not found.");

            //localLicenseDto.ApplicationStatus = body.ApplicationStatus;

            var updated = await _localLicenseAppsRepository.UpdateAsync(id, localLicenseDto, ct);

            if(updated == null)
                throw new InvalidOperationException("Update failed.");

            return updated;
        }

        
        //Helpers
        private async Task CheckAppIdAndTestTypeId(int localLicenseAppId, int testTypeId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(localLicenseAppId, nameof(localLicenseAppId));
            Guard.AgainstInvalidEnum<enTestType>(testTypeId, nameof(testTypeId));

            if (!await _localLicenseAppsRepository.ExistsAsync(localLicenseAppId, ct))
                throw new KeyNotFoundException($"Local license app with id {localLicenseAppId} not found.");
        }

        private static CreateLocalLicenseAppDto CreateLocalLicenseDto(ApplicationDto appDto, int licenseClassId)
            => new CreateLocalLicenseAppDto
            (
                   appDto.ApplicationId, licenseClassId, appDto.ApplicantPersonId, appDto.ApplicationDate, appDto.ApplicationTypeId,
                   appDto.ApplicationStatus, appDto.LastStatusDate, appDto.PaidFees, appDto.CreatedByUserId
            );

        private static decimal CalculateTotalFees(decimal appTypeFees, decimal classFees)
            => appTypeFees + classFees;


    }
}
