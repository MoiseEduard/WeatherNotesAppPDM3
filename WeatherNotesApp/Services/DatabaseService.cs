using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using WeatherNotesApp.Models;

namespace WeatherNotesApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Note>().Wait();
            _database.CreateTableAsync<City>().Wait();
        }
        public Task<List<Note>> GetNotesAsync()
        {
            return _database.Table<Note>().OrderByDescending(n => n.Date).ToListAsync();
        }

        public Task<int> SaveNoteAsync(Note note)
        {
            if (note.Id != 0)
                return _database.UpdateAsync(note);
            else
                return _database.InsertAsync(note);
        }

        public Task<int> DeleteNoteAsync(Note note)
        {
            return _database.DeleteAsync(note);
        }

        public Task<List<City>> GetCitiesAsync()
        {
            return _database.Table<City>().OrderBy(c => c.Name).ToListAsync();
        }

        public Task<int> SaveCityAsync(City city)
        {
            if (city.Id != 0)
                return _database.UpdateAsync(city);
            else
                return _database.InsertAsync(city);
        }

        public Task<int> DeleteCityAsync(City city)
        {
            return _database.DeleteAsync(city);
        }
    }
}
