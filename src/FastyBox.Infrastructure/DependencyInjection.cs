using FastyBox.Application.Common.Interfaces;
using FastyBox.Infrastructure.Persistence;
using FastyBox.Infrastructure.Services;
using FastyBox.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastyBox.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IFile, FileService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IShipmentService, ShipmentService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IAiService, AiService>();
            services.AddTransient<IWebhookService, WebhookService>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Configure settings
            services.Configure<StorageSettings>(configuration.GetSection("Storage"));
            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.Configure<AiSettings>(configuration.GetSection("Ai"));

            return services;
        }
    }
}
