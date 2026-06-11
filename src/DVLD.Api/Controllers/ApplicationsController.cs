using Microsoft.AspNetCore.Mvc;
using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Requests.Application;
using Microsoft.AspNetCore.Authorization;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/applications")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _service;

        public ApplicationsController(IApplicationService service)
        {
            _service = service;
        }


        [HttpGet("{id:int}", Name = "GetApplicationById")]
        [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

        public async Task< ActionResult<ApplicationDto>> GetApplicationByIdAsync(int id, CancellationToken ct)
        {
            var result = await _service.GetByIdAsync(id, ct);
         
            return result is null ? NotFound() : Ok(result);           
        }
         


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<PagedResult<ApplicationDto>>> GetAllApplicationsAsync([FromQuery] PaginationParams paging, CancellationToken ct = default)
        {
            var apps = await _service.GetPagedAsync(paging, ct);
            return Ok(apps);
        }


        // POST /api/applications
        [Authorize(Roles = "Officer")]
        [HttpPost]
        [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        
        public async Task <ActionResult<ApplicationDto>> CreateAsync([FromBody] CreateApplicationRequest body, CancellationToken ct = default)
        {
                var created = await _service.CreateAsync(body, ct);

                return CreatedAtRoute("GetApplicationById", new { id = created.ApplicationId }, created);
        }


        [Authorize(Roles = "Officer,Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

        public async Task <ActionResult<ApplicationDto>> UpdateApplicationAsync(int id, [FromBody] UpdateApplicationRequest body, CancellationToken ct = default)
        {
                var updated = await _service.UpdateAsync(id, body, ct);
                return Ok(updated);  
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

        public async Task <IActionResult> DeleteApplicationAsync(int id, CancellationToken ct = default)  //IActionResult Not return data
        {
          
                await _service.DeleteAsync(id, ct);
                return NoContent();
     
        }



        [HttpGet("active")]   // GET /api/applications/active?personId=1&typeId=2&licenseClassId=3
        [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

        public async Task <ActionResult<ApplicationDto>> GetActiveAsync(
            [FromQuery] int personId,
            [FromQuery] int typeId,
            [FromQuery] int? licenseClassId,
            CancellationToken ct = default)
        {
           

            var app = await _service.GetActiveAsync(personId, typeId, licenseClassId, ct);
            if (app == null) return NotFound();

            return Ok(app);
        }


        [Authorize(Roles = "Officer,Admin")]
        [HttpPatch("{id:int}/status")]          // PATCH /api/applications/{id}/status
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

        public async Task <IActionResult> UpdateStatusAsync(int id, [FromBody] UpdatedStatusRequest body, CancellationToken ct = default)
        {
            
                await _service.UpdateStatusAsync(id, body.NewStatus, ct);
                return NoContent();
            

        }

    }


}

    

