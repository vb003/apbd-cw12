using apbdcw12.DTOs;
using apbdcw12.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace apbdcw12.Repositories;

public class TripsRepository : ITripsRepository
{
    private readonly TripsDbContext _context;

    public TripsRepository(TripsDbContext context)
    {
        _context = context;
    }
    
    // do 1:
    public int GetTripsCount() => _context.Trips.Count();
    public async Task<PagedTripResponse> GetTrips(int page = 1, int pageSize = 10)
    {
        var allTripsCount = GetTripsCount();
        var allPages = (int)Math.Ceiling((double)allTripsCount/pageSize);
        
        var trips = await _context.Trips.Select(t =>
            new TripResponse
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c=> new CountryResponse
                {
                    Name = c.Name
                }).ToList(),
                Clients = t.ClientTrips.Select(ct => new ClientResponse
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            }
            )
            .OrderByDescending(t=>t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = new PagedTripResponse
        {
            allPages = allPages,
            pageNum = page,
            pageSize = pageSize,
            trips = trips
        };
        return response;
    }
    
    // do 2:
    public Task<Client?> GetClientById(int id)
    {
        return _context.Clients
            .Include(c=>c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == id);
    }
    public async Task<bool> DeleteClient(Client client)
    {
        _context.Clients.Remove(client);
        
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return _context.Database.BeginTransactionAsync();
    }

    public Task<Client?> GetClientByPesel(string pesel)
    {
        return _context.Clients.FirstOrDefaultAsync(c => c.Pesel == pesel);
    }

    public Task<Trip?> GetTripById(int idTrip)
    {
        return _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);
    }

    public async Task<int> AddClient(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client.IdClient;
    }

    public async Task<ClientTrip?> GetRegistration(string pesel, int idTrip)
    {
        var client = await GetClientByPesel(pesel);
        return await _context.ClientTrips.FirstOrDefaultAsync(ct => ct.IdTrip == idTrip && ct.IdClient == client.IdClient); //!!
    }

    public async Task<bool> AddRegistration(ClientTrip clientTrip)
    {
        _context.ClientTrips.Add(clientTrip);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }
}