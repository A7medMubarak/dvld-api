using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Requests.Test;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class TestsControllerTests
    {
        private readonly ITestService _service;
        private readonly TestsController _controller;

        public TestsControllerTests()
        {
            _service = Substitute.For<ITestService>();
            _controller = new TestsController(_service);
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

        private static TestDto CreateSample(int id = 1) => new()
        {
            TestId = id, TestAppointmentId = 1, TestResult = true, CreatedByUserId = 1
        };

        private static TestWithApplicantDto CreateSampleWithApplicant(int id = 1) => new()
        {
            TestId = id, TestAppointmentId = 1, TestResult = true, CreatedByUserId = 1
        };

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<TestDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<TestDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkWithList()
        {
            var expected = new List<TestDto> { CreateSample() };
            _service.GetPagedAsync(Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<TestDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllAsync(new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<TestDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<TestDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task GetLatestAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSampleWithApplicant();
            _service.GetLatestAsync(1, 1, 1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetLatestAsync(1, 1, 1);

            var action = result.Should().BeOfType<ActionResult<TestDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetLatestAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetLatestAsync(1, 1, 1, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetLatestAsync(1, 1, 1);

            var action = result.Should().BeOfType<ActionResult<TestDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetPassedCountAsync_ReturnsOk()
        {
            _service.GetPassedCountAsync(1, Arg.Any<CancellationToken>()).Returns(3);

            var result = await _controller.GetPassedCountAsync(1);

            var action = result.Should().BeOfType<ActionResult<int>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedAtRoute()
        {
            var request = new CreateTestRequest { TestAppointmentId = 1, TestResult = true };
            var created = CreateSample();
            _service.CreateAsync(request, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.CreateAsync(request);

            var action = result.Should().BeOfType<ActionResult<TestDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetTestById");
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            var request = new UpdateTestRequest { TestResult = false };
            var updated = CreateSample();
            _service.UpdateAsync(1, request, Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _controller.UpdateAsync(1, request);

            var action = result.Should().BeOfType<ActionResult<TestDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }
    }
}
