using AutoMapper;
using Booking.Application.DTO;
using Booking.Domain.Models;

namespace Booking.Application.Mapping;

public class RoomProfile : Profile
{
  public RoomProfile()
  {
    CreateMap<RoomCreate, Room>()
      .ForMember(x => x.Id, o => o.Ignore())
      .ForMember(x => x.Count, o => o.Equals(0))
      .ForMember(x => x.Reservations, o => o.Ignore());
    CreateMap<Room, RoomResponse>()
      .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
      .ForCtorParam("Capacity", opt => opt.MapFrom(src => src.Capacity))
      .ForCtorParam("Count", opt => opt.MapFrom(src => src.Count));
  }  
}