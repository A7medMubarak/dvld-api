using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Requests.Test;

namespace DVLD.Business.Interfaces.Services
{
    public interface ITestService
    {
        // Queries
        Task <TestDto?> GetByIdAsync(int id,CancellationToken ct =default);

        Task<IReadOnlyList<TestDto>> GetAllAsync(CancellationToken ct = default);

        Task<PagedResult<TestDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default);

        Task <TestWithApplicantDto?> GetLatestAsync(int personId, int licenseClassId, int testTypeId, CancellationToken ct = default);

        Task <int> GetPassedCountAsync(int localDrivingLicenseApp, CancellationToken ct = default);

        // Commands
        Task<TestDto> CreateAsync(CreateTestRequest request, CancellationToken ct = default);

        Task <TestDto> UpdateAsync(int id, UpdateTestRequest request, CancellationToken ct = default);
    }
}
