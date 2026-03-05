namespace SEYRİ_ALA.Services
{
    public interface IWeatherService
    {
        Task<(double? Temp, string? Condition)> GetWeatherAsync(string cityName);
    }
}