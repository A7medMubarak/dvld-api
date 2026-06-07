using DVLD.Contracts.Dtos.TestType;
using DVLD.Contracts.Requests.TestType;

namespace DVLD.Business.Interfaces.Services
{
    public interface ITestTypeService
    {
        Task <TestTypeDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task <IReadOnlyList<TestTypeDto>> GetAllAsync(CancellationToken ct = default);
        
        Task <TestTypeDto> CreateAsync(CreateTestTypeRequest body, CancellationToken ct = default);

        Task <TestTypeDto> UpdateAsync(int id, UpdateTestTypeRequest body, CancellationToken ct = default);
    }
}
