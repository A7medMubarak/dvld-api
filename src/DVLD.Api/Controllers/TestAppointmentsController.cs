using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Dtos.TestAppointment;
using DVLD.Contracts.Requests.TestAppointment;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/test-appointments")]
    [ApiController]
    public class TestAppointmentsController : ControllerBase
    {
        private readonly ITestAppointmentService _service;

        public TestAppointmentsController(ITestAppointmentService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Officer,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<TestAppointmentDto>> CreateAsync([FromBody] CreateTestAppointmentRequest body, CancellationToken ct = default)
        {
            var created = await _service.CreateAsync(body, ct);
            return CreatedAtRoute("GetTestAppointmentById", new { id = created.TestAppointmentId }, created);
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetTestAppointmentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<TestAppointmentDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var testAppointment = await _service.GetByIdAsync(id, ct);

            if (testAppointment == null)
                return NotFound($"Test apoointment with id {id} does not exist.");

            return Ok(testAppointment);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<PagedResult<TestAppointmentViewDto>>> GetAllAsync([FromQuery] PaginationParams paging, CancellationToken ct = default)
            => Ok(await _service.GetPagedAsync(paging, ct));

        [AllowAnonymous]
        [HttpGet("by-test-type")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<PagedResult<TestAppointmentByTestTypeDto>>> GetAllByTestTypeAsync(
            int localLicenseAppId, int testTypeId, [FromQuery] PaginationParams paging, CancellationToken ct = default)
        {
            var list = await _service.GetPagedByTestTypeAsync(localLicenseAppId, testTypeId, paging, ct);
            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<TestAppointmentDto>> GetLastAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default)
        {
            var last = await _service.GetLastAsync(localLicenseAppId, testTypeId, ct);
            return Ok(last);
        }

        [AllowAnonymous]
        [HttpGet("test-by-appointment/{testAppointmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<TestDto>> GetTestAsync(int testAppointmentId, CancellationToken ct = default)
        {
            var test = await _service.GetTestAsync(testAppointmentId, ct);

            if (test == null)
                return NotFound($"There is no test found for appointment id {testAppointmentId}");

            return Ok(test);
        }

        [Authorize(Roles = "Officer,Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<TestAppointmentDto>> UpdateAsync(int id, [FromBody] UpdateTestAppointmentRequest body, CancellationToken ct = default)
        {
            var updated = await _service.UpdateAsync(id, body, ct);
            return Ok(updated);
        }
    }
}
