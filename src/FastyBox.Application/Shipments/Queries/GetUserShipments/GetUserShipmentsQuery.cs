using AutoMapper;
using AutoMapper.QueryableExtensions;
using FastyBox.Application.Common.Extensions;
using FastyBox.Application.Common.Interfaces;
using FastyBox.Application.Common.Models;
using MediatR;

namespace FastyBox.Application.Shipments.Queries.GetUserShipments
{
    public class GetUserShipmentsQuery : IRequest<PaginatedList<ShipmentBriefDto>>
    {
        public string UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetUserShipmentsQueryHandler : IRequestHandler<GetUserShipmentsQuery, PaginatedList<ShipmentBriefDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetUserShipmentsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedList<ShipmentBriefDto>> Handle(GetUserShipmentsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Shipments
                .Where(s => s.UserId == request.UserId)
                .OrderByDescending(s => s.CreatedAt)
                .AsQueryable();

            return await query
                .ProjectTo<ShipmentBriefDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }

}
