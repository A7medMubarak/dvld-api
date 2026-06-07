using DVLD.Contracts.Dtos.Country;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class CountryServiceTests
    {
        private readonly ICountryRepository _repo;
        private readonly CountryService _sut;

        public CountryServiceTests()
        {
            _repo = Substitute.For<ICountryRepository>();
            _sut = new CountryService(_repo);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepo()
        {
            var expected = new List<CountryDto> { new(1, "Egypt"), new(2, "USA") };
            _repo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetAllAsync(default);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCountry()
        {
            var expected = new CountryDto(1, "Egypt");
            _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByIdAsync(1, CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            _repo.GetByIdAsync(99, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetByIdAsync(99, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByNameAsync_WithValidName_ReturnsCountry()
        {
            var expected = new CountryDto(1, "Egypt");
            _repo.GetByNameAsync("Egypt", Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _sut.GetByNameAsync("Egypt", CancellationToken.None);

            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetByNameAsync_WhenNotFound_ReturnsNull()
        {
            _repo.GetByNameAsync("Unknown", Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.GetByNameAsync("Unknown", CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WithNonPositiveId_ThrowsArgumentOutOfRange()
        {
            var act = () => _sut.GetByIdAsync(0, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithParameterName("id");
        }

        [Fact]
        public async Task GetByNameAsync_WithNullName_ThrowsArgumentException()
        {
            var act = () => _sut.GetByNameAsync(null!, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("name");
        }

        [Fact]
        public async Task GetByNameAsync_WithEmptyName_ThrowsArgumentException()
        {
            var act = () => _sut.GetByNameAsync("", CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("name");
        }

        [Fact]
        public async Task GetByNameAsync_WithWhitespaceName_ThrowsArgumentException()
        {
            var act = () => _sut.GetByNameAsync("   ", CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("name");
        }
    }
}
