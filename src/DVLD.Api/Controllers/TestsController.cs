using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Requests.Test;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/tests")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ITestService _service;

        public TestsController(ITestService service)
        {
            _service = service;    
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetTestById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<TestDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var test = await _service.GetByIdAsync(id, ct);

            if (test == null)
                return NotFound("Test with id {id} does not exist.");

            return Ok(test);    
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<PagedResult<TestDto>>> GetAllAsync([FromQuery] PaginationParams paging, CancellationToken ct = default)
            => Ok(await _service.GetPagedAsync(paging, ct));

        [AllowAnonymous]
        [HttpGet("latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<TestDto>> GetLatestAsync([FromQuery] int personId,
                                                [FromQuery] int licenseClassId,
                                                [FromQuery] int testTypeId, CancellationToken ct = default)
        {
            var test = await _service.GetLatestAsync(personId, licenseClassId, testTypeId, ct);

            if (test == null)
                return NotFound("No tests found.");

            return Ok(test);
        }

        [AllowAnonymous]
        [HttpGet("passed-count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<int>> GetPassedCountAsync([FromQuery]int localDrivingLicenseAppId, CancellationToken ct = default)
            => Ok(await _service.GetPassedCountAsync(localDrivingLicenseAppId, ct));

        [Authorize(Roles = "Officer,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<TestDto>> CreateAsync([FromBody]CreateTestRequest request, CancellationToken ct = default)
        {
            var created = await _service.CreateAsync(request, ct);
            return CreatedAtRoute("GetTestById", new { id = created.TestId }, created);
        }

        [Authorize(Roles = "Officer,Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<TestDto>> UpdateAsync(int id, [FromBody] UpdateTestRequest request, CancellationToken ct = default)
        {
            var updated = await _service.UpdateAsync(id, request, ct);
            return Ok(updated);
        }
    }
}
