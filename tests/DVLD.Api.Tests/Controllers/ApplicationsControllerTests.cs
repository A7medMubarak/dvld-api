using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Requests.Application;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class ApplicationsControllerTests
    {
        private readonly IApplicationService _service;
        private readonly ApplicationsController _controller;

        public ApplicationsControllerTests()
        {
            _service = Substitute.For<IApplicationService>();
            _controller = new ApplicationsController(_service);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Role, "Admin")
                    }, "TestAuth"))
                }
            };
        }

        private static ApplicationDto CreateSampleApp(int id = 1) => new()
        {
            ApplicationId = id, ApplicantPersonId = 1, ApplicationTypeId = 1, ApplicationStatus = 1,
            ApplicationDate = DateTime.UtcNow, LastStatusDate = DateTime.UtcNow, PaidFees = 100, CreatedByUserId = 1
        };

        [Fact]
        public async Task GetApplicationByIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSampleApp();
            _service.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetApplicationByIdAsync(1, CancellationToken.None);

            var action = result.Should().BeOfType<ActionResult<ApplicationDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetApplicationByIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetApplicationByIdAsync(99, CancellationToken.None);

            var action = result.Should().BeOfType<ActionResult<ApplicationDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetAllApplicationsAsync_ReturnsOkWithList()
        {
            var expected = new List<ApplicationDto> { CreateSampleApp() };
            _service.GetPagedAsync(Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<ApplicationDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllApplicationsAsync(new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<ApplicationDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<ApplicationDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedAtRoute()
        {
            var body = new CreateApplicationRequest();
            var created = CreateSampleApp();
            _service.CreateAsync(body, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.CreateAsync(body);

            var action = result.Should().BeOfType<ActionResult<ApplicationDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetApplicationById");
        }

        [Fact]
        public async Task UpdateApplicationAsync_ReturnsOk()
        {
            var body = new UpdateApplicationRequest();
            var updated = CreateSampleApp();
            _service.UpdateAsync(1, body, Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _controller.UpdateApplicationAsync(1, body);

            var action = result.Should().BeOfType<ActionResult<ApplicationDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteApplicationAsync_ReturnsNoContent()
        {
            var result = await _controller.DeleteApplicationAsync(1);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetActiveAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSampleApp();
            _service.GetActiveAsync(1, 1, 1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetActiveAsync(1, 1, 1);

            var action = result.Should().BeOfType<ActionResult<ApplicationDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetActiveAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetActiveAsync(1, 1, 1, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetActiveAsync(1, 1, 1);

            var action = result.Should().BeOfType<ActionResult<ApplicationDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateStatusAsync_ReturnsNoContent()
        {
            var body = new UpdatedStatusRequest { NewStatus = 2 };

            var result = await _controller.UpdateStatusAsync(1, body);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
