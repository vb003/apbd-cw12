using apbdcw12.Models;

namespace apbdcw12.DTOs;

public class PagedTripResponse
{
    public int pageNum { get; set; }
    public int pageSize { get; set; }
    public int allPages { get; set; }
    public List<TripResponse> trips { get; set; }
}

public class TripResponse
{
    public string Name {get; set;}
    public string Description {get; set;}
    public DateTime DateFrom {get; set;}
    public DateTime DateTo {get; set;}
    public int MaxPeople {get; set;}
    
    public List<CountryResponse> Countries {get; set;}
    public List<ClientResponse> Clients {get; set;}
}

public class ClientResponse
{
    public string FirstName {get; set;}
    public string LastName {get; set;}
}

public class CountryResponse
{
    public string Name {get; set;}
}

