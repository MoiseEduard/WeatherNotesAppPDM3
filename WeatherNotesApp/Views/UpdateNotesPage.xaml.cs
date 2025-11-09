using WeatherNotesApp.Services;
using WeatherNotesApp.ViewModels;

namespace WeatherNotesApp.Views;

public partial class UpdateNotesPage : ContentPage
{
    public UpdateNotesPage(DatabaseService databaseService)
    {
        InitializeComponent();
        BindingContext = new UpdateNotesViewModel(databaseService);
    }
}
