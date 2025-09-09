using Booking.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers.ReservationControllers;

[ApiController]
[Route("reservation/user")]
public class ReservationUserController(ReservationService reservationService) : ControllerBase
{
  private readonly ReservationService _reservationService = reservationService;
  [HttpGet]
  public async Task<IActionResult> GetByUsername([FromQuery] string username)
  {
    return Ok(await _reservationService.GetByUsername(username));
  }
}