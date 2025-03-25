using FastyBox.Application.Common.Interfaces;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FastyBox.Application.Users.CreateUser
{
    public class CreateUserCommand : IRequest<string>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public UserType UserType { get; set; }
        public Guid? TenantId { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public CreateUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                TenantId = request.TenantId,
                UserType = request.UserType,
                EmailConfirmed = true // For simplicity; in production, use email confirmation
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new Exception($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Assign role based on user type
            var roleName = request.UserType.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, roleName);

            // Send welcome email
            await _emailService.SendEmailTemplateAsync(
                user.Email,
                "WelcomeEmail",
                new { user.FirstName, user.LastName, user.Email },
                cancellationToken);

            return user.Id;
        }
    }
}
