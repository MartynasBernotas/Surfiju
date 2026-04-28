using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Commands
{
    public class DeleteSurfSpotCommandHandler : IRequestHandler<DeleteSurfSpotCommand, bool>
    {
        private readonly DevDatabaseContext _context;

        public DeleteSurfSpotCommandHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteSurfSpotCommand request, CancellationToken cancellationToken)
        {
            var spot = await _context.SurfSpots.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (spot == null) return false;

            _context.SurfSpots.Remove(spot);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
