namespace DVLD.Business.Interfaces.Services
{
    public interface ISecurityAuditService
    {
        void LogFailedLogin(string username);

        void LogAdminAction(string action, string target);

        void LogPasswordChanged(int userId);
    }
}
