using DVLD.Contracts.Common.Enums;
using DVLD.Business.Interfaces.Services;
using System.Security.Claims;

namespace DVLD.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId => int.Parse(
            _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        public enRole Role => Enum.Parse<enRole>(
            _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value);
    }
}
