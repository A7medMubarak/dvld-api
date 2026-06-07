using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Dtos.TestAppointment;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface ITestAppointmentRepository
    {
        Task<TestAppointmentDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<TestAppointmentDto?> GetLastAsync(int localLicenseAppId, int testTypeId,CancellationToken ct = default);

        Task<IReadOnlyList<TestAppointmentViewDto>> GetAllAsync(CancellationToken ct = default);

        Task<PagedResult<TestAppointmentViewDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default);

        Task<IReadOnlyList<TestAppointmentByTestTypeDto>> GetAllByTestTypeAsync(int localLicenseAppId, int testTypeId,CancellationToken ct = default);

        Task<PagedResult<TestAppointmentByTestTypeDto>> GetPagedByTestTypeAsync(int localLicenseAppId, int testTypeId, PaginationParams paging, CancellationToken ct = default);

        Task<TestDto?> GetTestAsync(int testAppointmentId,CancellationToken ct = default);

        Task<TestAppointmentDto> CreateAsync(TestAppointmentDto testAppointmentDTO,CancellationToken ct = default);

        Task<TestAppointmentDto> UpdateAsync(int id, TestAppointmentDto testAppointmentDTO,CancellationToken ct = default);
    }
}
