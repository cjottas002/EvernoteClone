using EvernoteClone.Model;
using EvernoteClone.ViewModel.Commands;
using EvernoteClone.ViewModel.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EvernoteClone.ViewModel
{
    public class NotesVm : INotifyPropertyChanged
    {
        private Notebook _selectedNotebook;
        
        public ObservableCollection<Note> Notes { get; set; }
        public ObservableCollection<Notebook> Notebooks { get; set; }
        public Notebook SelectedNotebook
        {
            get => _selectedNotebook;
            set
            {
                _selectedNotebook = value;
                OnPropertyChanged(nameof(SelectedNotebook));
                _ = this.LoadNotesAsync();
            }
        }
        
        private Note _selectedNote;
        public Note SelectedNote
        {
           get => _selectedNote;
           set
           {
               _selectedNote = value;
               OnPropertyChanged(nameof(SelectedNote));
               SelectedNoteChanged?.Invoke(this, EventArgs.Empty);
           }
        }

        private Visibility _isVisible;
        public Visibility IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public NewNotebookCommand NewNotebookCommand { get; set; }
        public NewNoteCommand NewNoteCommand { get; set; }
        public EditCommand EditCommand { get; set; }
        public EndEditingCommand EndEditingCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SelectedNoteChanged;

        public NotesVm()
        {
            this.NewNotebookCommand = new NewNotebookCommand(this);
            this.NewNoteCommand = new NewNoteCommand(this);
            this.EditCommand = new EditCommand(this);
            this.EndEditingCommand = new EndEditingCommand(this);
            
            this.Notebooks = [];
            this.Notes = [];
            
            this.IsVisible = Visibility.Collapsed;

            _ = this.InitializeAsync();
        }
        
        private async Task LoadNotesAsync()
        {
            await GetNotes();
        }
        
        private async Task InitializeAsync()
        {
            await GetNotebooks();
        }

        public async Task CreateNotebook()
        {
            var notebook = new Notebook()
            {
                Name = "Notebook",
                UserId = App.UserId
            };

            await DatabaseHelper.Insert(notebook);
            await this.GetNotebooks();
        }

        public async Task CreateNote(string notebookId)
        {
            var newNote = new Note()
            {
                NotebookId = notebookId,
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Title = $"Note for {DateTime.Now}"
            };

            await DatabaseHelper.Insert(newNote);
            await this.GetNotes();
        }

        private async Task GetNotebooks()
        {
            var notebooks = (await DatabaseHelper
                    .Read<Notebook>())
                    .Where(n => n.UserId == App.UserId)
                    .ToList();

            Notebooks.Clear();
            foreach (var notebook in notebooks)
            {
                Notebooks.Add(notebook);
            }
        }

        private async Task GetNotes()
        {
            if (this.SelectedNotebook is null) return;
            var notes = (await DatabaseHelper
                .Read<Note>())
                .Where(n => n.NotebookId == this.SelectedNotebook.Id)
                .ToList();

            this.Notes.Clear();
            foreach (var note in notes)
            {
                this.Notes.Add(note);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StartEditing()
        {
            IsVisible = Visibility.Visible;
        }
        
        public async Task StopEditing(Notebook notebook)
        {
            IsVisible = Visibility.Collapsed;
            await DatabaseHelper.Update(notebook);
            await this.GetNotebooks();
        }
    }
}
