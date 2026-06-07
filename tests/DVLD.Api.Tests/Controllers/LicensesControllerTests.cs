using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.License;
using DVLD.Contracts.Requests.License;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class LicensesControllerTests
    {
        private readonly ILicenseService _service;
        private readonly LicensesController _controller;

        public LicensesControllerTests()
        {
            _service = Substitute.For<ILicenseService>();
            _controller = new LicensesController(_service);
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

        private static LicenseDto CreateSample(int id = 1) => new()
        {
            LicenseId = id, DriverId = 1, LicenseClassId = 1, IsActive = true
        };

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<LicenseDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<LicenseDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkWithList()
        {
            var expected = new List<LicenseDto> { CreateSample() };
            _service.GetPagedAsync(Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<LicenseDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllAsync(new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<LicenseDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<LicenseDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task GetDriverLicensesAsync_ReturnsOkWithList()
        {
            var expected = new List<DriverLicensesDto> { new() };
            _service.GetPagedDriverLicensesAsync(1, Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<DriverLicensesDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetDriverLicensesAsync(1, new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<DriverLicensesDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<DriverLicensesDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task GetActiveLicenseForPersonAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetActiveLicenseForPersonAsync(1, 1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetActiveLicenseForPersonAsync(1, 1);

            var action = result.Should().BeOfType<ActionResult<LicenseDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetActiveLicenseForPersonAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetActiveLicenseForPersonAsync(1, 1, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetActiveLicenseForPersonAsync(1, 1);

            var action = result.Should().BeOfType<ActionResult<LicenseDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedAtRoute()
        {
            var request = new CreateLicenseRequest { ApplicationId = 1, DriverId = 1, LicenseClassId = 1, IssueDate = DateTime.UtcNow, ExpirationDate = DateTime.UtcNow.AddYears(1), PaidFees = 100, IsActive = true, IssueReason = 1 };
            var created = CreateSample();
            _service.CreateAsync(request, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.CreateAsync(request);

            var action = result.Should().BeOfType<ActionResult<LicenseDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetLicenseById");
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            var request = new UpdateLicenseRequest { IsActive = true };
            var updated = CreateSample();
            _service.UpdateAsync(1, request, Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _controller.UpdateAsync(1, request);

            var action = result.Should().BeOfType<ActionResult<LicenseDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeactivateAsync_ReturnsNoContent()
        {
            var result = await _controller.DeactivateAsync(1);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
