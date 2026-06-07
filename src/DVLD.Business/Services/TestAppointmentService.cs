using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Dtos.TestAppointment;
using DVLD.Contracts.Exceptions;
using DVLD.Contracts.Requests.Application;
using DVLD.Contracts.Requests.TestAppointment;
using DVLD.Business.Common.Validation;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;

namespace DVLD.Business.Services
{
    public class TestAppointmentService : ITestAppointmentService
    {
        private readonly ITestAppointmentRepository _testAppointmentRepository;
        private readonly ILocalLicenseAppRepository _localLicenseAppRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationTypeRepository _applicationTypeRepository;
        private readonly ITestTypeRepository _testTypeRepository;
        private readonly ICurrentUserService _currentUser;

        public TestAppointmentService(ITestAppointmentRepository testAppointmentRepository, ILocalLicenseAppRepository localLicenseAppRepository,
            IApplicationRepository applicationRepository, IApplicationTypeRepository applicationTypeRepository, ITestTypeRepository testTypeRepository,
            ICurrentUserService currentUser)
        {
            _testAppointmentRepository = testAppointmentRepository;
            _localLicenseAppRepository = localLicenseAppRepository;
            _applicationRepository = applicationRepository;
            _applicationTypeRepository = applicationTypeRepository;
            _testTypeRepository = testTypeRepository;
            _currentUser = currentUser;
        }

        public async Task <TestAppointmentDto> CreateAsync(CreateTestAppointmentRequest request, CancellationToken ct )
        {
            await _ValidateWrite(request);

            if (await _localLicenseAppRepository.HasActiveTestAppointmentAsync(request.LocalDrivingLicenseApplicationId, request.TestTypeId, ct))
                throw new ArgumentException("He has an active appointment for this test, You cannot add new appointment");

            await _ValidateTestSequence(request, ct);

            int? newRetakeAppId = null;
            decimal retakeAppFees = 0;

            if (await _RequiresRetakeAsync(request))
                (newRetakeAppId, retakeAppFees) = await _CreateRetakeTestApplicationId(request);
           
            var testType = await _testTypeRepository.GetByIdAsync(request.TestTypeId)??
                throw new InvalidOperationException("Test type not found.");

            decimal testFees = testType.Fees;

            var testAppointmentDto = new TestAppointmentDto
            (
                0,
                request.TestTypeId,
                request.LocalDrivingLicenseApplicationId,
                request.AppointmentDate,
                retakeAppFees + testFees,
                _currentUser.UserId,
                false,
                newRetakeAppId
            );

            var created = await _testAppointmentRepository.CreateAsync(testAppointmentDto, ct);

            return created;
        }

        public async Task<TestAppointmentDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            Guard.AgainstNonPositive(id, nameof(id));
            return await _testAppointmentRepository.GetByIdAsync(id, ct);
        }

        public async Task <IReadOnlyList<TestAppointmentViewDto>> GetAllAsync(CancellationToken ct) 
            => await _testAppointmentRepository.GetAllAsync(ct);

