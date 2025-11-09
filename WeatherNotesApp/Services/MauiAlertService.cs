using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Controls;

namespace WeatherNotesApp.Services
{
 public class MauiAlertService : IAlertService
 {
 public async Task ShowAlert(string title, string message, string cancel = "OK")
 {

 await MainThread.InvokeOnMainThreadAsync(async () =>
 {
 var main = Application.Current?.MainPage;
 Page page = null;
 if (main is NavigationPage nav)
 {
 page = nav.CurrentPage ?? nav;
 }
 else if (main is FlyoutPage flyout)
 {
 page = flyout.Detail as Page ?? flyout;
 }
 else
 {
 page = main;
 }

 if (page != null)
 {
 await page.DisplayAlert(title, message, cancel);
 }
 });
 }
 }
}
