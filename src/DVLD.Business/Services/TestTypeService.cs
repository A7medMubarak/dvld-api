using DVLD.Contracts.Dtos.TestType;
using DVLD.Contracts.Requests.TestType;
using DVLD.Business.Common.Validation;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;

namespace DVLD.Business.Services
{
    public class TestTypeService : ITestTypeService
    {
        private readonly ITestTypeRepository _repo;

        public TestTypeService(ITestTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task <IReadOnlyList<TestTypeDto>> GetAllAsync(CancellationToken ct) 
            => await _repo.GetAllAsync(ct);

        public async Task <TestTypeDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            Guard.AgainstNonPositive(id, nameof(id));

            return await _repo.GetByIdAsync(id, ct);

        }

        public async Task <TestTypeDto> CreateAsync(CreateTestTypeRequest body, CancellationToken ct)
        {
            ValidateInput(body);

            var Dto = ToDto(body);

            var created = await _repo.CreateAsync(Dto, ct);
           
            return created;
        }

        public async Task <TestTypeDto> UpdateAsync(int id, UpdateTestTypeRequest body, CancellationToken ct)
        {
            Guard.AgainstNonPositive(id, nameof(id));

            ValidateInput(body);

            var Dto = ToDto(body, id);

            var updated = await _repo.UpdateAsync(id, Dto, ct);

            return updated;
        }

        private static void ValidateInput(TestTypeWriteRequest body)
        {
            Guard.AgainstNull(body, nameof(body));
            Guard.AgainstNullOrWhiteSpace(body.Title, nameof(body.Title));
            Guard.AgainstNullOrWhiteSpace(body.Description, nameof(body.Description));
            Guard.AgainstInvalidFees(body.Fees, nameof(body.Fees));
        }

        private static TestTypeDto ToDto(TestTypeWriteRequest body, int testTypeId = 0)
            => new TestTypeDto(testTypeId, body.Title, body.Description, body.Fees);
    }
}
