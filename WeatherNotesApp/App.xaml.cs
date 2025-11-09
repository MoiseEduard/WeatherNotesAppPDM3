using WeatherNotesApp.Views;
using WeatherNotesApp.Services;

namespace WeatherNotesApp
{
    public partial class App : Application
    {
        public static DatabaseService DatabaseService { get; private set; }
        public static IAlertService AlertService { get; private set; }

        public App(DatabaseService databaseService, IAlertService alertService)
        {
            InitializeComponent();

            DatabaseService = databaseService;
            AlertService = alertService;

            MainPage = new NavigationPage(new NotesPage(databaseService, alertService));
        }
    }
}
