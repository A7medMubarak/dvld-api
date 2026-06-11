using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.License;
using DVLD.Contracts.Requests.License;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/licenses")]
    [ApiController]
    public class LicensesController : ControllerBase
    {
        private readonly ILicenseService _service;

        public LicensesController(ILicenseService service)
        {
            _service = service; 
        }


        [HttpGet("{id:int}", Name = "GetLicenseById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<LicenseDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var license = await _service.GetByIdAsync(id, ct);

            if (license == null)
                return NotFound($"License with ID {id} not found.");

            return Ok(license);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<PagedResult<LicenseDto>>> GetAllAsync([FromQuery] PaginationParams paging, CancellationToken ct = default)
            => Ok(await _service.GetPagedAsync(paging, ct));


        [HttpGet("driver/{driverId:int}/licenses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<PagedResult<DriverLicensesDto>>> GetDriverLicensesAsync(
            int driverId, [FromQuery] PaginationParams paging, CancellationToken ct = default)
        {
            var licensesList = await _service.GetPagedDriverLicensesAsync(driverId, paging, ct);
            return Ok(licensesList);
        }


        [HttpGet("person/{personId:int}/active-license/{licenseClassId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<LicenseDto>> GetActiveLicenseForPersonAsync(int personId, int licenseClassId, CancellationToken ct = default)
        {
            var license = await _service.GetActiveLicenseForPersonAsync(personId, licenseClassId, ct);

            if (license == null)
                return NotFound("No active license found.");

            return Ok(license);
        }

        [Authorize(Roles = "Officer,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<LicenseDto>> CreateAsync([FromBody]CreateLicenseRequest request, CancellationToken ct = default)
        {
            var created = await _service.CreateAsync(request, ct);
            return CreatedAtRoute("GetLicenseById", new { id = created.LicenseId }, created);
        }

        [Authorize(Roles = "Officer,Admin")]
        [HttpPut("{licenseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<LicenseDto>> UpdateAsync(int licenseId, [FromBody]UpdateLicenseRequest request, CancellationToken ct = default)
        {
            var updated = await _service.UpdateAsync(licenseId, request, ct);
            return Ok(updated);
        }

        [Authorize(Roles = "Officer,Admin")]
        [HttpPatch("deactivate/{licenseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeactivateAsync(int licenseId, CancellationToken ct = default)
        {
            await _service.DeactivateAsync(licenseId, ct);
            return NoContent();
        }
    }
}
