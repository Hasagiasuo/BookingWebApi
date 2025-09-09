using Booking.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers.RoomControllers;

[ApiController]
[Route("room/controll")]
public class RoomControllContoller(RoomService roomService) : ControllerBase
{
  private readonly RoomService _roomService = roomService;
  [HttpPost]
  public async Task<IActionResult> DiscardRooms()
  {
    await _roomService.DiscardCount();
    return Ok();
  }
}