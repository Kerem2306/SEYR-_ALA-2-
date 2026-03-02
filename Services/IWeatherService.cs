namespace SEYRİ_ALA.Services
{
    public interface IWeatherService
    {
        // Sadece tanım yapıyoruz, gövde (body) yok!
        Task<(double? Temp, string? Condition)> GetWeatherAsync(string cityName);
    }
}