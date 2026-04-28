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
            CreateMap<SurfSpot, SurfSpotDto>();
            CreateMap<SurfSpot, SurfSpotSummaryDto>();
            CreateMap<CreateSurfSpotDto, SurfSpot>();
            CreateMap<UpdateSurfSpotDto, SurfSpot>();
            CreateMap<Camp, CampDto>()
               .ForMember(dest => dest.CurrentParticipants,
                   opt => opt.MapFrom(src => src.Bookings.Sum(b => b.Participants)))
               .ForMember(dest => dest.OrganizerName,
                   opt => opt.MapFrom(src => src.Organizer.FullName));

            CreateMap<CreateCampDto, Camp>();
            CreateMap<UpdateCampDto, Camp>();

            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.CampName, opt => opt.MapFrom(src => src.Camp.Name))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Camp.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Camp.EndDate))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Camp.Location))
                .ForMember(dest => dest.CanCancel, opt => opt.MapFrom(src =>
                    src.Status == Domain.Enums.BookingStatus.Pending ||
                    (src.Status == Domain.Enums.BookingStatus.Confirmed && src.Camp.StartDate > DateTime.UtcNow.AddHours(48))));

        }
    }
}
