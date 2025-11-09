using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherNotesApp.ViewModels;
using WeatherNotesApp.Services;
using WeatherNotesApp.Views;

namespace WeatherNotesApp.Views
{
    public partial class NotesPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly IAlertService _alertService;

        public NotesPage(DatabaseService databaseService, IAlertService alertService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _alertService = alertService;
            BindingContext = new NotesViewModel(databaseService);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is NotesViewModel vm)
            {
                await vm.LoadNotesAsync();
            }
        }

        private async void OnWeatherButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WeatherPage(_databaseService, _alertService));
        }

        private async void OnDeleteNotesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DeleteNotesPage(App.DatabaseService));
        }

        private async void OnUpdateNotesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateNotesPage(App.DatabaseService));
        }
    }
}
