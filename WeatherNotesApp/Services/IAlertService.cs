using System.Threading.Tasks;

namespace WeatherNotesApp.Services
{
 public interface IAlertService
 {
 Task ShowAlert(string title, string message, string cancel = "OK");
 }
}
