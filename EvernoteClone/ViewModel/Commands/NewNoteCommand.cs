using EvernoteClone.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commands
{
    public class NewNoteCommand(NotesVm vm) : ICommand
    {
        private NotesVm ViewModel { get; set; } = vm;
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            var selectedNotebook = parameter as Notebook;
            return selectedNotebook != null;
        }

        public async void Execute(object parameter)
        {
            var selectedNotebook = parameter as Notebook;
            await ViewModel.CreateNote(selectedNotebook?.Id);
        }
    }
}
