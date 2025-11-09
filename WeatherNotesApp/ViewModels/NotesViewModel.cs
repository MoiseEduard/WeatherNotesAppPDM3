using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WeatherNotesApp.Models;
using WeatherNotesApp.Services;
using Microsoft.Maui.Controls;

namespace WeatherNotesApp.ViewModels
{
    public class NotesViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;

        private const int NotesPerPage = 5;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private List<Note> _allNotes = new();

        public ObservableCollection<Note> Notes { get; } = new();

        private string _noteText;
        public string NoteText
        {
            get => _noteText;
            set
            {
                _noteText = value;
                OnPropertyChanged();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PageInfo));
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PageInfo));
            }
        }

        public string PageInfo => $"Pagina {CurrentPage} / {TotalPages}";

        public ICommand AddNoteCommand { get; }
        public ICommand DeleteNoteCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public NotesViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            AddNoteCommand = new Command(async () => await AddNote());
            DeleteNoteCommand = new Command<Note>(async (note) => await DeleteNote(note));
            NextPageCommand = new Command(async () => await NextPage());
            PreviousPageCommand = new Command(async () => await PreviousPage());

            _ = LoadNotesAsync();
        }

        public async Task LoadNotesAsync()
        {
            _allNotes = await _databaseService.GetNotesAsync();

            if (_allNotes.Count == 0)
            {
                Notes.Clear();
                TotalPages = 1;
                CurrentPage = 1;
                return;
            }

            TotalPages = (int)Math.Ceiling((double)_allNotes.Count / NotesPerPage);
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;

            await LoadCurrentPageAsync();
        }

        private async Task LoadCurrentPageAsync()
        {
            Notes.Clear();

            var skip = (CurrentPage - 1) * NotesPerPage;
            var pageNotes = _allNotes
                .Skip(skip)
                .Take(NotesPerPage)
                .ToList();

            foreach (var note in pageNotes)
                Notes.Add(note);

            OnPropertyChanged(nameof(PageInfo));
            await Task.CompletedTask;
        }

        private async Task AddNote()
        {
            if (string.IsNullOrWhiteSpace(NoteText))
                return;

            var note = new Note
            {
                Text = NoteText,
                Date = DateTime.Now,
                WeatherCondition = "-"
            };

            await _databaseService.SaveNoteAsync(note);
            NoteText = string.Empty;

            await LoadNotesAsync();
        }

        private async Task DeleteNote(Note note)
        {
            await _databaseService.DeleteNoteAsync(note);
            await LoadNotesAsync();
        }

        private async Task NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadCurrentPageAsync();
            }
        }

        private async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadCurrentPageAsync();
            }
        }
    }
}
