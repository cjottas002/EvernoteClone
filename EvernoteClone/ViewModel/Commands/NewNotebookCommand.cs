using EvernoteClone.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commands
{
    public class NewNotebookCommand(NotesVm vm) : ICommand
    {
        public NotesVm VM { get; set; } = vm;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;    
        }

        public void Execute(object parameter)
        {
            VM.CreateNotebook();    
        }
    }
}
