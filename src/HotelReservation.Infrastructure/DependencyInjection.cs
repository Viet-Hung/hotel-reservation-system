using HotelReservation.Infrastructure.Persistence;
using HotelReservation.Domain.Hotels.Interfaces;
using HotelReservation.Domain.Reservations.Interfaces;
using HotelReservation.Domain.Users.Interfaces;
using HotelReservation.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelReservation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}