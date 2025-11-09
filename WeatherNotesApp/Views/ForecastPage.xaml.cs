using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherNotesApp.ViewModels;

namespace WeatherNotesApp.Views
{
 public partial class ForecastPage : ContentPage
 {
 public ForecastPage(WeatherViewModel vm)
 {
 InitializeComponent();
 BindingContext = vm;
 }
 }
}
