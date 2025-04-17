using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public class DeleteCampCommand : IRequest<bool>
    {
        public Guid CampId { get; set; }
        public string OrganizerId { get; set; } = string.Empty;

        public DeleteCampCommand(Guid campId, string organizerId)
        {
            CampId = campId;
            OrganizerId = organizerId;
        }
    }
}
