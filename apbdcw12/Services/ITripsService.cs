using apbdcw12.DTOs;

namespace apbdcw12.Services;

public interface ITripsService
{
    Task<PagedTripResponse> GetTrips(int page = 1, int pageSize = 10);
    Task<bool> DeleteClient(int id);
    Task<AssignClientResponse> AssignClientToTrip(ClientTripRequest clientTripRequest);
}