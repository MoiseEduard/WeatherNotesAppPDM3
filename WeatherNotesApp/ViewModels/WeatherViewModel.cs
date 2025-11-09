using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using WeatherNotesApp.Models;
using WeatherNotesApp.Services;

namespace WeatherNotesApp.ViewModels
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        private readonly WeatherService _weatherService = new();
        private readonly DatabaseService _databaseService;
        private readonly IAlertService _alertService;

        private string _city;
        private WeatherInfo _weather;
        private bool _isBusy;
        public bool HasWeather => Weather != null;

        private List<ForecastDay> _forecast;
        public List<ForecastDay> Forecast
        {
            get => _forecast;
            private set { _forecast = value; OnPropertyChanged(); }
        }

        // Modelul pentru afisarea oraselor cautate in UI
        public record CitySearchEntry(string City, DateTime Date)
        {
            public string DisplayText => $"{City} - {Date:dd MMM yyyy}";
        }

        private readonly ObservableCollection<CitySearchEntry> _searchedCities = new();
        public ObservableCollection<CitySearchEntry> SearchedCities => _searchedCities;

        public string City
        {
            get => _city;
            set { _city = value; OnPropertyChanged(); }
        }

        public WeatherInfo Weather
        {
            get => _weather;
            set
            {
                _weather = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasWeather));

                if (_weather != null && !string.IsNullOrWhiteSpace(_weather.CityName))
                {
                    var cityName = _weather.CityName.Trim();
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (!SearchedCities.Any(c => string.Equals(c.City, cityName, StringComparison.OrdinalIgnoreCase)))
                        {
                            System.Diagnostics.Debug.WriteLine($"Adding history entry: {cityName}");
                            var entry = new CitySearchEntry(cityName, DateTime.Now);
                            SearchedCities.Add(entry);

                            // ✅ Salvare in baza de date
                            var cities = await _databaseService.GetCitiesAsync();
                            if (!cities.Any(c => c.Name.Equals(cityName, StringComparison.OrdinalIgnoreCase)))
                            {
                                var city = new City { Name = cityName, IsFavorite = false };
                                await _databaseService.SaveCityAsync(city);
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"History already contains: {cityName}");
                        }
                    });
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand GetWeatherCommand { get; }
        public ICommand SaveCityAsNoteCommand { get; }

        private Chart _temperatureChart;
        public Chart TemperatureChart
        {
            get => _temperatureChart;
            set { _temperatureChart = value; OnPropertyChanged(); }
        }

        private void UpdateChart(List<ForecastDay> forecast)
        {
            Forecast = forecast;

            if (forecast == null || !forecast.Any())
            {
                TemperatureChart = null;
                return;
            }

            var entries = forecast.Select(d =>
                new ChartEntry((float)d.Temp)
                {
                    Label = d.Date.ToString("dd MMM"),
                    ValueLabel = $"{d.Temp}°C",
                    Color = SKColor.Parse("#2196F3")
                }).ToList();

            TemperatureChart = new BarChart { Entries = entries };
        }

        public WeatherViewModel(DatabaseService databaseService, IAlertService alertService)
        {
            _databaseService = databaseService;
            _alertService = alertService;
            GetWeatherCommand = new Command(async () => await GetWeatherAsync());
            SaveCityAsNoteCommand = new Command(async () => await SaveCurrentWeatherAsNoteAsync());

            _ = LoadCityHistoryAsync();
        }

        private async Task GetWeatherAsync()
        {
            if (string.IsNullOrWhiteSpace(City)) return;

            IsBusy = true;
            try
            {
                Weather = await _weatherService.GetWeatherAsync(City);

                var forecast = await _weatherService.GetFiveDayForecastAsync(City);
                if (forecast != null)
                {
                    UpdateChart(forecast);
                }
                else
                {
                    Forecast = null;
                    TemperatureChart = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetWeatherAsync error: {ex}");
                try
                {
                    await _alertService.ShowAlert("Eroare", ex.Message);
                }
                catch
                {
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadCityHistoryAsync()
        {
            try
            {
                var cities = await _databaseService.GetCitiesAsync();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SearchedCities.Clear();
                    foreach (var city in cities)
                    {
                        SearchedCities.Add(new CitySearchEntry(city.Name, DateTime.Now));
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadCityHistoryAsync error: {ex}");
            }
        }

        public async Task<bool> EnsureForecastForWeatherAsync()
        {
            if (Weather == null || string.IsNullOrWhiteSpace(Weather.CityName)) return false;

            if (Forecast != null && Forecast.Any()) return true;

            IsBusy = true;
            try
            {
                var forecast = await _weatherService.GetFiveDayForecastAsync(Weather.CityName);
                if (forecast != null && forecast.Any())
                {
                    UpdateChart(forecast);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EnsureForecastForWeatherAsync error: {ex}");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveCurrentWeatherAsNoteAsync()
        {
            if (Weather == null) return;

            var note = new Note
            {
                Date = DateTime.Now,
                Text = $"{Weather.CityName} - {Weather.Temperature:F1}°C - {Weather.Description}",
                WeatherCondition = Weather.Description,
                CityName = Weather.CityName,
                Temperature = Weather.Temperature
            };

            await _databaseService.SaveNoteAsync(note);
            await _alertService.ShowAlert("Saved", "Notita creata, oras salvat in istoricul cautarilor.");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
