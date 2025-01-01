using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EvernoteClone.Model;

namespace EvernoteClone.ViewModel.Commands
{
    public class LoginCommand(LoginVm vm) : ICommand
    {
        public LoginVm ViewModel { get; set; } = vm;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (parameter is not User user)
                return false;
            
            if(string.IsNullOrEmpty(user.UserName))
                return false;
            
            return !string.IsNullOrEmpty(user.Password);
        }

        public void Execute(object parameter)
        {
            this.ViewModel.Login();
        }
    }
}
