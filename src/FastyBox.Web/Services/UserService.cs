using FastyBox.Domain.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FastyBox.Web.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> GetCurrentUserAsync();
        Task<string> GetCurrentUserIdAsync();
        Task<bool> IsInRoleAsync(string role);
        Task<IEnumerable<string>> GetCurrentUserRolesAsync();
        Task<Guid?> GetCurrentTenantIdAsync();
    }

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public UserService(UserManager<ApplicationUser> userManager, AuthenticationStateProvider authenticationStateProvider)
        {
            _userManager = userManager;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            return user.Identity?.IsAuthenticated == true
                ? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User.IsInRole(role);
        }

        public async Task<IEnumerable<string>> GetCurrentUserRolesAsync()
        {
            var user = await GetCurrentUserAsync();
            return user != null ? await _userManager.GetRolesAsync(user) : Array.Empty<string>();
        }

        public async Task<Guid?> GetCurrentTenantIdAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.TenantId;
        }
    }
}
