using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace WeatherNotesApp.Models
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public string WeatherCondition { get; set; }

        public string CityName { get; set; }
        public double Temperature { get; set; }
    }
}
