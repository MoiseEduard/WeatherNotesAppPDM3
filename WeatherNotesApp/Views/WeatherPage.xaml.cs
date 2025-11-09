using System;
using System.Linq;
using WeatherNotesApp.ViewModels;
using WeatherNotesApp.Services;
using Microsoft.Maui.Controls;

namespace WeatherNotesApp.Views
{
    public partial class WeatherPage : ContentPage
    {
        public WeatherPage(DatabaseService databaseService, IAlertService alertService)
        {
            InitializeComponent();
            BindingContext = new WeatherViewModel(databaseService, alertService);
        }

        private async void OnShowForecastClicked(object sender, EventArgs e)
        {
            if (BindingContext is WeatherViewModel vm)
            {
                var ok = await vm.EnsureForecastForWeatherAsync();
                if (ok)
                {
                    await Navigation.PushAsync(new ForecastPage(vm));
                    return;
                }
            }

            await DisplayAlert("Info", "Nu exista prognoza. Cauta mai intai o locatie.", "OK");
        }

        private void OnCityPicked(object sender, EventArgs e)
        {
            if (BindingContext is WeatherViewModel vm)
            {
                var picker = sender as Picker;
                var selectedEntry = picker?.SelectedItem as WeatherViewModel.CitySearchEntry;
                if (selectedEntry == null)
                    return;

                vm.City = selectedEntry.City;
                if (vm.GetWeatherCommand?.CanExecute(null) == true)
                    vm.GetWeatherCommand.Execute(null);

                if (picker != null)
                    picker.SelectedItem = null;
            }
        }
    }
}