        public async Task <PagedResult<TestAppointmentViewDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct) 
            => await _testAppointmentRepository.GetPagedAsync(paging, ct);

        public async Task <IReadOnlyList<TestAppointmentByTestTypeDto>> GetAllByTestTypeAsync(int localLicenseAppId, int testTypeId, CancellationToken ct)
        {
            await _ValidateLicenseAppAndTestType(localLicenseAppId, testTypeId, ct);
            return await _testAppointmentRepository.GetAllByTestTypeAsync(localLicenseAppId, testTypeId, ct);
        }

        public async Task <PagedResult<TestAppointmentByTestTypeDto>> GetPagedByTestTypeAsync(
            int localLicenseAppId, int testTypeId, PaginationParams paging, CancellationToken ct)
        {
            await _ValidateLicenseAppAndTestType(localLicenseAppId, testTypeId, ct);
            return await _testAppointmentRepository.GetPagedByTestTypeAsync(localLicenseAppId, testTypeId, paging, ct);
        }

        public async Task<TestAppointmentDto?> GetLastAsync(int localLicenseAppId, int testTypeId, CancellationToken ct)
        {
            await _ValidateLicenseAppAndTestType(localLicenseAppId, testTypeId, ct);
            return await _testAppointmentRepository.GetLastAsync(localLicenseAppId, testTypeId, ct);
        }

        public async Task<TestDto?> GetTestAsync(int testAppointmentId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(testAppointmentId, nameof(testAppointmentId));
            return await _testAppointmentRepository.GetTestAsync(testAppointmentId, ct);
        }

        public async Task<TestAppointmentDto> UpdateAsync(int testAppointmentId, UpdateTestAppointmentRequest request, CancellationToken ct)
        {
            // 1. Validate input
            if (request.AppointmentDate < DateTime.UtcNow)
                throw new ArgumentException("Appointment date must be in the future.", nameof(request.AppointmentDate));

            Guard.AgainstNonPositive(testAppointmentId, nameof(testAppointmentId));

            // 2. Fetch existing appointment
            var existing = await _testAppointmentRepository.GetByIdAsync(testAppointmentId, ct);

            if (existing == null)
                throw new KeyNotFoundException($"Test appointment with id {testAppointmentId} not found.");

            // 3. Check business rules
            if (existing.IsLocked)
                throw new ResourceConflictException("Cannot update a locked appointment.");

            // 4. Apply changes
            existing.AppointmentDate = request.AppointmentDate;

            // 5. Persist
            var updated = await _testAppointmentRepository.UpdateAsync(testAppointmentId, existing, ct)            
                ?? throw new InvalidOperationException($"Failed to updated test appointment with id {testAppointmentId}");

            return updated;

        }
       
        // helpers
        private async Task _ValidateWrite(CreateTestAppointmentRequest createTestAppointmentRequest, CancellationToken ct = default)
        {
            Guard.AgainstNull(createTestAppointmentRequest, nameof(createTestAppointmentRequest));
            Guard.AgainstNonPositive(createTestAppointmentRequest.LocalDrivingLicenseApplicationId, nameof(createTestAppointmentRequest.LocalDrivingLicenseApplicationId));
            Guard.AgainstInvalidEnum<enTestType>(createTestAppointmentRequest.TestTypeId, nameof(createTestAppointmentRequest.TestTypeId));
            
            if (!await _localLicenseAppRepository.ExistsAsync(createTestAppointmentRequest.LocalDrivingLicenseApplicationId))
                throw new ArgumentException($"Local license app with id {createTestAppointmentRequest.LocalDrivingLicenseApplicationId} does not exist.",
                    nameof(createTestAppointmentRequest.LocalDrivingLicenseApplicationId));

            if (createTestAppointmentRequest.AppointmentDate <= DateTime.UtcNow)
                throw new ArgumentException("Appointment date must be in the future.", nameof(createTestAppointmentRequest.AppointmentDate));
        }

        private async Task _ValidateLicenseAppAndTestType(int localLicenseAppId, int testTypeId, CancellationToken ct = default)
        {
            Guard.AgainstNonPositive(localLicenseAppId, nameof(localLicenseAppId));
            Guard.AgainstInvalidEnum<enTestType>(testTypeId, nameof(testTypeId));
            if (!await _localLicenseAppRepository.ExistsAsync(localLicenseAppId, ct))
                throw new ArgumentException($"Local license app with id {localLicenseAppId} does not exist.", nameof(localLicenseAppId));
        }

        private async Task _ValidateTestSequence(CreateTestAppointmentRequest request, CancellationToken ct = default)
        {
            switch (request.TestTypeId)
            {
                case (int)enTestType.VisionTest:
                    return;

                case (int)enTestType.WrittenTest:
                    if (!await _localLicenseAppRepository.HasPassedTestAsync(request.LocalDrivingLicenseApplicationId, (int)enTestType.VisionTest))
                        throw new ArgumentException("Cannot Sechule, Vision Test must be passed first");
                    return;

                case (int)enTestType.StreetTest:
                    if (!await _localLicenseAppRepository.HasPassedTestAsync(request.LocalDrivingLicenseApplicationId, (int)enTestType.WrittenTest))
                        throw new ArgumentException("Cannot Sechule, Written Test must be passed first");
                    return;

                default:
                    throw new ArgumentException($"Invalid test type ID: {request.TestTypeId}");
            }
        }

        private async Task<bool> _RequiresRetakeAsync(CreateTestAppointmentRequest request, CancellationToken ct = default)
        {
            var lastAppointment = await _testAppointmentRepository.GetLastAsync(request.LocalDrivingLicenseApplicationId, request.TestTypeId, ct);

            if (lastAppointment == null)
                return false;

            var lastTest = await _testAppointmentRepository.GetTestAsync(lastAppointment.TestAppointmentId, ct);

            if (lastTest == null)
                return false;

            if (lastTest.TestResult == true)
            {
                throw new ArgumentException("He passed this test before.");
            }

            return true;
        }

        private async Task<(int retakeAppId, decimal retakeFee)>_CreateRetakeTestApplicationId(CreateTestAppointmentRequest request, CancellationToken ct = default)
        {
            var localApp =await _localLicenseAppRepository.GetByLocalLicenseAppIdAsync(request.LocalDrivingLicenseApplicationId, ct)
                ?? throw new KeyNotFoundException("Local License application not found.");

            var app = await _applicationRepository.GetByIdAsync(localApp.ApplicationId, ct)
                ?? throw new KeyNotFoundException("Associated application not found.");

            var appType = await _applicationTypeRepository.GetByIdAsync((int)enApplicationType.RetakeTest, ct)
                ?? throw new KeyNotFoundException("Application type not found.");

            decimal retakeFee = appType.ApplicationTypeFees;


            var dto = new CreateApplicationRequest
                (
                    app.ApplicantPersonId,
                    (int)enApplicationType.RetakeTest,
                    (byte)enApplicationStatus.New,
                    retakeFee,
                    _currentUser.UserId
                );

            var created = await _applicationRepository.AddAsync(dto, DateTime.UtcNow, ct);
            int id = created.ApplicationId;

            if (id == -1)
                throw new InvalidOperationException("Failed to create retake test application.");

            return (id, retakeFee);
        }
    }
}