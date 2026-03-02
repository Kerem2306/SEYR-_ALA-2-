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

            // API isteği: Birim olarak 'metric' (Celsius) ekliyoruz
            var url = $"{baseUrl}weather?q={cityName}&appid={apiKey}&units=metric&lang=tr";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<dynamic>();

                // JSON'dan gelen veriyi güvenli bir şekilde çekiyoruz
                double? temp = data?.GetProperty("main").GetProperty("temp").GetDouble();
                string? condition = data?.GetProperty("weather")[0].GetProperty("description").GetString();

                return (temp, condition);
            }

            return (null, null);
        }
    }
}