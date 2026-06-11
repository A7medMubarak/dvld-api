using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Driver;
using DVLD.Contracts.Requests.Driver;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/drivers")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly IDriverService _service;

        public DriversController(IDriverService service)
        {
            _service = service;
        }


        [HttpGet("{driverId:int}", Name = "GetDriverInfoByDriverId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<DriverDetailedDto>> GetByDriverIdAsync(int driverId, CancellationToken ct = default)
        {
            var d = await _service.GetDetailedByDriverIdAsync(driverId, ct);

            if (d == null)
                return NotFound($"Driver with ID {driverId} was not found.");

            return Ok(d);
        } 


        [HttpGet("person/{personId:int}", Name = "GetDriverInfoByPersonId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<DriverDetailedDto>> GetByPersonIdAsync(int personId, CancellationToken ct = default)
        {
            var d = await _service.GetDetailedByPersonIdAsync(personId, ct);

            if (d == null)
                return NotFound($"Driver with personId {personId} was not found.");

            return Ok(d);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<PagedResult<DriverDto>>> GetAllDriversAsync([FromQuery] PaginationParams paging, CancellationToken ct = default)
            => Ok(await _service.GetPagedAsync(paging, ct));

        [Authorize(Roles = "Officer,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<DriverDto>> AddNewDriverAsync([FromBody] CreateDriverRequest body, CancellationToken ct = default)
        {
            var d = await _service.CreateAsync(body, ct);
            return CreatedAtRoute("GetDriverInfoByDriverId", new { driverId = d.DriverId }, d);
        }
    }
}
