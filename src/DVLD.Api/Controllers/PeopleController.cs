using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Person;
using DVLD.Contracts.Requests.Person;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/people")]
    [ApiController]

    public class PeopleController : ControllerBase
    {
        private readonly IPersonService _service;

        public PeopleController(IPersonService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetPersonById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<PersonDetailedDto>> GetPersonByIdAsync(int id, CancellationToken ct = default)
        {
            var person = await _service.GetDetailsByIdAsync(id, ct);

            if (person == null)
                return NotFound();

            return Ok(person);
        }

        [AllowAnonymous]
        [HttpGet("by-national-no")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<PersonDetailedDto>> GetPersonByNationalNoAsync([FromQuery] string nationalNo, CancellationToken ct = default)
        {
            var person = await _service.GetDetailsByNationalNoAsync(nationalNo, ct);

            if (person == null)
                return NotFound("No Person Found");

            return Ok(person);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<PagedResult<PersonDto>>> GetAllPeopleAsync([FromQuery] PaginationParams paging, CancellationToken ct = default)
            => Ok(await _service.GetPagedAsync(paging, ct));

        [Authorize(Roles = "Officer,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<PersonDto>> AddNewPersonAsync([FromBody] CreatePersonRequest body, CancellationToken ct = default)
        {
            var created = await _service.CreateAsync(body, ct);

            return CreatedAtRoute("GetPersonById", new { id = created.PersonId }, created);
        }

        [Authorize(Roles = "Officer,Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> UpdatePersonAsync(int id, [FromBody] UpdatePersonRequest body, CancellationToken ct = default)
        {
            var updated = await _service.UpdateAsync(id, body, ct);
            return Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeletePersonAsync(int id, CancellationToken ct = default)
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
