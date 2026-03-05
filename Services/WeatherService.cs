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
                var data = await response.Content.ReadFromJsonAsync<dynamic>();
                double? temp = (double?)data?.main?.temp;
                string? condition = (string?)data?.weather[0]?.description;
                return (temp, condition);
            }
            return (null, null);
        }
    }
}