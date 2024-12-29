using EvernoteClone.Model;
using EvernoteClone.ViewModel.Commands;
using System.ComponentModel;
using System.Windows;

namespace EvernoteClone.ViewModel
{
    public class LoginVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isShowingRegister = false;

		private User user;
		public User User
		{
			get => user;
            set => user = value;
        }

        private Visibility loginVis;
        public Visibility LoginVis
        {
            get { return this.loginVis; }
            set 
            { 
                this.loginVis = value; 
                OnPropertyChanged(nameof(LoginVis));
            }
        }


        public Visibility registerVis;
        public Visibility RegisterVis
        {
            get => this.registerVis;
            set 
            { 
                this.registerVis = value; 
                OnPropertyChanged(nameof(RegisterVis));
            }
        }


        public RegisterCommand RegisterCommand { get; set; }
        public LogingCommand LogingCommand { get; set; }
        public ShowRegisterCommand ShowRegisterCommand { get; set; }

        public LoginVM()
        {
            this.LoginVis = Visibility.Visible;
            this.RegisterVis = Visibility.Collapsed;

            this.RegisterCommand = new RegisterCommand(this);
            this.LogingCommand = new LogingCommand(this);
            this.ShowRegisterCommand = new ShowRegisterCommand(this);
        }

        public void SwitchViews()
        {
            this.isShowingRegister = !this.isShowingRegister;

            if (this.isShowingRegister)
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

        private void OnPropertyChanged(string propertyName) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
