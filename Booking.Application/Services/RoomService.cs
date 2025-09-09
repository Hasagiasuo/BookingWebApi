using AutoMapper;
using Booking.Application.DTO;
using Booking.Application.Errors;
using Booking.Application.Interfaces.Repositories;
using Booking.Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Booking.Application.Services;

public class RoomService(IRoomRepository roomRepository, IMapper mapper, IMemoryCache cache)
{
  private readonly IRoomRepository _roomRepository = roomRepository;
  private readonly IMemoryCache _cache = cache;
  private readonly IMapper _mapper = mapper;

  private ICollection<RoomResponse> ConvertToResponse(ICollection<Room> rooms)
  {
    return [.. rooms.Select(x => _mapper.Map<RoomResponse>(x))];
  }

  public async Task<Result<bool>> Create(RoomCreate dto)
  {
    Room room = _mapper.Map<Room>(dto);
    if (await _roomRepository.Add(room)) return Result<bool>.Ok(true);
    else return Result<bool>.Fail(new DataBaseError("Cannot add room", "Rooms"));
  }
  public async Task<ICollection<RoomResponse>> GetAll()
  {
    if (_cache.TryGetValue("rooms", out ICollection<RoomResponse>? rooms))
      return rooms!;
    ICollection<RoomResponse> newRooms = ConvertToResponse(await _roomRepository.GetAll());
    _cache.Set("rooms", newRooms, TimeSpan.FromMinutes(5));
    return newRooms;
  }
  public async Task DiscardCount(string roomTitle = "")
  {
    ICollection<Room> rooms = await _roomRepository.GetAll();
    if (roomTitle == "") foreach (Room room in rooms) room.Count = 0;
    else foreach (Room room in rooms) if (room.Title == roomTitle) room.Count = 0;
  }
}