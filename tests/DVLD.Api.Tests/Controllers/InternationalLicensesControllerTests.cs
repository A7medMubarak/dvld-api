using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.InternationalLicense;
using DVLD.Contracts.Requests.InternationalLicense;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class InternationalLicensesControllerTests
    {
        private readonly IInternationalLicenseService _service;
        private readonly InternationalLicensesController _controller;

        public InternationalLicensesControllerTests()
        {
            _service = Substitute.For<IInternationalLicenseService>();
            _controller = new InternationalLicensesController(_service);
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

        private static InternationalLicenseDto CreateSample(int id = 1) => new()
        {
            InternationalLicenseId = id, DriverId = 1, IsActive = true
        };

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<InternationalLicenseDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<InternationalLicenseDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedAtRoute()
        {
            var request = new CreateInternationalLicenseRequest { IssuedUsingLocalLicenseId = 1, DriverId = 1 };
            var created = CreateSample();
            _service.CreateAsync(request, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.CreateAsync(request);

            var action = result.Should().BeOfType<ActionResult<InternationalLicenseDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetInternationalLicenseById");
        }

        [Fact]
        public async Task GetActiveByDriverIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetActiveByDriverIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetActiveByDriverIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<InternationalLicenseDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetActiveByDriverIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetActiveByDriverIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetActiveByDriverIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<InternationalLicenseDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkWithList()
        {
            var expected = new List<InternationalLicenseDto> { CreateSample() };
            _service.GetPagedAsync(Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<InternationalLicenseDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllAsync(new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<InternationalLicenseDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<InternationalLicenseDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllDriverLicensesAsync_ReturnsOkWithList()
        {
            var expected = new List<InternationalLicenseDto> { CreateSample() };
            _service.GetPagedDriverLicensesAsync(1, Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<InternationalLicenseDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllDriverLicensesAsync(1, new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<InternationalLicenseDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<InternationalLicenseDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WhenNotExists_ReturnsNotFound()
        {
            var request = new UpdateInternationalLicenseRequest { IsActive = false };
            _service.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.UpdateAsync(99, request);

            var action = result.Should().BeOfType<ActionResult<InternationalLicenseDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
