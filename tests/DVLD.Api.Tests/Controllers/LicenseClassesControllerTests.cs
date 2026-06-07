using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Dtos.LicenseClass;
using DVLD.Contracts.Requests.LicenseClass;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class LicenseClassesControllerTests
    {
        private readonly ILicenseClassService _service;
        private readonly LicenseClassesController _controller;

        public LicenseClassesControllerTests()
        {
            _service = Substitute.For<ILicenseClassService>();
            _controller = new LicenseClassesController(_service);
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

        private static LicenseClassDto CreateSample(int id = 1) => new(id, "Class A", "Description", (byte)18, (byte)10, 100m);

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<LicenseClassDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<LicenseClassDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByClassNameAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSample();
            _service.GetByClassNameAsync("Class A", Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetByClassNameAsync("Class A");

            var action = result.Should().BeOfType<ActionResult<LicenseClassDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByClassNameAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetByClassNameAsync("Unknown", Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetByClassNameAsync("Unknown");

            var action = result.Should().BeOfType<ActionResult<LicenseClassDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkWithList()
        {
            var expected = new List<LicenseClassDto> { CreateSample() };
            _service.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetAllAsync();

            var action = result.Should().BeOfType<ActionResult<IEnumerable<LicenseClassDto>>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedAtRoute()
        {
            var request = new CreateLicenseClassRequest { ClassName = "New", ClassDescription = "Desc", MinimumAllowedAge = 18, DefaultValidityLength = 10, ClassFees = 200 };
            var created = CreateSample();
            _service.CreateAsync(request, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.CreateAsync(request);

            var action = result.Should().BeOfType<ActionResult<LicenseClassDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetLicenseClassById");
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            var request = new UpdateLicenseClassRequest { ClassName = "Updated" };
            var updated = CreateSample();
            _service.UpdateAsync(1, request, Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _controller.UpdateAsync(1, request);

            var action = result.Should().BeOfType<ActionResult<LicenseClassDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }
    }
}
