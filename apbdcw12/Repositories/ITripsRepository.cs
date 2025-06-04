using apbdcw12.DTOs;
using apbdcw12.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace apbdcw12.Repositories;

public interface ITripsRepository
{
    // Do końcówki 1:
    Task<PagedTripResponse> GetTrips(int page = 1, int pageSize = 10);
    
    // Do końcówki 2:
    Task<Client?> GetClientById(int id);
    Task<bool> DeleteClient(Client client);
    
    //Do końcówki 3:
    Task<Client?> GetClientByPesel(string pesel);
    Task<Trip?> GetTripById(int idTrip);
    Task<int> AddClient(Client client);
    Task<ClientTrip?> GetRegistration(string pesel, int idTrip);
    Task<bool> AddRegistration(ClientTrip clientTrip);
    
    Task<IDbContextTransaction> BeginTransactionAsync();
}