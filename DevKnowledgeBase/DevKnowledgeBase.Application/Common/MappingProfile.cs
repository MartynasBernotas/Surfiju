using AutoMapper;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Domain.Entities;

namespace DevKnowledgeBase.Application.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Note, NoteDto>();
            CreateMap<Camp, CampDto>()
               .ForMember(dest => dest.CurrentParticipants,
                   opt => opt.MapFrom(src => src.Members.Count))
               .ForMember(dest => dest.OrganizerName,
                   opt => opt.MapFrom(src => src.Organizer.FullName));

            CreateMap<CreateCampDto, Camp>();
            CreateMap<UpdateCampDto, Camp>();
        }
    }
}
