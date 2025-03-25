using AutoMapper;
using FastyBox.Application.Common.Exceptions;
using FastyBox.Application.Common.Interfaces;
using FastyBox.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastyBox.Application.Shipments.Queries.GetShipmentById
{
    public class GetShipmentByIdQuery : IRequest<Shipment>
    {
        public Guid Id { get; set; }
    }

    public class GetShipmentByIdQueryHandler : IRequestHandler<GetShipmentByIdQuery, Shipment>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetShipmentByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Shipment> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
        {
            var shipment = await _context.Shipments
                .Include(s => s.User)
                .Include(s => s.SourceAddress)
                .Include(s => s.DestinationAddress)
                .Include(s => s.Items)
                .Include(s => s.Documents)
                .Include(s => s.StatusHistory)
                .Include(s => s.Notes.Where(n => n.IsPublic))
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shipment == null)
            {
                throw new NotFoundException(nameof(Shipment), request.Id);
            }

            return _mapper.Map<Shipment>(shipment);
        }
    }

}
