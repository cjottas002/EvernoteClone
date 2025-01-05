using System;
using System.Windows.Input;
using EvernoteClone.Model;

namespace EvernoteClone.ViewModel.Commands;

public class EndEditingCommand(NotesVm vm) : ICommand
{
    public event EventHandler CanExecuteChanged;
    private NotesVm ViewModel { get; set; } = vm;

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public async void Execute(object parameter)
    {
        if (parameter is Notebook notebook)
            await ViewModel.StopEditing(notebook);
    }

}