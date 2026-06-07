using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DVLD.Business.Tests.Services
{
    public class SecurityAuditServiceTests
    {
        private readonly ILogger<SecurityAuditService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SecurityAuditService _sut;

        public SecurityAuditServiceTests()
        {
            _logger = Substitute.For<ILogger<SecurityAuditService>>();
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _httpContextAccessor.HttpContext.Returns((HttpContext?)null);
            _sut = new SecurityAuditService(_logger, _httpContextAccessor);
        }

        [Fact]
        public void LogFailedLogin_LogsWarning()
        {
            _sut.LogFailedLogin("testuser");

            _logger.Received(1).Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("testuser") && o.ToString()!.Contains("SecurityEvent")),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public void LogAdminAction_LogsWarning()
        {
            _sut.LogAdminAction("DELETE_USER", "user123");

            _logger.Received(1).Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("DELETE_USER") && o.ToString()!.Contains("user123")),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public void LogPasswordChanged_LogsWarning()
        {
            _sut.LogPasswordChanged(42);

            _logger.Received(1).Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("42")),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }
    }
}
