using DVLD.Contracts.Dtos.TestType;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface ITestTypeRepository
    {
        Task<TestTypeDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<TestTypeDto>> GetAllAsync(CancellationToken ct = default);

        Task <TestTypeDto> CreateAsync(TestTypeDto testTypeDto, CancellationToken ct = default);

        Task <TestTypeDto> UpdateAsync(int id, TestTypeDto testTypeDto, CancellationToken ct = default);
    }
}
