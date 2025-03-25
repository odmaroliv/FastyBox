using FastyBox.Application.Tenants.Commands.CreateTenant;
using FastyBox.Domain.Entities;
using FastyBox.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastyBox.Web.Services
{
    public interface ITenantService
    {
        Task<IEnumerable<Tenant>> GetAllTenantsAsync();
        Task<Tenant> GetTenantByIdAsync(Guid id);
        Task<Tenant> GetTenantBySlugAsync(string slug);
        Task<TenantSettings> GetTenantSettingsAsync(Guid tenantId);
        Task<Guid> CreateTenantAsync(CreateTenantCommand command);
        Task UpdateTenantSettingsAsync(TenantSettings settings);
    }

    public class TenantService : ITenantService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public TenantService(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
        {
            return await _context.Tenants
                .Include(t => t.ParentTenant)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Tenant> GetTenantByIdAsync(Guid id)
        {
            return await _context.Tenants
                .Include(t => t.Settings)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tenant> GetTenantBySlugAsync(string slug)
        {
            return await _context.Tenants
                .Include(t => t.Settings)
                .FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task<TenantSettings> GetTenantSettingsAsync(Guid tenantId)
        {
            return await _context.TenantSettings
                .FirstOrDefaultAsync(ts => ts.TenantId == tenantId);
        }

        public async Task<Guid> CreateTenantAsync(CreateTenantCommand command)
        {
            return await _mediator.Send(command);
        }

        public async Task UpdateTenantSettingsAsync(TenantSettings settings)
        {
            _context.TenantSettings.Update(settings);
            await _context.SaveChangesAsync();
        }
    }
}
