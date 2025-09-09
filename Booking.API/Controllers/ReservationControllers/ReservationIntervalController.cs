using Booking.Application.DTO;
using Booking.Application.Errors;
using Booking.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers.ReservationControllers;

[ApiController]
[Route("reservation/interval")]
public class ReservationIntervalController(ReservationService reservationService) : ControllerBase
{
  private readonly ReservationService _reservationService = reservationService;
  [HttpGet]
  public async Task<IActionResult> GetByInterval([FromBody] ReservationByInterval dto)
  {
    Result<ICollection<ReservationResponse>> res = await _reservationService.GetIntoInterval(dto);
    if (res.IsSuccess) return Ok(res.Value);
    else return StatusCode(500, new { error = res.Error!.ToResponse() });
  }
}