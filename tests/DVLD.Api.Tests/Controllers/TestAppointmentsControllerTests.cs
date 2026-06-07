using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Dtos.TestAppointment;
using DVLD.Contracts.Requests.TestAppointment;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class TestAppointmentsControllerTests
    {
        private readonly ITestAppointmentService _service;
        private readonly TestAppointmentsController _controller;

        public TestAppointmentsControllerTests()
        {
            _service = Substitute.For<ITestAppointmentService>();
            _controller = new TestAppointmentsController(_service);
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

        private static TestAppointmentDto CreateSample(int id = 1) => new()
        {
            TestAppointmentId = id, LocalDrivingLicenseApplicationId = 1, TestTypeId = 1
        };

        [Fact]
        public async Task CreateAsync_ReturnsCreatedAtRoute()
        {
            var body = new CreateTestAppointmentRequest { LocalDrivingLicenseApplicationId = 1, TestTypeId = 1, AppointmentDate = DateTime.UtcNow };
            var created = CreateSample();
            _service.CreateAsync(body, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.CreateAsync(body);

            var action = result.Should().BeOfType<ActionResult<TestAppointmentDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetTestAppointmentById");
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<TestAppointmentDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<TestAppointmentDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkWithList()
        {
            var expected = new List<TestAppointmentViewDto> { new() };
            _service.GetPagedAsync(Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<TestAppointmentViewDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllAsync(new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<TestAppointmentViewDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<TestAppointmentViewDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllByTestTypeAsync_ReturnsOkWithList()
        {
            var expected = new List<TestAppointmentByTestTypeDto> { new() };
            _service.GetPagedByTestTypeAsync(1, 1, Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<TestAppointmentByTestTypeDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllByTestTypeAsync(1, 1, new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<TestAppointmentByTestTypeDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<TestAppointmentByTestTypeDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task GetLastAsync_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetLastAsync(1, 1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetLastAsync(1, 1);

            var action = result.Should().BeOfType<ActionResult<TestAppointmentDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetTestAsync_WhenExists_ReturnsOk()
        {
            var expected = new TestDto();
            _service.GetTestAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetTestAsync(1);

            var action = result.Should().BeOfType<ActionResult<TestDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetTestAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetTestAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetTestAsync(99);

            var action = result.Should().BeOfType<ActionResult<TestDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            var body = new UpdateTestAppointmentRequest();
            var updated = CreateSample();
            _service.UpdateAsync(1, body, Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _controller.UpdateAsync(1, body);

            var action = result.Should().BeOfType<ActionResult<TestAppointmentDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }
    }
}
