using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.InternationalLicense;
using DVLD.Contracts.Requests.InternationalLicense;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/international-licenses")]
    [ApiController]
    public class InternationalLicensesController : ControllerBase
    {
        private readonly IInternationalLicenseService _internationalLicenseService;

        public InternationalLicensesController(IInternationalLicenseService internationalLicenseService)
        {
            _internationalLicenseService = internationalLicenseService;
        }


        [HttpGet("{id}", Name = "GetInternationalLicenseById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<InternationalLicenseDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var found = await _internationalLicenseService.GetByIdAsync(id, ct);

            if (found == null)
                return NotFound($"International license with id {id} does not exist.");

            return Ok(found);
        }

        [Authorize(Roles = "Officer,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<InternationalLicenseDto>> CreateAsync(CreateInternationalLicenseRequest request, CancellationToken ct = default)
        {
            var created = await _internationalLicenseService.CreateAsync(request, ct);
            return CreatedAtRoute("GetInternationalLicenseById", new { id = created.InternationalLicenseId }, created);
        }


        [HttpGet("active/by-driver/{driverId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<InternationalLicenseDto>> GetActiveByDriverIdAsync(int driverId, CancellationToken ct = default)
        {
            var found = await _internationalLicenseService.GetActiveByDriverIdAsync(driverId, ct);

            if (found == null)
                return NotFound($"There is no active international license founded for driver with id {driverId}");

            return Ok(found);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<PagedResult<InternationalLicenseDto>>> GetAllAsync([FromQuery] PaginationParams paging, CancellationToken ct = default)
            => Ok(await _internationalLicenseService.GetPagedAsync(paging, ct));


        [HttpGet("by-driver/{driverId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<PagedResult<InternationalLicenseDto>>> GetAllDriverLicensesAsync(
            int driverId, [FromQuery] PaginationParams paging, CancellationToken ct = default)
            => Ok(await _internationalLicenseService.GetPagedDriverLicensesAsync(driverId, paging, ct));

        [Authorize(Roles = "Officer,Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<InternationalLicenseDto>> UpdateAsync(int id, [FromBody] UpdateInternationalLicenseRequest request, CancellationToken ct = default)
        {
            var existing = await _internationalLicenseService.GetByIdAsync(id, ct);
            if (existing == null)
                return NotFound($"International license with id {id} not found.");

            existing.IsActive = request.IsActive;
            var updated = await _internationalLicenseService.UpdateAsync(id, existing, ct);
            return Ok(updated);
        }
    }
}
