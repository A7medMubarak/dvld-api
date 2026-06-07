using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Test;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface ITestRepository
    {
        // Queries
        Task <TestDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task <IReadOnlyList<TestDto>> GetAllAsync(CancellationToken ct = default);

        Task<PagedResult<TestDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default);

        Task <TestWithApplicantDto?> GetLatestAsync(int personId, int licenseClassId, int testTypeId, CancellationToken ct = default);

        Task <int> GetPassedCountAsync(int localDrivingLicenseApp, CancellationToken ct = default);

        // Commands
        Task <TestDto> CreateAsync(TestDto testDTO, CancellationToken ct = default);

        Task <TestDto> UpdateAsync(int id, TestDto testDTO, CancellationToken ct = default);   
    }
}
