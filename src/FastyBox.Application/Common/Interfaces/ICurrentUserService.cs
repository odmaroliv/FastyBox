namespace FastyBox.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        Guid? TenantId { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
        IEnumerable<string> Roles { get; }
    }
}
