using apbdcw12.Models;
using apbdcw12.Repositories;
using apbdcw12.Services;
using Microsoft.EntityFrameworkCore;

namespace apbdcw12;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        
        // Rejestracja zależności:
        builder.Services.AddScoped<ITripsService, TripsService>();
        builder.Services.AddScoped<ITripsRepository, TripsRepository>();
        builder.Services.AddDbContext<TripsDbContext>(opt =>
        {
            var connectionString = builder.Configuration.GetConnectionString("Default");
            opt.UseSqlServer(connectionString);
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        
        app.Run();
    }
}