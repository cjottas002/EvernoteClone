using System;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commands;

public class EditCommand(NotesVM vm) : ICommand
{
    public event EventHandler CanExecuteChanged;
    private NotesVM ViewModel { get; set; } = vm;

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        ViewModel.StartEditing();
    }

}