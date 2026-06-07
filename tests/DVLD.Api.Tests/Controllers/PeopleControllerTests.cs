using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Person;
using DVLD.Contracts.Requests.Person;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLD.Api.Tests.Controllers
{
    public class PeopleControllerTests
    {
        private readonly IPersonService _service;
        private readonly PeopleController _controller;

        public PeopleControllerTests()
        {
            _service = Substitute.For<IPersonService>();
            _controller = new PeopleController(_service);
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

        private static PersonDetailedDto CreateSamplePerson() => new()
        {
            PersonId = 1, NationalNo = "N123", FirstName = "John", LastName = "Doe"
        };

        private static PersonDto CreateSamplePersonDto() => new()
        {
            PersonId = 1, NationalNo = "N123", FirstName = "John", LastName = "Doe"
        };

        [Fact]
        public async Task GetPersonByIdAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSamplePerson();
            _service.GetDetailsByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetPersonByIdAsync(1);

            var action = result.Should().BeOfType<ActionResult<PersonDetailedDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetPersonByIdAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetDetailsByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetPersonByIdAsync(99);

            var action = result.Should().BeOfType<ActionResult<PersonDetailedDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetPersonByNationalNoAsync_WhenExists_ReturnsOk()
        {
            var expected = CreateSamplePerson();
            _service.GetDetailsByNationalNoAsync("N123", Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.GetPersonByNationalNoAsync("N123");

            var action = result.Should().BeOfType<ActionResult<PersonDetailedDto>>().Subject;
            action.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetPersonByNationalNoAsync_WhenNotExists_ReturnsNotFound()
        {
            _service.GetDetailsByNationalNoAsync("UNKNOWN", Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.GetPersonByNationalNoAsync("UNKNOWN");

            var action = result.Should().BeOfType<ActionResult<PersonDetailedDto>>().Subject;
            action.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllPeopleAsync_ReturnsOkWithList()
        {
            var expected = new List<PersonDto> { CreateSamplePersonDto() };
            _service.GetPagedAsync(Arg.Any<PaginationParams>(), Arg.Any<CancellationToken>()).Returns(new PagedResult<PersonDto> { Items = expected, PageNumber = 1, PageSize = 10, TotalCount = expected.Count });

            var result = await _controller.GetAllPeopleAsync(new PaginationParams());

            var action = result.Should().BeOfType<ActionResult<PagedResult<PersonDto>>>().Subject;
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var pagedResult = okResult.Value.Should().BeOfType<PagedResult<PersonDto>>().Subject;
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
            pagedResult.TotalCount.Should().Be(expected.Count);
            pagedResult.HasPreviousPage.Should().BeFalse();
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task AddNewPersonAsync_ReturnsCreatedAtRoute()
        {
            var body = new CreatePersonRequest { NationalNo = "N456", FirstName = "Jane", LastName = "Doe" };
            var created = CreateSamplePersonDto();
            _service.CreateAsync(body, Arg.Any<CancellationToken>()).Returns(created);

            var result = await _controller.AddNewPersonAsync(body);

            var action = result.Should().BeOfType<ActionResult<PersonDto>>().Subject;
            var createdResult = action.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetPersonById");
        }

        [Fact]
        public async Task UpdatePersonAsync_ReturnsOk()
        {
            var body = new UpdatePersonRequest { FirstName = "Updated" };
            var updated = CreateSamplePersonDto();
            _service.UpdateAsync(1, body, Arg.Any<CancellationToken>()).Returns(updated);

            var result = await _controller.UpdatePersonAsync(1, body);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeletePersonAsync_ReturnsNoContent()
        {
            var result = await _controller.DeletePersonAsync(1);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
