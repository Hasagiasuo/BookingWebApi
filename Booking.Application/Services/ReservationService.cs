using Booking.Application.Errors;
using Booking.Application.Interfaces.Repositories;
using AutoMapper;
using Booking.Application.DTO;
using Booking.Domain.Models;

namespace Booking.Application.Services;

public class ReservationService(
  IReservationRepository reservationRepository,
  IRoomRepository roomRepository,
  IUserRepository userRepository,
  IMapper mapper)
{
  private readonly IReservationRepository _reservationRepository = reservationRepository;
  private readonly IUserRepository _userRepository = userRepository;
  private readonly IRoomRepository _roomRepository = roomRepository;
  private readonly IMapper _mapper = mapper;

  private ICollection<ReservationResponse> ConvertToResponse(ICollection<Reservation> reservs)
  {
    List<ReservationResponse> result = new(reservs.Count);
    foreach (Reservation reserv in reservs)
    {
      ReservationResponse tmp = _mapper.Map<ReservationResponse>(reserv);
      result.Add(tmp);
    }
    return result; 
  }

  public async Task<Result<bool>> Create(ReservationCreate dto, string username)
  {
    if (dto.Start < DateTime.Now)
      return Result<bool>.Fail(new ValidationError("Start should be more", "Start"));

    if (dto.Start >= dto.End)
        return Result<bool>.Fail(new ValidationError("Start reservation cannot be biggest than end", "start/end"));

    User? user = await _userRepository.GetByUsername(username);
    if (user == null)
      return Result<bool>.Fail(new DataBaseError("Not found user", "Users"));

    Room? room = await _roomRepository.GetByTitle(dto.RoomTitle);
    if (room == null)
      return Result<bool>.Fail(new DataBaseError("Not found room", "Rooms"));

    if (dto.Count + room.Count > room.Capacity)
      return Result<bool>.Fail(new ValidationError("Room is full", "Capacity"));
    room.Count += dto.Count;


    Reservation reservation = new()
    {
      User = user,
      UserId = user.Id,
      Room = room,
      RoomId = room.Id,
      Start = dto.Start,
      End = dto.End
    };

    ICollection<Reservation> roomReservs = await GetByRoomTitle(dto.RoomTitle);

    ICollection<Reservation> uncollised = [.. roomReservs.Where(x =>
      x.Start < reservation.End &&
      x.End > reservation.Start
    )];
    if (uncollised.Count > 0) return Result<bool>.Fail(new IntervalShedulingError("This timeline already busy"));

    if (!await _reservationRepository.Add(reservation)) return Result<bool>.Fail(new DataBaseError("Cannot create reservation", "Reservations"));
    return Result<bool>.Ok(true);
  }
  public async Task<ICollection<ReservationResponse>> GetAll()
  {
    ICollection<Reservation> reservs = await _reservationRepository.GetAll();
    return ConvertToResponse(reservs);
  }
  public async Task<ICollection<ReservationResponse>> GetByUsername(string username)
  {
    ICollection<Reservation> reservs = await _reservationRepository.GetAll();
    return [.. reservs
      .Where(x => x.User.Username == username)
      .Select(x => _mapper.Map<ReservationResponse>(x))
    ];
  }
  public async Task<Result<ICollection<ReservationResponse>>> GetIntoInterval(ReservationByInterval dto)
  {
    ICollection<Reservation> reservs = await _reservationRepository.GetAll();
    if (reservs.Count == 0) return Result<ICollection<ReservationResponse>>.Fail(new DataBaseError("Cannot get reservations", "Reservations"));
    ICollection<ReservationResponse> unvalidation = [.. reservs
      .Where(x => x.Start >= dto.Start && x.End <= dto.End && x.Room.Count < x.Room.Capacity)
      .Select(r => _mapper.Map<ReservationResponse>(r))
    ];
    return Result<ICollection<ReservationResponse>>.Ok(unvalidation);
  }
  public async Task<ICollection<Reservation>> GetByRoomTitle(string roomTitle)
  {
    ICollection<Reservation> reservs = await _reservationRepository.GetAll();
    return [.. reservs.Where(x => x.Room.Title == roomTitle)];
  }
  public async Task<Result<Reservation>> GetById(Guid id)
  {
    Reservation? reserv = await _reservationRepository.GetById(id);
    if (reserv == null) return Result<Reservation>.Fail(new DataBaseError("Cannot get reservation by id", "Reservation"));
    return Result<Reservation>.Ok(reserv!);
  }
}