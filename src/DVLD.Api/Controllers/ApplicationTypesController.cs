using DVLD.Contracts.Dtos.ApplicationType;
using DVLD.Contracts.Requests.ApplicationType;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/application-types")]
    [ApiController]
    public class ApplicationTypesController : ControllerBase
    {
        private readonly IApplicationTypeService _service;

        public ApplicationTypesController(IApplicationTypeService service)
        {
            _service = service;
        }


        [HttpGet("{id:int}", Name = "GetApplicationTypeById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<ApplicationTypeDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var appType = await _service.GetByIdAsync(id, ct);

            if (appType == null)
                return NotFound($"Application type with id {id} not found.");

            return Ok(appType);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<ApplicationTypeDto>>> GetAllAsync(CancellationToken ct = default)
            => Ok(await _service.GetAllAsync(ct));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<ApplicationTypeDto>> CreateAsync([FromBody] CreateApplicationTypeRequest request, CancellationToken ct = default)
        {
            var created = await _service.CreateAsync(request, ct);
            return CreatedAtRoute("GetApplicationTypeById", new { id = created.ApplicationTypeId }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateApplicationTypeRequest request, CancellationToken ct = default)
        {
            var updated = await _service.UpdateAsync(id, request, ct);
            return Ok(updated);
        }
    }
}
