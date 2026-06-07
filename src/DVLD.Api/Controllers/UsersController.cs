using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.User;
using DVLD.Contracts.Requests.User;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DVLD.Api.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly ICurrentUserService _currentUser;
        private readonly ISecurityAuditService _audit;

        public UsersController(IUserService service, ICurrentUserService currentUser, ISecurityAuditService audit)
        {
            _service = service;
            _currentUser = currentUser;
            _audit = audit;
        }

        [HttpGet("{id:int}", Name = "GetUserByUserId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<UserDetailedDto>> GetByUserIdAsync(int id, CancellationToken ct = default)
        {
            if (IsNotAdminOrSelf(id))
                return Forbid();

            var user = await _service.GetDetailedByUserIdAsync(id, ct);

            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        [HttpGet("person/{personId:int}", Name = "GetUserByPersonId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<UserDetailedDto>> GetByPersonIdAsync(int personId, CancellationToken ct = default)
        {
            var user = await _service.GetDetailedByPersonIdAsync(personId, ct);

            if (user == null)
                return NotFound("User not found.");

            if (IsNotAdminOrSelf(user.UserId))
                return Forbid();

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<PagedResult<UserDto>>> GetAllUsersAsync([FromQuery] PaginationParams paging, CancellationToken ct = default)
            => Ok(await _service.GetPagedAsync(paging, ct));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<UserDto>> AddNewUserAsync([FromBody] CreateUserRequest body, CancellationToken ct = default)
        {
            var dto = new CreateUserDto
            {
                PersonId = body.PersonId,
                UserName = body.UserName,
                Password = body.Password,
                IsActive = body.IsActive,
                Role = body.Role
            };
            var created = await _service.CreateAsync(dto, ct);

            _audit.LogAdminAction("CreateUser", $"UserId={created.UserId}");

            return CreatedAtRoute("GetUserByUserId", new { id = created.UserId }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<UserDto>> UpdateUserAsync(int id, [FromBody] UpdateUserRequest body, CancellationToken ct = default)
        {
            var dto = new UpdateUserDto
            {
                UserName = body.UserName,
                IsActive = body.IsActive,
                Role = body.Role
            };
            var updated = await _service.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteUserAsync(int id, CancellationToken ct = default)
        {
            await _service.DeleteAsync(id, ct);

            _audit.LogAdminAction("DeleteUser", $"UserId={id}");

            return NoContent();
        }

        [HttpPut("{id:int}/password")]
        [EnableRateLimiting("Sensitive")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<IActionResult> ChangePasswordAsync(int id, [FromBody] string newPassword, CancellationToken ct = default)
        {
            if (IsNotAdminOrSelf(id))
                return Forbid();

            await _service.ChangePasswordAsync(id, newPassword, ct);

            _audit.LogPasswordChanged(id);

            return NoContent();
        }

        private bool IsNotAdminOrSelf(int targetUserId)
            => _currentUser.Role != enRole.Admin && _currentUser.UserId != targetUserId;
    }
}
