using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.User;
using DVLD.Contracts.Requests.User;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly IUserService _service;
        private readonly ICurrentUserService _currentUser;
        private readonly ISecurityAuditService _audit;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _service = Substitute.For<IUserService>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _audit = Substitute.For<ISecurityAuditService>();
            _controller = new UsersController(_service, _currentUser, _audit);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim(ClaimTypes.UserData, "admin_user")
                    }, "TestAuth"))
                }
            };
            _currentUser.UserId.Returns(1);
            _currentUser.Role.Returns(enRole.Admin);
        }

        private static UserDetailedDto CreateSampleDetailedUser(int id = 1) => new()
        {
            UserId = id,
            PersonId = 1,
            UserName = "testuser",
            IsActive = true,
            Role = enRole.Admin
        };

        private static UserDto CreateSampleUser(int id = 1) => new(id, 1, "testuser", true, enRole.Admin);

        [Fact]
        public async Task GetByUserIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSampleDetailedUser();
            _service.GetDetailedByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByUserIdAsync(1);

            var ok = result.Should().BeOfType<ActionResult<UserDetailedDto>>().Subject;
            ok.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByUserIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetDetailedByUserIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByUserIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<UserDetailedDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByUserIdAsync_AsNonAdmin_ForOtherUser_ReturnsForbid()
        {
            _currentUser.Role.Returns(enRole.Officer);
            _currentUser.UserId.Returns(2);

            var result = await _controller.GetByUserIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<UserDetailedDto>>().Subject;
            action.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsOkWithList()
        {
            var expected = new List<UserDto> { CreateSampleUser() };
            _service.GetPagedAsync(Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<UserDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllUsersAsync(new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<UserDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<UserDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task AddNewUserAsync_ReturnsCreatedAtRoute()
        {
            var body = new CreateUserRequest { PersonId = 1, UserName = "newuser", Password = "Test@123", IsActive = true, Role = enRole.Admin };
            var created = CreateSampleUser();
            _service.CreateAsync(Arg.Any<CreateUserDto>(), Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.AddNewUserAsync(body);

            var action = result.Should().BeOfType<ActionResult<UserDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetUserByUserId");
            _audit.Received(1).LogAdminAction("CreateUser", Arg.Any<string>());
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsOk()
        {
            var body = new UpdateUserRequest { UserName = "updated", IsActive = true, Role = enRole.Officer };
            var updated = CreateSampleUser();
            _service.UpdateAsync(1, Arg.Any<UpdateUserDto>(), Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _controller.UpdateUserAsync(1, body);

            var action = result.Should().BeOfType<ActionResult<UserDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsNoContent()
        {
            var result = await _controller.DeleteUserAsync(1);

            result.Should().BeOfType<NoContentResult>();
            _audit.Received(1).LogAdminAction("DeleteUser", Arg.Any<string>());
        }

        [Fact]
        public async Task ChangePasswordAsync_ReturnsNoContent()
        {
            _currentUser.Role.Returns(enRole.Admin);

            var result = await _controller.ChangePasswordAsync(1, "NewPass@1");

            result.Should().BeOfType<NoContentResult>();
            _audit.Received(1).LogPasswordChanged(1);
        }

        [Fact]
        public async Task ChangePasswordAsync_AsNonAdmin_ForOtherUser_ReturnsForbid()
        {
            _currentUser.Role.Returns(enRole.Officer);
            _currentUser.UserId.Returns(2);

            var result = await _controller.ChangePasswordAsync(1, "NewPass@1");

            result.Should().BeOfType<ForbidResult>();
        }
    }
}
