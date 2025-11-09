using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using WeatherNotesApp.Models;
using System.Text.Json;

namespace WeatherNotesApp.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "01ac7e1a9e973cae6f53ed1a508a6959";
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";
        private const string ForecastUrl = "https://api.openweathermap.org/data/2.5/forecast";

        public WeatherService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<WeatherInfo?> GetWeatherAsync(string city)
        {
            try
            {
                string url = $"{BaseUrl}?q={city}&appid={ApiKey}&units=metric&lang=ro";
                var response = await _httpClient.GetFromJsonAsync<OpenWeatherResponse>(url);

                if (response == null) return null;

                return new WeatherInfo
                {
                    CityName = response.name,
                    Temperature = response.main.temp,
                    Description = response.weather[0].description,
                    Humidity = response.main.humidity,
                    WindSpeed = response.wind.speed
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<ForecastDay>?> GetFiveDayForecastAsync(string city)
        {
            try
            {
                string url = $"{ForecastUrl}?q={city}&appid={ApiKey}&units=metric&lang=ro";

                var json = await _httpClient.GetStringAsync(url);
                if (string.IsNullOrWhiteSpace(json)) return null;

                OpenWeatherForecastResponse? response = null;
                try
                {
                    response = JsonSerializer.Deserialize<OpenWeatherForecastResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (JsonException jex)
                {
                    System.Diagnostics.Debug.WriteLine($"Forecast JSON parse error: {jex.Message}");
                    System.Diagnostics.Debug.WriteLine(json);
                    return null;
                }

                if (response == null || response.list == null) return null;

                var grouped = response.list
                    .Select(i => new { Item = i, Date = ParseDate(i.dt_txt) })
                    .Where(x => x.Date.HasValue)
                    .GroupBy(x => x.Date.Value.Date)
                    .Select(g => new ForecastDay
                    {
                        Date = g.Key,
                        Temp = Math.Round(g.Average(x => x.Item.main.temp), 1)
                    })
                    .OrderBy(d => d.Date)
                    .Take(5)
                    .ToList();

                return grouped;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetFiveDayForecastAsync error: {ex}");
                return null;
            }
        }

        private static DateTime? ParseDate(string? dtTxt)
        {
            if (string.IsNullOrWhiteSpace(dtTxt)) return null;
            if (DateTime.TryParse(dtTxt, out var dt)) return dt;
            if (DateTime.TryParseExact(dtTxt, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt))
                return dt;
            return null;
        }

        private class OpenWeatherResponse
        {
            public string name { get; set; }
            public WeatherDesc[] weather { get; set; }
            public MainInfo main { get; set; }
            public WindInfo wind { get; set; }
        }

        private class WeatherDesc { public string description { get; set; } }
        private class MainInfo { public double temp { get; set; } public double humidity { get; set; } }
        private class WindInfo { public double speed { get; set; } }

        private class OpenWeatherForecastResponse
        {
            public ForecastItem[] list { get; set; }
        }

        private class ForecastItem
        {
            public MainInfo main { get; set; }
            public string dt_txt { get; set; }
        }
    }
}
