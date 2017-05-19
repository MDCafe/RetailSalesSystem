using RetailManagementSystem.ViewModel.Entitlements;
using System.Windows;

namespace RetailManagementSystem.View.Entitlements
{

    public partial class Login : Window
    {
        LoginViewModel loginViewModel;

        public Login()
        {
            InitializeComponent();
            loginViewModel = new LoginViewModel();
            DataContext = loginViewModel;
        }
    }
}
