using Microsoft.EntityFrameworkCore;
using SEYRİ_ALA.Data.Interfaces;
using SEYRİ_ALA.Models;

namespace SEYRİ_ALA.Data.Repositories
{
    // DİKKAT: Burası "class" olmalı, "interface" değil!
    public class CityRepository : ICityRepository
    {
        private readonly ApplicationDbContext _context;

        // Class olduğu için artık Constructor (Yapıcı Metot) kullanabiliriz
        public CityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<City>> GetAllAsync() => await _context.Cities.ToListAsync();

        public async Task<City?> GetByIdAsync(int id) => await _context.Cities
                .Include(c => c.Comments).ThenInclude(u => u.User)
                .Include(c => c.Favorites)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<bool> IsFavoriteAsync(int cityId, int userId) =>
            await _context.Favorites.AnyAsync(f => f.CityId == cityId && f.UserId == userId);

        public async Task AddCommentAsync(Comment comment) => await _context.Comments.AddAsync(comment);

        public async Task<Comment?> GetCommentByIdAsync(int id) =>
            await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

        public async Task DeleteCommentAsync(Comment comment) => _context.Comments.Remove(comment);

        public async Task ToggleFavoriteAsync(int cityId, int userId)
        {
            var fav = await _context.Favorites.FirstOrDefaultAsync(f => f.CityId == cityId && f.UserId == userId);
            if (fav != null) _context.Favorites.Remove(fav);
            else await _context.Favorites.AddAsync(new Favorite { CityId = cityId, UserId = userId });
        }
        public async Task<IEnumerable<City>> GetFavoritesByUserIdAsync(int userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.City) // Şehir bilgilerini de beraberinde getir
                .Select(f => f.City)  // Sadece şehirleri seç
                .ToListAsync();
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        // ICityRepository içinde varsa ama burada yoksa hata verir, 
        // Interface'indeki tüm metotların burada olduğundan emin ol.
        public async Task<bool> AddFavoriteAsync(int cityId, int userId) { throw new NotImplementedException(); }
        public async Task<bool> RemoveFavoriteAsync(int cityId, int userId) { throw new NotImplementedException(); }
    }
}