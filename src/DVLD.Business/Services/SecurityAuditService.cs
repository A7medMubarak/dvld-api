using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DVLD.Business.Services
{
    public class SecurityAuditService : ISecurityAuditService
    {
        private readonly ILogger<SecurityAuditService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecurityAuditService(ILogger<SecurityAuditService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetClientIp()
            => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        public void LogFailedLogin(string username)
        {
            _logger.LogWarning("Failed login attempt for username {Username} from IP {IP}. {EventType}",
                username, GetClientIp(), "SecurityEvent");
        }

        public void LogAdminAction(string action, string target)
        {
            _logger.LogWarning("Admin action: {Action} on {Target}. {EventType}",
                action, target, "SecurityEvent");
        }

        public void LogPasswordChanged(int userId)
        {
            _logger.LogWarning("Password changed for user ID {UserId}. {EventType}",
                userId, "SecurityEvent");
        }
    }
}
