using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.DetainedLicense;
using DVLD.Contracts.Requests.DetainedLicense;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/detained-licenses")]
    [ApiController]
    public class DetainedLicensesController : ControllerBase
    {
        private readonly IDetainedLicenseService _detainedLicenseService;

        public DetainedLicensesController(IDetainedLicenseService detainedLicenseService)
        {
            _detainedLicenseService = detainedLicenseService; 
        }

        [Authorize(Roles = "Officer,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<DetainedLicenseDto>> CreateAsync([FromBody] CreateDetainedLicenseRequest request, CancellationToken ct = default)
        {
            var created = await _detainedLicenseService.CreateAsync(request, ct);
            return CreatedAtRoute("GetDetainedLicenseByDetainId", new { detainId = created.DetainId }, created);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<PagedResult<DetainedLicenseViewDto>>> GetAllAsync([FromQuery] PaginationParams paging, CancellationToken ct = default)
            => Ok(await _detainedLicenseService.GetPagedAsync(paging, ct));


        [HttpGet("{detainId}", Name = "GetDetainedLicenseByDetainId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<DetainedLicenseDto>> GetByDetainIdAsync(int detainId, CancellationToken ct = default)
        {
            var detainedLicense = await _detainedLicenseService.GetByDetainIdAsync(detainId, ct);

            if (detainedLicense == null)
                return NotFound($"Detained license with id {detainId} not found.");

            return Ok(detainedLicense);
        }


        [HttpGet("by-license/{licenseId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<DetainedLicenseDto>> GetByLicenseIdAsync(int licenseId, CancellationToken ct = default)
        {
            var detainedLicense = await _detainedLicenseService.GetByLicenseIdAsync(licenseId, ct);

            if (detainedLicense == null)
                return NotFound($"Detained license with id {licenseId} not found.");

            return Ok(detainedLicense);
        }


        [HttpGet("{licenseId}/is-detained")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<bool>> IsLicenseDetainedAsync(int licenseId, CancellationToken ct = default)
        {
            bool isDetained = await _detainedLicenseService.IsLicenseDetainedAsync(licenseId, ct);
            return Ok(isDetained);
        }

        [Authorize(Roles = "Officer,Admin")]
        [HttpPost("{detainId}/release")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> ReleaseAsync(int detainId, CancellationToken ct = default)
        {
            await _detainedLicenseService.ReleaseDetainedLicenseAsync(detainId, ct);
            return NoContent();
        }
    }
}
