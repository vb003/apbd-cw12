using apbdcw12.DTOs;
using apbdcw12.Exceptions;
using apbdcw12.Models;
using apbdcw12.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace apbdcw12.Controllers;

[ApiController]
[Route("api")]
public class TripsController : ControllerBase
{
    private ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }
    
    // Końcówka 1: Zwróć listę wycieczek posortowanych malejąco po dacie rozpoczęcia wycieczki
    // + opcjonalna możliwość stronicowania wyniku z pomocą query stringa i parametrów page i pageSize
    // domyślny pageSize = 10
    // Przykład żądania z opcjonalnymi parametrami: (...)/api/trips?page=6&pageSize=2
    [HttpGet("trips")]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var trips = await _tripsService.GetTrips(page, pageSize);
            
            return Ok(trips);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // Końcówka 2: Usuń klienta
    // + serwer powinien wpierw sprawdzić, czy klient nie ma przypisanych żadnych wycieczek. 
    //      Jeśli klient ma co najmniej 1 wycieczkę, to zwracamy kod błędu z wiadomością

    [HttpDelete("clients/{id:int}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        try
        {
            var result = await _tripsService.DeleteClient(id);
            if (result)
                return Ok("Successfully deleted client");
            return BadRequest("Failed to delete client");
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // Końcówka 3: Przypisz klienta do wycieczki
    [HttpPost("trips/{idTrip:int}/clients")]
    public async Task<IActionResult> AssignClientToTrip([FromBody] ClientTripRequest clientTripRequest)
    {
        try
        {
            await _tripsService.AssignClientToTrip(clientTripRequest);
            return Ok("Successfully assigned client to trip");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}