using DVLD.Contracts.Dtos.Person;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class PersonMappingExtensions
    {
        public static IQueryable<PersonDto> ProjectToDto(this IQueryable<Person> query)
        => query.Select(p => new PersonDto
        {
            PersonId = p.PersonId,
            FirstName = p.FirstName,
            SecondName = p.SecondName,
            ThirdName = p.ThirdName,
            LastName = p.LastName,
            DateOfBirth = p.DateOfBirth,
            Address = p.Address,
            Phone = p.Phone,
            Gender = (short)p.Gender,
            Email = p.Email,
            NationalityCountryId = p.NationalityCountryId,
            NationalNo = p.NationalNo,
            ImagePath = p.ImagePath,
            CreatedByUserId = p.CreatedByUserId
        });

        public static PersonDto ToDto(this Person entity)
            => new PersonDto
            {
                PersonId = entity.PersonId,
                FirstName = entity.FirstName,
                SecondName = entity.SecondName,
                ThirdName = entity.ThirdName,
                LastName = entity.LastName,
                DateOfBirth = entity.DateOfBirth,
                Address = entity.Address,
                Phone = entity.Phone,
                Gender = (short)entity.Gender,
                Email = entity.Email,
                NationalityCountryId = entity.NationalityCountryId,
                NationalNo = entity.NationalNo,
                ImagePath = entity.ImagePath,
                CreatedByUserId = entity.CreatedByUserId
            };
    }
}
