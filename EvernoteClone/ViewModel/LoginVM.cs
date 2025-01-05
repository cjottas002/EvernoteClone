using System;
using EvernoteClone.Model;
using EvernoteClone.ViewModel.Commands;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using EvernoteClone.ViewModel.Helpers;

namespace EvernoteClone.ViewModel
{
    public class LoginVm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler Authenticated;

        private bool _isShowingRegister = false;

		private User _user;
		public User User
		{
			get => _user;
            set
            {
             _user = value;
             OnPropertyChanged(nameof(User));
            } 
        }

        private Visibility _loginVis;
        public Visibility LoginVis
        {
            get => this._loginVis;
            set 
            { 
                _loginVis = value; 
                OnPropertyChanged(nameof(LoginVis));
            }
        }

        private Visibility _registerVis;
        public Visibility RegisterVis
        {
            get => this._registerVis;
            set 
            { 
                _registerVis = value; 
                OnPropertyChanged(nameof(RegisterVis));
            }
        }
        
        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                this.User = new User()
                {
                    UserName = _userName,
                    Password = this.Password,
                    Name = this.Name,
                    LastName = this.LastName,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged(nameof(UserName));
            }
        }
        
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                this.User = new User()
                {
                    UserName = this.UserName,
                    Password = this.Password,
                    Name = _name,
                    LastName = this.LastName,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged(nameof(Name));
            }
        }           
        
        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                this.User = new User()
                {
                    UserName = this.UserName,
                    Password = this.Password,
                    Name = this.Name,
                    LastName = _lastName,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged(nameof(LastName));
            }
        }       
        
       private string _password;
       public string Password
       {
           get => _password;
           set
           {
               _password = value;
               this.User = new User()
               {
                    UserName    = this.UserName,
                    Password    = _password,
                    Name = this.Name,
                    LastName = this.LastName,
                    ConfirmPassword = this.ConfirmPassword
               };
               OnPropertyChanged(nameof(Password));
           }
       }       
       
       private string _confirmPassword;
       public string ConfirmPassword 
       {
           get => _confirmPassword;
           set
           {
               _confirmPassword = value;
               this.User = new User()
               {
                    UserName    = this.UserName,
                    Password    = this.Password,
                    Name = this.Name,
                    LastName = this.LastName,
                    ConfirmPassword = _confirmPassword
               };
               OnPropertyChanged(nameof(ConfirmPassword));
           }
       }


        public RegisterCommand RegisterCommand { get; set; }
        public LoginCommand LoginCommand { get; set; }
        public ShowRegisterCommand ShowRegisterCommand { get; set; }

        public LoginVm()
        {
            this.LoginVis = Visibility.Visible;
            this.RegisterVis = Visibility.Collapsed;

            this.RegisterCommand = new RegisterCommand(this);
            this.LoginCommand = new LoginCommand(this);
            this.ShowRegisterCommand = new ShowRegisterCommand(this);
            
            this.User = new User();
        }

        public void SwitchViews()
        {
            this._isShowingRegister = !this._isShowingRegister;

            if (this._isShowingRegister)
            {
                this.RegisterVis = Visibility.Visible;
                this.LoginVis = Visibility.Collapsed;
            }
            else
            {
                this.RegisterVis = Visibility.Collapsed;
                this.LoginVis = Visibility.Visible;
            }
        }

        public async Task LoginAsync()
        {
            var result = await FirebaseAuthHelper.LoginAsync(this.User);
            
            if(!result) return;
            this.Authenticated?.Invoke(this, EventArgs.Empty);
            
        }

        public async Task Register()
        {
            var result = await FirebaseAuthHelper.RegisterAsync(this.User);
            
            if(!result) return;
            this.Authenticated?.Invoke(this, EventArgs.Empty);
        }

        private void OnPropertyChanged(string propertyName) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
