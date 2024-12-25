using EvernoteClone.Model;
using EvernoteClone.ViewModel.Commands;
using EvernoteClone.ViewModel.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
                this.GetNotes();
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
            
            Notebooks = new ObservableCollection<Notebook>();
            Notes = new ObservableCollection<Note>();
            
            IsVisible = Visibility.Collapsed;

            this.GetNotebooks();
        }

        public void CreateNotebook()
        {
            var notebook = new Notebook()
            {
                Name = "Notebook"
            };

            DatabaseHelper.Insert(notebook);

            this.GetNotebooks();
        }

        public void CreateNote(int notebookId)
        {
            var newNote = new Note()
            {
                NotebookId = notebookId,
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Title = $"Note for {DateTime.Now}"
            };

            DatabaseHelper.Insert(newNote);

            this.GetNotes();
        }

        private void GetNotebooks()
        {
            var notebooks = DatabaseHelper.Read<Notebook>();

            Notebooks.Clear();
            foreach (var notebook in notebooks)
            {
                Notebooks.Add(notebook);
            }
        }

        private void GetNotes()
        {
            if (this.SelectedNotebook is not null)
            {

                var notes = DatabaseHelper
                    .Read<Note>()
                    .Where(n => n.NotebookId == this.SelectedNotebook.Id)
                    .ToList();

                this.Notes.Clear();
                foreach (var note in notes)
                {
                    this.Notes.Add(note);
                }
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
        
        public void StopEditing(Notebook notebook)
        {
            IsVisible = Visibility.Collapsed;
            DatabaseHelper.Update(notebook);
            GetNotebooks();
        }
    }
}
