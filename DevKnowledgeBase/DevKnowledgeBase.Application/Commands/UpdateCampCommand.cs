using DevKnowledgeBase.Domain.Dtos;
using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public class UpdateCampCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> PhotoUrls { get; set; } = new();
        public bool IsPublic { get; set; }
        public string OrganizerId { get; set; } = string.Empty;

        public UpdateCampCommand(UpdateCampDto dto, string organizerId)
        {
            Id = dto.Id;
            Name = dto.Name;
            StartDate = dto.StartDate;
            EndDate = dto.EndDate;
            Description = dto.Description;
            MaxParticipants = dto.MaxParticipants;
            Price = dto.Price;
            Location = dto.Location;
            PhotoUrls = dto.PhotoUrls;
            IsPublic = dto.IsPublic;
            OrganizerId = organizerId;
        }
    }
}
