using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Requests.Test;
using DVLD.Business.Common.Validation;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using System.Reflection.Metadata.Ecma335;

namespace DVLD.Business.Services
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _tests;
        //private readonly ILicenseClassRepository _licenseClasses;
        private readonly IPersonRepository _people;
        private readonly ICurrentUserService _currentUser;
        //private readonly ILocalLicenseAppRepository _localLicenseApps;
        //private readonly ITestAppointmentRepository _appointments;

        public TestService(ITestRepository tests, ILicenseClassRepository licenseClasses, IPersonRepository people,
           ILocalLicenseAppRepository localLicenseApps, ITestAppointmentRepository appointments, ICurrentUserService currentUser)
        {
            _tests = tests;
            //_licenseClasses = licenseClasses;
            _people = people;
            _currentUser = currentUser;
            //_localLicenseApps = localLicenseApps;
            //_appointments = appointments;
        }

        // Queries
        public async Task <IReadOnlyList<TestDto>> GetAllAsync(CancellationToken ct) 
            => await _tests.GetAllAsync(ct);

        public async Task <PagedResult<TestDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct) 
            => await _tests.GetPagedAsync(paging, ct);

        public async Task <TestDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            Guard.AgainstNonPositive(id, nameof(id));

            return await _tests.GetByIdAsync(id,ct);
        }

        public async Task <TestWithApplicantDto?> GetLatestAsync(int personId, int licenseClassId, int testTypeId, CancellationToken ct)
        {
            // Validate IDs      
            Guard.AgainstNonPositive(personId, nameof(personId));
            Guard.AgainstNonPositive(licenseClassId, nameof(licenseClassId));
            Guard.AgainstInvalidEnum <enTestType> (testTypeId,nameof(testTypeId));

            // Check existence in DB
            if (!await _people.ExistsByIdAsync(personId, ct))
                throw new ArgumentException($"Person with id {personId} does not exist.", nameof(personId));
          
            return await _tests.GetLatestAsync(personId, licenseClassId, testTypeId, ct);

        }

        public async Task <int> GetPassedCountAsync(int localDrivingLicenseAppId, CancellationToken ct)
        {
            Guard.AgainstNonPositive(localDrivingLicenseAppId, nameof(localDrivingLicenseAppId));
           
            return await _tests.GetPassedCountAsync(localDrivingLicenseAppId, ct);
        }

        // Commands
        public async Task <TestDto> CreateAsync(CreateTestRequest body, CancellationToken ct)
        {
            ValidateWrite(body);

            var testDto = ToDto(body);

            return await _tests.CreateAsync(testDto, ct);  
        }

        public async Task <TestDto> UpdateAsync(int id, UpdateTestRequest body, CancellationToken ct)
        {
            Guard.AgainstNonPositive(id, nameof(id));
            
            ValidateWrite(body);
           
            var testDto = ToDto(body, id);

            return await _tests.UpdateAsync(id, testDto, ct);
        }

        // Helpers
        private static void ValidateWrite(TestWriteRequest body)
        {
            Guard.AgainstNull(body, nameof(body));
            Guard.AgainstNonPositive(body.TestAppointmentId, nameof(body.TestAppointmentId));
        }

        private TestDto ToDto(TestWriteRequest body, int testId = 0) => new TestDto
            (
                testId,
                body.TestAppointmentId,
                body.TestResult,
                body.Notes,
                _currentUser.UserId
            );
       
    }
}
