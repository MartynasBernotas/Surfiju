using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class UpdateCampCommandHandler : IRequestHandler<UpdateCampCommand, bool>
    {
        private readonly DevDatabaseContext _context;

        public UpdateCampCommandHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateCampCommand request, CancellationToken cancellationToken)
        {
            var camp = await _context.Camps.FindAsync(request.Id);
            if (camp == null)
            {
                return false;
            }

            // Verify that the organizer is authorized to update this camp
            if (camp.OrganizerId != request.OrganizerId)
            {
                return false;
            }

            camp.Name = request.Name;
            camp.StartDate = request.StartDate.ToUniversalTime();
            camp.EndDate = request.EndDate.ToUniversalTime();
            camp.Description = request.Description;
            camp.MaxParticipants = request.MaxParticipants;
            camp.Price = request.Price;
            camp.Location = request.Location;
            camp.PhotoUrls = request.PhotoUrls;
            camp.IsPublic = request.IsPublic;

            _context.Update(camp);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
