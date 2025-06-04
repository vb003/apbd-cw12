using apbdcw12.DTOs;
using apbdcw12.Exceptions;
using apbdcw12.Models;
using apbdcw12.Repositories;

namespace apbdcw12.Services;

public class TripsService : ITripsService
{
    private readonly ITripsRepository _repository;

    public TripsService(ITripsRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedTripResponse> GetTrips(int page = 1, int pageSize = 10)
    {
        var trips = _repository.GetTrips(page, pageSize);
        return trips;
    }

    public async Task<bool> DeleteClient(int id)
    {
        var client = await  _repository.GetClientById(id);
        if (client == null)
            throw new NotFoundException("Client not found");

        if (client.ClientTrips.Count > 0)
            throw new ConflictException("Client is assigned to the trip. Client cannot be removed.");

        var result = await _repository.DeleteClient(client);
        return result;
    }

    public async Task<AssignClientResponse> AssignClientToTrip(ClientTripRequest clientTripRequest)
    {
        await using var transaction = await _repository.BeginTransactionAsync();
        try
        {
            // 1. Sprawdź, czy klient o tym peselu już istnieje:
            var client = await _repository.GetClientByPesel(clientTripRequest.Pesel);

            if (client != null)
            {
                throw new Exception("Client already exists.");
            }

            var newClient = new Client
            {
                FirstName = clientTripRequest.FirstName,
                LastName = clientTripRequest.LastName,
                Email = clientTripRequest.Email,
                Telephone = clientTripRequest.Telephone,
                Pesel = clientTripRequest.Pesel
            };
            int idClient = await _repository.AddClient(newClient);

            // 2. Sprawdź, czy ta wycieczka istnieje:
            var trip = await _repository.GetTripById(clientTripRequest.IdTrip);
            if (trip == null)
            {
                throw new Exception("Trip not found.");
            }

            DateTime currentDate = DateTime.UtcNow;

            // 3. Sprawdź, czy DateFrom jest w przyszłości
            if (trip.DateFrom <= currentDate)
            {
                throw new Exception("Trip has already taken place.");
            }

            // 3. Sprawdź, czy klient o tym peselu już jest zarejestrowany na tę wycieczkę:
            var registration = await _repository.GetRegistration(clientTripRequest.Pesel, clientTripRequest.IdTrip);
            if (registration != null)
            {
                throw new Exception("Registration already exists.");
            }

            var newRegistration = new ClientTrip
            {
                IdClient = idClient,
                IdTrip = clientTripRequest.IdTrip,
                RegisteredAt = currentDate,
                PaymentDate = clientTripRequest.PaymentDate // ?
            };
            var addRegistrationResult = await _repository.AddRegistration(newRegistration);
            if (!addRegistrationResult)
            {
                throw new Exception("Registration could not be added.");
            }

            await transaction.CommitAsync();
            
            return new AssignClientResponse
            {
                IdClient = idClient,
                IdTrip = clientTripRequest.IdTrip,
                RegisteredAt = currentDate,
                PaymentDate = clientTripRequest.PaymentDate
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}