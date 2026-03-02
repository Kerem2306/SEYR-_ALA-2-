using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SEYRİ_ALA.Data.Interfaces; // Repository arayüzleri için
using SEYRİ_ALA.Models;
using SEYRİ_ALA.Models.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace SEYRİ_ALA.Controllers
{
    public class HomeController : Controller
    {
        // Artik Context (DB) ile degil, Repository ile konusuyoruz
        private readonly ICityRepository _cityRepository;
        private readonly ILogger<HomeController> _logger;

        // Dependency Injection ile Repository'yi sisteme dahil ediyoruz
        public HomeController(ILogger<HomeController> logger, ICityRepository cityRepository)

        {
            _logger = logger;
            _cityRepository = cityRepository;
        }

        // --- DİL DEĞİŞTİRME ---
        [HttpPost]
        public IActionResult ChangeLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }

        // --- ANA SAYFA: Şehirleri Listeler ---
        public async Task<IActionResult> Index()
        {
            // Veritabanindan tum sehirleri Repository araciligiyla cekiyoruz
            var cities = await _cityRepository.GetAllAsync();
            return View(cities);
        }

        // --- DETAY SAYFASI: Şehir Detayları, Yorumlar ve Favoriler ---
       
            public async Task<IActionResult> Details(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null) return NotFound();

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdString, out int userId);

            // MODEL -> VIEWMODEL DÖNÜŞÜMÜ (Mühürleme işlemi)
            var viewModel = new CityDetailsViewModel
            {
                Id = city.Id,
                Name = city.Name,
                Description = city.Description,
                HistoryScore = city.HistoryScore,
                NatureScore = city.NatureScore,
                FoodScore = city.FoodScore,
                Latitude = city.Latitude,
                Longitude = city.Longitude,
                IsFavorite = city.Favorites.Any(f => f.UserId == userId),
                Comments = city.Comments.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    UserName = c.User?.FullName ?? "Anonim",
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    IsBelongsToUser = c.UserId == userId
                }).ToList()
            };

            return View(viewModel);
        } // <--- Detay metodu burada kapandi

        // --- YORUM EKLEME ---
        [HttpPost]
        [Authorize] // Sadece giris yapanlar yorum yapabilir
        public async Task<IActionResult> AddComment(int CityId, string Content)
        {
            if (string.IsNullOrWhiteSpace(Content))
                return RedirectToAction("Details", new { id = CityId });

            // Giris yapan kullanicinin ID'sini Claim uzerinden aliyoruz
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdString, out int userId))
            {
                var comment = new Comment
                {
                    CityId = CityId,
                    UserId = userId,
                    Text = Content,
                    CreatedAt = DateTime.Now,
                    IsApproved = true
                };

                // Repository uzerinden kayit islemi
                await _cityRepository.AddCommentAsync(comment);
                await _cityRepository.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = CityId });
        }

        // --- FAVORİ EKLEME / ÇIKARMA ---
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int cityId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                // Varsa siler, yoksa ekler (Repository icinde kurguladik)
                await _cityRepository.ToggleFavoriteAsync(cityId, userId);
                await _cityRepository.SaveChangesAsync();
                TempData["FavoriteMessage"] = "Favori durumunuz güncellendi!";
            }
            return RedirectToAction("Details", new { id = cityId });
        }

        // --- YORUM SİLME ---
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Silinecek yorumu bul
            var comment = await _cityRepository.GetCommentByIdAsync(id);

            // Yorum varsa ve silmek isteyen kisi yorumun sahibiyse sil
            if (comment != null && userIdString != null && comment.UserId.ToString() == userIdString)
            {
                await _cityRepository.DeleteCommentAsync(comment);
                await _cityRepository.SaveChangesAsync();
                TempData["FavoriteMessage"] = "Yorumunuz silindi.";
            }
            return RedirectToAction("Details", new { id = comment?.CityId });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard() => View();

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        // ------FAVORİ ŞEJİRLER SAYFASI---------
        [Authorize] // Sadece giriş yapanlar görebilir
        public async Task<IActionResult> Favorites()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var favorites = await _cityRepository.GetFavoritesByUserIdAsync(userId);
                return View(favorites);
            }
            return RedirectToAction("Login", "Account");
        }


    } // <--- HomeController SINIFI BURADA KAPANIYOR
}