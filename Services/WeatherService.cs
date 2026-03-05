using System.Net.Http.Json;

namespace SEYRİ_ALA.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<(double? Temp, string? Condition)> GetWeatherAsync(string cityName)
        {
            var apiKey = _configuration["WeatherApi:ApiKey"];
            var baseUrl = _configuration["WeatherApi:BaseUrl"];
            var url = $"{baseUrl}weather?q={cityName}&appid={apiKey}&units=metric&lang=tr";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                // 'dynamic' yerine JsonElement kullanarak veriyi daha sağlam okuyoruz
                var data = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();

                try
                {
                    // Veriyi güvenli bir şekilde çekiyoruz
                    double? temp = data.GetProperty("main").GetProperty("temp").GetDouble();
                    string? condition = data.GetProperty("weather")[0].GetProperty("description").GetString();

                    return (temp, condition);
                }
                catch (Exception)
                {
                    return (null, null);
                }
            }
            return (null, null);
        }
    }
}