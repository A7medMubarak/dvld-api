using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.LocalLicenseApp;
using DVLD.Contracts.Requests.LocalLicenseApp;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/local-driving-license-applications")]
    [ApiController]
    public class LocalDrivingLicenseApplicationsController : ControllerBase
    {
        private readonly ILocalLicenseAppService _localLicenseAppService;

        public LocalDrivingLicenseApplicationsController(ILocalLicenseAppService localLicenseAppService)
        {
            _localLicenseAppService = localLicenseAppService;
        }


        [HttpGet("{localLicenseAppId:int}", Name = "GetByLocalLicenseAppId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<LocalLicenseAppDto>> GetByLocalLicenseAppIdAsync(int localLicenseAppId, CancellationToken ct = default)
        {
            var app = await _localLicenseAppService.GetByLocalLicenseAppIdAsync(localLicenseAppId, ct);

            if (app == null)
                return NotFound($"Local license app with id {localLicenseAppId} not found.");

            return Ok(app);
        }


        [HttpGet("by-application/{applicationId:int}", Name = "GetByApplicationId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<LocalLicenseAppDto>> GetByApplicationIdAsync(int applicationId, CancellationToken ct = default)
        {
            var app = await _localLicenseAppService.GetByApplicationIdAsync(applicationId, ct);

            if (app == null)
                return NotFound($"Local license app with application id {applicationId} not found.");

            return Ok(app);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<PagedResult<LocalLicenseViewDto>>> GetAllAsync([FromQuery] PaginationParams paging, CancellationToken ct = default)
            => Ok(await _localLicenseAppService.GetPagedAsync(paging, ct));


        [HttpGet("{localLicenseId:int}/attempt-count/{testTypeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<byte>> GetTestAttemptCountAsync(int localLicenseId, int testTypeId, CancellationToken ct = default)
            => Ok(await _localLicenseAppService.GetTestAttemptCountAsync(localLicenseId, testTypeId, ct));


        [HttpGet("{localLicenseId:int}/active-appointment/{testTypeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<bool>> HasActiveTestAppointmentAsync(int localLicenseId, int testTypeId, CancellationToken ct = default)
            => Ok(await _localLicenseAppService.HasActiveTestAppointmentAsync(localLicenseId, testTypeId, ct));


        [HttpGet("{localLicenseId:int}/has-attended/{testTypeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<bool>> HasAttendedTestAsync(int localLicenseId, int testTypeId, CancellationToken ct = default)
            => Ok(await _localLicenseAppService.HasAttendedTestAsync(localLicenseId, testTypeId, ct));


        [HttpGet("{localLicenseId:int}/has-Passed/{testTypeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<bool>> HasPassedTestAsync(int localLicenseId, int testTypeId, CancellationToken ct = default)
            => Ok(await _localLicenseAppService.HasPassedTestAsync(localLicenseId, testTypeId, ct));

        [Authorize(Roles = "Officer,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<LocalLicenseAppDto>> CreateAsync([FromBody]CreateLocalLicenseAppRequest request, CancellationToken ct = default)
        {
            var created = await _localLicenseAppService.CreateAsync(request, ct);
            return CreatedAtRoute("GetByLocalLicenseAppId", new { localLicenseAppId = created.LocalDrivingLicenseApplicationId }, created);
        }

        [Authorize(Roles = "Officer,Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<LocalLicenseAppDto>> UpdateAsync(int id, [FromBody]UpdateLocalLicenseAppRequest request, CancellationToken ct = default)
        {
            var updated = await _localLicenseAppService.UpdateAsync(id, request, ct);
            return Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            await _localLicenseAppService.DeleteAsync(id, ct);
            return NoContent();
        }


        [HttpGet("exists{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<bool>> ExistsAsync(int id, CancellationToken ct = default)
        {
            bool result = await _localLicenseAppService.ExistsAsync(id, ct);
            return Ok(result);
        }
    }
}
