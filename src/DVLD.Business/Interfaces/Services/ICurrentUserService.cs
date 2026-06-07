using DVLD.Contracts.Common.Enums;

namespace DVLD.Business.Interfaces.Services
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        enRole Role { get; }
    }
}
