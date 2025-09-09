namespace Booking.Application.DTO;

public record RoomCreate(string Title, int Capacity);

public record RoomResponse(string Title, int Count, int Capacity);