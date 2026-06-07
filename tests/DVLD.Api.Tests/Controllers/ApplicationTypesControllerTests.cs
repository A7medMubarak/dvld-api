using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Dtos.ApplicationType;
using DVLD.Contracts.Requests.ApplicationType;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class ApplicationTypesControllerTests
    {
        private readonly IApplicationTypeService _service;
        private readonly ApplicationTypesController _controller;

        public ApplicationTypesControllerTests()
        {
            _service = Substitute.For<IApplicationTypeService>();
            _controller = new ApplicationTypesController(_service);
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

        private static ApplicationTypeDto CreateSample(int id = 1) => new(id, "Type", 100);

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<ApplicationTypeDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<ApplicationTypeDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkWithList()
        {
            var expected = new List<ApplicationTypeDto> { CreateSample() };
            _service.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetAllAsync();

            var action = result.Should().BeOfType<ActionResult<IEnumerable<ApplicationTypeDto>>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedAtRoute()
        {
            var request = new CreateApplicationTypeRequest { ApplicationTypeTitle = "New", ApplicationTypeFees = 200 };
            var created = CreateSample();
            _service.CreateAsync(request, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.CreateAsync(request);

            var action = result.Should().BeOfType<ActionResult<ApplicationTypeDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetApplicationTypeById");
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            var request = new UpdateApplicationTypeRequest { ApplicationTypeTitle = "Updated", ApplicationTypeFees = 300 };
            var updated = CreateSample();
            _service.UpdateAsync(1, request, Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _controller.UpdateAsync(1, request);

            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
