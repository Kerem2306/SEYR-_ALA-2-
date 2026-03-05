using Microsoft.AspNetCore.Mvc;
using SEYRİ_ALA.Services;

namespace SEYRİ_ALA.Controllers
{
    public class RouteController : Controller
    {
        private readonly IRouteService _routeService;

        // Constructor: Servisi burada içeri alıyoruz
        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        // SENİN SORDUĞUN TEST METODU BURASI:
        [HttpGet("test-route")]
        public async Task<IActionResult> TestRoute()
        {
            // Test için veritabanındaki Adana(1) ve Ankara(6) ID'lerini kullanıyoruz
            // Senin senaryonda bunlar şehir içindeki mekan ID'leri olacak
            var ids = new List<int> { 1, 6 };

            var distance = await _routeService.CalculateTotalDistanceAsync(ids);

            return Ok(new
            {
                Mesaj = "Mesafe başarıyla hesaplandı!",
                HesaplananKM = distance
            });
        }
    }
}