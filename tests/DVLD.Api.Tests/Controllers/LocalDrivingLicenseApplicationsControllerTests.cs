using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.LocalLicenseApp;
using DVLD.Contracts.Requests.LocalLicenseApp;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class LocalDrivingLicenseApplicationsControllerTests
    {
        private readonly ILocalLicenseAppService _service;
        private readonly LocalDrivingLicenseApplicationsController _controller;

        public LocalDrivingLicenseApplicationsControllerTests()
        {
            _service = Substitute.For<ILocalLicenseAppService>();
            _controller = new LocalDrivingLicenseApplicationsController(_service);
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

        private static LocalLicenseAppDto CreateSample(int id = 1) => new()
        {
            LocalDrivingLicenseApplicationId = id, ApplicationId = 1, LicenseClassId = 1
        };

        [Fact]
        public async Task GetByLocalLicenseAppIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByLocalLicenseAppIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByLocalLicenseAppIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<LocalLicenseAppDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByLocalLicenseAppIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByLocalLicenseAppIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByLocalLicenseAppIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<LocalLicenseAppDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByApplicationIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByApplicationIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByApplicationIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<LocalLicenseAppDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByApplicationIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByApplicationIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByApplicationIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<LocalLicenseAppDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkWithList()
        {
            var expected = new List<LocalLicenseViewDto> { new() };
            _service.GetPagedAsync(Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<LocalLicenseViewDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllAsync(new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<LocalLicenseViewDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<LocalLicenseViewDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task GetTestAttemptCountAsync_ReturnsOk()
        {
            _service.GetTestAttemptCountAsync(1, 1, Arg.Any<CancellationToken>()).Returns((byte)2);

            var result = await _controller.GetTestAttemptCountAsync(1, 1);

            var action = result.Should().BeOfType<ActionResult<byte>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task HasActiveTestAppointmentAsync_ReturnsOk()
        {
            _service.HasActiveTestAppointmentAsync(1, 1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _controller.HasActiveTestAppointmentAsync(1, 1);

            var action = result.Should().BeOfType<ActionResult<bool>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task HasAttendedTestAsync_ReturnsOk()
        {
            _service.HasAttendedTestAsync(1, 1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _controller.HasAttendedTestAsync(1, 1);

            var action = result.Should().BeOfType<ActionResult<bool>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task HasPassedTestAsync_ReturnsOk()
        {
            _service.HasPassedTestAsync(1, 1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _controller.HasPassedTestAsync(1, 1);

            var action = result.Should().BeOfType<ActionResult<bool>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedAtRoute()
        {
            var request = new CreateLocalLicenseAppRequest { ApplicantPersonId = 1, LicenseClassId = 1 };
            var created = CreateSample();
            _service.CreateAsync(request, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.CreateAsync(request);

            var action = result.Should().BeOfType<ActionResult<LocalLicenseAppDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetByLocalLicenseAppId");
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            var request = new UpdateLocalLicenseAppRequest { ApplicationStatus = 2 };
            var updated = CreateSample();
            _service.UpdateAsync(1, request, Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _controller.UpdateAsync(1, request);

            var action = result.Should().BeOfType<ActionResult<LocalLicenseAppDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent()
        {
            var result = await _controller.DeleteAsync(1);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ExistsAsync_ReturnsOk()
        {
            _service.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _controller.ExistsAsync(1);

            var action = result.Should().BeOfType<ActionResult<bool>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }
    }
}
