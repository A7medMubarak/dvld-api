using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Dtos.TestType;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class TestTypeMappingExtensions
    {
        public static IQueryable<TestTypeDto> ProjectToDto(this IQueryable<TestType> query)
        => query.Select(t => new TestTypeDto
        (
            t.TestTypeId,
            t.Title,
            t.Description,
            t.Fees
        ));
       
        public static TestType ToEntity(this TestTypeDto testTypeDTO)
            => new TestType
            {
                Title = testTypeDTO.Title,
                Description = testTypeDTO.Description,
                Fees = testTypeDTO.Fees
            };

        public static TestTypeDto ToDto(this TestType testType)
           => new TestTypeDto
           {
               Id = testType.TestTypeId,
               Title = testType.Title,
               Description = testType.Description,
               Fees = testType.Fees
           };
    }
}





