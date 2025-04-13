using DevKnowledgeBase.Domain.Dtos;
using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public class CreateTripCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> PhotoUrls { get; set; } = new();
        public string OrganizerId { get; set; } = string.Empty;
        public bool IsPublic { get; set; }

        public CreateTripCommand(CreateTripDto dto, string organizerId)
        {
            Name = dto.Name;
            StartDate = dto.StartDate;
            EndDate = dto.EndDate;
            Description = dto.Description;
            MaxParticipants = dto.MaxParticipants;
            Price = dto.Price;
            Location = dto.Location;
            PhotoUrls = dto.PhotoUrls;
            OrganizerId = organizerId;
            IsPublic = dto.IsPublic;
        }
    }
}
