using System;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commands
{
    public class NewNotebookCommand(NotesVm vm) : ICommand
    {
        private NotesVm ViewModel { get; set; } = vm;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;    
        }

        public async void Execute(object parameter)
        {
            await ViewModel.CreateNotebook();
        }
    }
}
