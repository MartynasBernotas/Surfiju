using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class DeleteCampCommandHandler : IRequestHandler<DeleteCampCommand, bool>
    {
        private readonly DevDatabaseContext _context;

        public DeleteCampCommandHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCampCommand request, CancellationToken cancellationToken)
        {
            var camp = await _context.Camps.FindAsync(request.CampId);
            if (camp == null)
            {
                return false;
            }

            // Verify that the organizer is authorized to delete this camp
            if (camp.OrganizerId != request.OrganizerId)
            {
                return false;
            }

            _context.Camps.Remove(camp);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
