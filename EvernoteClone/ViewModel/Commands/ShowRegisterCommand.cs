using System;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commands
{
    public class ShowRegisterCommand(LoginVM vm) : ICommand
    {
        public LoginVM ViewModel { get; set; } = vm;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ViewModel.SwitchViews();
        }
    }
}
