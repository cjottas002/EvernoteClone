using System;
using System.Windows;
using EvernoteClone.ViewModel;

namespace EvernoteClone.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly LoginVm viewModel;
        public LoginWindow()
        {
            InitializeComponent();
            
            viewModel = Resources["vm"] as LoginVm;
            if (viewModel != null) 
                viewModel.Authenticated += ViewModel_Authenticated;
        }

        private void ViewModel_Authenticated(object sender, EventArgs e)
        {
            Close();
        }
    }
}
