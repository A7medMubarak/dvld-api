using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.DetainedLicense;
using DVLD.Contracts.Requests.DetainedLicense;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class DetainedLicensesControllerTests
    {
        private readonly IDetainedLicenseService _service;
        private readonly DetainedLicensesController _controller;

        public DetainedLicensesControllerTests()
        {
            _service = Substitute.For<IDetainedLicenseService>();
            _controller = new DetainedLicensesController(_service);
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

        private static DetainedLicenseDto CreateSample(int detainId = 1) => new()
        {
            DetainId = detainId, LicenseId = 1, IsReleased = false
        };

        [Fact]
        public async Task CreateAsync_ReturnsCreatedAtRoute()
        {
            var request = new CreateDetainedLicenseRequest { LicenseId = 1, FineFees = 100 };
            var created = CreateSample();
            _service.CreateAsync(request, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.CreateAsync(request);

            var action = result.Should().BeOfType<ActionResult<DetainedLicenseDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetDetainedLicenseByDetainId");
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkWithList()
        {
            var expected = new List<DetainedLicenseViewDto> { new() };
            _service.GetPagedAsync(Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<DetainedLicenseViewDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllAsync(new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<DetainedLicenseViewDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<DetainedLicenseViewDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task GetByDetainIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByDetainIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByDetainIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<DetainedLicenseDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByDetainIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByDetainIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByDetainIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<DetainedLicenseDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByLicenseIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByLicenseIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByLicenseIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<DetainedLicenseDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByLicenseIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByLicenseIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByLicenseIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<DetainedLicenseDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task IsLicenseDetainedAsync_ReturnsOkWithBool()
        {
            _service.IsLicenseDetainedAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            var result = await _controller.IsLicenseDetainedAsync(1);

            var action = result.Should().BeOfType<ActionResult<bool>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ReleaseAsync_ReturnsNoContent()
        {
            var result = await _controller.ReleaseAsync(1);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
