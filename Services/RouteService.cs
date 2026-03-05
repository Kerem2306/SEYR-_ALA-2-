using Microsoft.EntityFrameworkCore;
using SEYRİ_ALA.Data;
using SEYRİ_ALA.Models;

namespace SEYRİ_ALA.Services
{
    public class RouteService : IRouteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistanceService _distanceService;

        public RouteService(ApplicationDbContext context, IDistanceService distanceService)
        {
            _context = context;
            _distanceService = distanceService;
        }

        // 1. METOT: Mesafeyi hesaplayan kısım (Interface'den gelen)
        public async Task<double> CalculateTotalDistanceAsync(List<int> cityIds)
        {
            var cities = await _context.Cities
                .Where(c => cityIds.Contains(c.Id))
                .ToListAsync();

            // ID sırasına göre şehirleri diziyoruz
            var orderedCities = cityIds.Select(id => cities.First(c => c.Id == id)).ToList();

            double totalDistance = 0;

            for (int i = 0; i < orderedCities.Count - 1; i++)
            {
                totalDistance += _distanceService.CalculateDistance(
                    orderedCities[i].Latitude, orderedCities[i].Longitude,
                    orderedCities[i + 1].Latitude, orderedCities[i + 1].Longitude);
            }

            return totalDistance;
        }

        // 2. METOT: Veritabanına kaydeden kısım
        public async Task<int> CreateRouteAsync(string routeName, List<int> cityIds)
        {
            // Hata aldığın yer burasıydı, artık yukarıdaki metodu görebilecek
            double totalDistance = await CalculateTotalDistanceAsync(cityIds);

            var newRoute = new TravelRoute
            {
                Title = routeName,
                TotalDistance = totalDistance,
                Description = "Hafta 3 Test Rotası" // Modelindeki zorunlu alanlar
            };

            _context.Routes.Add(newRoute);
            await _context.SaveChangesAsync();

            return newRoute.Id;
        }
    }
}