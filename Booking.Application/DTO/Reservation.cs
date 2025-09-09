namespace Booking.Application.DTO;

// Input
public record ReservationCreate(string RoomTitle, int Count, DateTime Start, DateTime End);
public record ReservationByInterval(DateTime Start, DateTime End);

// Output
public record ReservationResponse(string Username, string RoomTitle, DateTime Start, DateTime End);