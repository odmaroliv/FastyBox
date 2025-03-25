using FastyBox.Application.Common.Interfaces;
using FastyBox.Domain.Entities;
using MediatR;

namespace FastyBox.Application.Tenants.Commands.CreateTenant
{
    public class CreateTenantCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string LogoUrl { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? ParentTenantId { get; set; }
        public decimal ProfitMarginPercentage { get; set; } = 30.0m;
        public string UsAddress { get; set; }
    }

    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateTenantCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = new Tenant
            {
                Name = request.Name,
                Slug = request.Slug,
                LogoUrl = request.LogoUrl,
                PrimaryColor = request.PrimaryColor,
                SecondaryColor = request.SecondaryColor,
                IsActive = request.IsActive,
                ParentTenantId = request.ParentTenantId
            };

            _context.Tenants.Add(tenant);

            var tenantSettings = new TenantSettings
            {
                TenantId = tenant.Id,
                ProfitMarginPercentage = request.ProfitMarginPercentage,
                UsAddress = request.UsAddress
            };

            _context.TenantSettings.Add(tenantSettings);

            await _context.SaveChangesAsync(cancellationToken);

            return tenant.Id;
        }
    }

}
