using DVLD.Contracts.Dtos.TestType;
using DVLD.Contracts.Requests.TestType;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/test-types")]
    [ApiController]
    public class TestTypesController : ControllerBase
    {
        private readonly ITestTypeService _service;

        public TestTypesController(ITestTypeService service)
        {
            _service = service;
        }


        [HttpGet("{id:int}", Name = "GetTestTypeById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<TestTypeDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var testType = await _service.GetByIdAsync(id, ct);

            if (testType == null)
                return NotFound($"Test type with id {id} doesn't exist.");

            return Ok(testType);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<TestTypeDto>>> GetAllAsync(CancellationToken ct = default) => Ok(await _service.GetAllAsync(ct));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<TestTypeDto>> CreateAsync([FromBody] CreateTestTypeRequest request, CancellationToken ct = default)
        {
            var created = await _service.CreateAsync(request, ct);
            return CreatedAtRoute("GetTestTypeById", new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<TestTypeDto>> UpdateAsync(int id, [FromBody] UpdateTestTypeRequest request, CancellationToken ct = default)
        {
            var updated = await _service.UpdateAsync(id, request, ct);
            return Ok(updated);
        }
    }
}
