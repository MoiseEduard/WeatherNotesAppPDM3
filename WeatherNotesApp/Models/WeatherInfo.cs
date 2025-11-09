using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherNotesApp.Models
{
    public class WeatherInfo
    {
        public string CityName { get; set; }
        public double Temperature { get; set; }
        public string Description { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
    }
}
