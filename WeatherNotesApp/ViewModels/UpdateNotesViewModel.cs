using System.Collections.ObjectModel;
using System.Windows.Input;
using WeatherNotesApp.Models;
using WeatherNotesApp.Services;

namespace WeatherNotesApp.ViewModels
{
    public class UpdateNotesViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Note> EditableNotes { get; } = new();

        public ICommand UpdateNoteCommand { get; }

        public UpdateNotesViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            UpdateNoteCommand = new Command<Note>(async (note) => await UpdateNoteAsync(note));
            _ = LoadEditableNotesAsync();
        }

        private async Task LoadEditableNotesAsync()
        {
            var allNotes = await _databaseService.GetNotesAsync();
            var manualNotes = allNotes.Where(n => n.WeatherCondition == "-").ToList();
            EditableNotes.Clear();
            foreach (var note in manualNotes)
                EditableNotes.Add(note);
        }

        private async Task UpdateNoteAsync(Note note)
        {
            if (note == null) return;
            await _databaseService.SaveNoteAsync(note);
            await Application.Current.MainPage.DisplayAlert("Succes", "Notița a fost actualizată.", "OK");
        }
    }
}
