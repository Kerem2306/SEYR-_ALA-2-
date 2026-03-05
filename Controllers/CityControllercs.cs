using Microsoft.AspNetCore.Mvc;
using SEYRİ_ALA.Services;
using System;
using SEYRİ_ALA.Data; // Eğer Data klasöründeyse
using SEYRİ_ALA.Models; // Eğer Models klasöründeyse
using Microsoft.EntityFrameworkCore; // Async işlemler için bu da gereklidir

namespace SEYRİ_ALA.Controllers
{
    public class CityController : Controller
    {
        private readonly ApplicationDbContext _context; // Veritabanı bağlamın
        private readonly IWeatherService _weatherService; // Senin yazdığın servis

        // Constructor (Yapıcı Metot) içinde her ikisini de tanımlıyoruz
        public CityController(ApplicationDbContext context, IWeatherService weatherService)
        {
            _context = context;
            _weatherService = weatherService;
        }

        // ... Diğer metodlar (Index, Details vb.) buradadır ...

        // ŞİMDİ BU YENİ METODU EKLEYELİM:
        // "City" ön ekini buraya da ekleyerek yolu kesinleştiriyoruz
        [HttpGet("/City/update-weather/{id}")]
        public async Task<IActionResult> UpdateCityWeather(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null) return NotFound();

            var (temp, condition) = await _weatherService.GetWeatherAsync(city.Name);

            if (temp != null)
            {
                city.Temperature = temp;
                city.WeatherCondition = condition;
                await _context.SaveChangesAsync();
            }

            return Ok(new { city.Name, city.Temperature, city.WeatherCondition });
        }
    }
}
