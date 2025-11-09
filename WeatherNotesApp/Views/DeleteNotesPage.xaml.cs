using WeatherNotesApp.Services;
using WeatherNotesApp.ViewModels;

namespace WeatherNotesApp.Views;

public partial class DeleteNotesPage : ContentPage
{
    public DeleteNotesPage(DatabaseService databaseService)
    {
        InitializeComponent();
        BindingContext = new NotesViewModel(databaseService);
    }
}
