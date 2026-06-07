using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Dtos.TestType;
using DVLD.Contracts.Requests.TestType;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class TestTypesControllerTests
    {
        private readonly ITestTypeService _service;
        private readonly TestTypesController _controller;

        public TestTypesControllerTests()
        {
            _service = Substitute.For<ITestTypeService>();
            _controller = new TestTypesController(_service);
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

        private static TestTypeDto CreateSample(int id = 1) => new(id, "Vision", "Description", 50m);

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<TestTypeDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<TestTypeDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkWithList()
        {
            var expected = new List<TestTypeDto> { CreateSample() };
            _service.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetAllAsync();

            var action = result.Should().BeOfType<ActionResult<IEnumerable<TestTypeDto>>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedAtRoute()
        {
            var request = new CreateTestTypeRequest { Title = "New", Description = "Desc", Fees = 100 };
            var created = CreateSample();
            _service.CreateAsync(request, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.CreateAsync(request);

            var action = result.Should().BeOfType<ActionResult<TestTypeDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetTestTypeById");
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            var request = new UpdateTestTypeRequest { Title = "Updated", Description = "Desc", Fees = 150 };
            var updated = CreateSample();
            _service.UpdateAsync(1, request, Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _controller.UpdateAsync(1, request);

            var action = result.Should().BeOfType<ActionResult<TestTypeDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }
    }
}
