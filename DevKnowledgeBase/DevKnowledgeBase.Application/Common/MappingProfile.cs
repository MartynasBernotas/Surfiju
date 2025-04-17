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

            // Add to DevKnowledgeBase.Application/Common/MappingProfile.cs in the constructor
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.CampName, opt => opt.MapFrom(src => src.Camp.Name))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Camp.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Camp.EndDate))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Camp.Location));

        }
    }
}
