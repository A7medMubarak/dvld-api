using DVLD.Contracts.Dtos.LicenseClass;
using DVLD.Contracts.Requests.LicenseClass;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/license-classes")]
    [ApiController]
    public class LicenseClassesController : ControllerBase
    {
        private readonly ILicenseClassService _service;

        public LicenseClassesController(ILicenseClassService service)
        {
            _service = service;
        }


        [HttpGet("{id:int}", Name = "GetLicenseClassById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<LicenseClassDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var licenseClass = await _service.GetByIdAsync(id, ct);

            if (licenseClass == null)
                return NotFound($"License class with id {id} not found.");

            return Ok(licenseClass);
        }


        [HttpGet("by-name{className}", Name = "GetLicenseClassByClassName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<LicenseClassDto>> GetByClassNameAsync(string className, CancellationToken ct = default)
        {
            var licenseClass = await _service.GetByClassNameAsync(className, ct);

            if (licenseClass == null)
                return NotFound($"License class with class name {className} not found.");

            return Ok(licenseClass);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<LicenseClassDto>>> GetAllAsync(CancellationToken ct = default) => Ok(await _service.GetAllAsync(ct));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<LicenseClassDto>> CreateAsync([FromBody] CreateLicenseClassRequest request, CancellationToken ct = default)
        {
            var created = await _service.CreateAsync(request, ct);
            return CreatedAtRoute("GetLicenseClassById", new { id = created.LicenseClassId }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<LicenseClassDto>> UpdateAsync(int id, [FromBody] UpdateLicenseClassRequest request, CancellationToken ct = default)
        {
            var updated = await _service.UpdateAsync(id, request, ct);
            return Ok(updated);
        }
    }
}
