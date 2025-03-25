using AutoMapper;
using FastyBox.Application.Common.Extensions;
using FastyBox.Application.Common.Interfaces;
using FastyBox.Application.Common.Models;
using FastyBox.Application.Shipments.Queries.GetUserShipments;
using FastyBox.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastyBox.Application.Shipments.Queries.GetAdminShipments
{
    public class GetAdminShipmentsQuery : IRequest<PaginatedList<ShipmentBriefDto>>
    {
        public Guid? TenantId { get; set; }
        public ShipmentStatus? Status { get; set; }
        public ShipmentType? Type { get; set; }
        public string SearchString { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetAdminShipmentsQueryHandler : IRequestHandler<GetAdminShipmentsQuery, PaginatedList<ShipmentBriefDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAdminShipmentsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedList<ShipmentBriefDto>> Handle(GetAdminShipmentsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Shipments
                .Include(s => s.User)
                .AsQueryable();

            if (request.TenantId.HasValue)
            {
                query = query.Where(s => s.TenantId == request.TenantId);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(s => s.Status == request.Status);
            }

            if (request.Type.HasValue)
            {
                query = query.Where(s => s.Type == request.Type);
            }

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var search = request.SearchString.Trim().ToLower();
                query = query.Where(s =>
                    s.TrackingNumber.ToLower().Contains(search) ||
                    s.ReferenceNumber.ToLower().Contains(search) ||
                    s.User.FirstName.ToLower().Contains(search) ||
                    s.User.LastName.ToLower().Contains(search) ||
                    s.User.Email.ToLower().Contains(search));
            }

            // Order by creation date, newest first
            query = query.OrderByDescending(s => s.CreatedAt);

            var shipments = await query
                .Select(s => new ShipmentBriefDto
                {
                    Id = s.Id,
                    TrackingNumber = s.TrackingNumber,
                    Type = s.Type,
                    Status = s.Status,
                    DeclaredValue = s.DeclaredValue,
                    TotalCost = s.TotalCost,
                    IsPaid = s.IsPaid,
                    CreatedAt = s.CreatedAt,
                    User = new UserBriefDto
                    {
                        Id = s.User.Id,
                        FullName = $"{s.User.FirstName} {s.User.LastName}",
                        Email = s.User.Email
                    }
                })
                .PaginatedListAsync(request.PageNumber, request.PageSize);

            return shipments;
        }
    }
}