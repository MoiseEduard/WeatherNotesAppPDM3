namespace WeatherNotesApp.Views
{
    public partial class ViewPhotoPage : ContentPage
    {
        public ViewPhotoPage(string photoPath)
        {
            InitializeComponent();

            FullScreenImage.Source = ImageSource.FromFile(photoPath);
        }
    }
}