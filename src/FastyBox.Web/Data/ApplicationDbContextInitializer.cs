using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;
using FastyBox.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastyBox.Web.Data
{
    public class ApplicationDbContextInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ApplicationDbContextInitializer> _logger;

        public ApplicationDbContextInitializer(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ApplicationDbContextInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            // Default roles
            var roles = new[]
            {
                UserType.SuperAdmin.ToString(),
                UserType.TenantAdmin.ToString(),
                UserType.TenantStaff.ToString(),
                UserType.Customer.ToString()
            };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation("Created role {Role}", role);
                }
            }

            //// Default tenant
            //var defaultTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Slug == "fastybox");
            //if (defaultTenant == null)
            //{
            //    defaultTenant = new Tenant
            //    {
            //        Name = "FastyBox",
            //        Slug = "fastybox",
            //        PrimaryColor = "#4CAF50",
            //        SecondaryColor = "#2196F3",
            //        IsActive = true,
            //        CreatedBy = "System",

            //    };

            //    _context.Tenants.Add(defaultTenant);
            //    await _context.SaveChangesAsync();
            //    _logger.LogInformation("Created default tenant {TenantName}", defaultTenant.Name);

            //    // Tenant settings
            //    var tenantSettings = new TenantSettings
            //    {
            //        TenantId = defaultTenant.Id,
            //        ProfitMarginPercentage = 30.0m,
            //        UsAddress = "123 Main Street, Suite 5, San Diego, CA 92154, USA"
            //    };

            //    _context.TenantSettings.Add(tenantSettings);
            //    await _context.SaveChangesAsync();
            //    _logger.LogInformation("Created default tenant settings");

            //    // Default pricing rules
            //    var pricingRules = new List<PricingRule>
            //    {
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 0, MaxWeight = 1, Price = 640, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 1, MaxWeight = 2, Price = 700, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 2, MaxWeight = 3, Price = 780, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 3, MaxWeight = 4, Price = 820, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 4, MaxWeight = 5, Price = 860, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 5, MaxWeight = 6, Price = 880, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 6, MaxWeight = 7, Price = 920, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 7, MaxWeight = 8, Price = 960, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 8, MaxWeight = 9, Price = 980, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 9, MaxWeight = 10, Price = 1100, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 10, MaxWeight = 11, Price = 1180, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 11, MaxWeight = 12, Price = 1200, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 12, MaxWeight = 13, Price = 1260, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 13, MaxWeight = 14, Price = 1300, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 14, MaxWeight = 15, Price = 1400, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 15, MaxWeight = 16, Price = 1480, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 16, MaxWeight = 17, Price = 1680, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 17, MaxWeight = 18, Price = 1720, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 18, MaxWeight = 19, Price = 1800, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 19, MaxWeight = 20, Price = 1980, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 20, MaxWeight = 21, Price = 2020, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 21, MaxWeight = 22, Price = 2100, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 22, MaxWeight = 23, Price = 2140, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 23, MaxWeight = 24, Price = 2220, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 24, MaxWeight = 25, Price = 2260, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 25, MaxWeight = 26, Price = 2290, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 26, MaxWeight = 27, Price = 2310, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 27, MaxWeight = 28, Price = 2360, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 28, MaxWeight = 29, Price = 2390, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 29, MaxWeight = 30, Price = 2410, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 30, MaxWeight = 31, Price = 2450, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 31, MaxWeight = 32, Price = 2480, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 32, MaxWeight = 33, Price = 2496, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 33, MaxWeight = 34, Price = 2520, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 34, MaxWeight = 35, Price = 2580, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 35, MaxWeight = 36, Price = 2620, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 36, MaxWeight = 37, Price = 2680, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 37, MaxWeight = 38, Price = 2720, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 38, MaxWeight = 39, Price = 2780, IsActive = true },
            //        new PricingRule { TenantId = defaultTenant.Id, MinWeight = 39, MaxWeight = 40, Price = 2852, IsActive = true }
            //    };

            //    _context.PricingRules.AddRange(pricingRules);
            //    await _context.SaveChangesAsync();
            //    _logger.LogInformation("Created default pricing rules");
            //}

            // Default admin user
            var adminEmail = "admin@fastybox.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    // TenantId = defaultTenant?.Id,
                    UserType = UserType.SuperAdmin
                };

                await _userManager.CreateAsync(adminUser, "Admin123!");
                await _userManager.AddToRoleAsync(adminUser, UserType.SuperAdmin.ToString());
                _logger.LogInformation("Created administrator user {Email}", adminEmail);
            }
        }
    }
}