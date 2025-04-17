using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Domain.Entities;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class CreateCampCommandHandler : IRequestHandler<CreateCampCommand, Guid>
    {
        private readonly DevDatabaseContext _context;

        public CreateCampCommandHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateCampCommand request, CancellationToken cancellationToken)
        {
            var camp = new Camp
            {
                Name = request.Name,
                StartDate = request.StartDate.ToUniversalTime(),
                EndDate = request.EndDate.ToUniversalTime(),
                Description = request.Description,
                MaxParticipants = request.MaxParticipants,
                Price = request.Price,
                Location = request.Location,
                PhotoUrls = request.PhotoUrls,
                OrganizerId = request.OrganizerId,
                IsPublic = true
            };

            _context.Camps.Add(camp);
            await _context.SaveChangesAsync(cancellationToken);

            return camp.Id;
        }
    }
}
