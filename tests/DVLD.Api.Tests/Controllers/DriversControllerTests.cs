using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Driver;
using DVLD.Contracts.Requests.Driver;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class DriversControllerTests
    {
        private readonly IDriverService _service;
        private readonly DriversController _controller;

        public DriversControllerTests()
        {
            _service = Substitute.For<IDriverService>();
            _controller = new DriversController(_service);
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

        private static DriverDetailedDto CreateSampleDetailed(int id = 1) => new()
        {
            DriverId = id, PersonId = 1, CreatedByUserId = 1, CreatedDate = DateTime.UtcNow
        };

        private static DriverDto CreateSampleDto(int id = 1) => new()
        {
            DriverId = id, PersonId = 1, CreatedByUserId = 1, CreatedDate = DateTime.UtcNow
        };

        [Fact]
        public async Task GetByDriverIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSampleDetailed();
            _service.GetDetailedByDriverIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByDriverIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<DriverDetailedDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByDriverIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetDetailedByDriverIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByDriverIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<DriverDetailedDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByPersonIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSampleDetailed();
            _service.GetDetailedByPersonIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByPersonIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<DriverDetailedDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByPersonIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetDetailedByPersonIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByPersonIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<DriverDetailedDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllDriversAsync_ReturnsOkWithList()
        {
            var expected = new List<DriverDto> { CreateSampleDto() };
            _service.GetPagedAsync(Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<DriverDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllDriversAsync(new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<DriverDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<DriverDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task AddNewDriverAsync_ReturnsCreatedAtRoute()
        {
            var body = new CreateDriverRequest { PersonId = 1, CreatedDate = DateTime.UtcNow };
            var created = CreateSampleDto();
            _service.CreateAsync(body, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.AddNewDriverAsync(body);

            var action = result.Should().BeOfType<ActionResult<DriverDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetDriverInfoByDriverId");
        }
    }
}
