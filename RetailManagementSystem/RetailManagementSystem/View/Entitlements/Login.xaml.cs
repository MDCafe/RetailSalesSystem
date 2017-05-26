using RetailManagementSystem.ViewModel.Entitlements;
using System.Windows;

namespace RetailManagementSystem.View.Entitlements
{

    public partial class Login : Window
    {
        LoginViewModel loginViewModel;

        public Login()
        {
            Initalize(false);
        }

        private void Initalize(bool validateAsAdmin)
        {
            InitializeComponent();
            loginViewModel = new LoginViewModel(validateAsAdmin);
            DataContext = loginViewModel;
        }

        public Login(bool validateAsAdmin)
        {
            Initalize(validateAsAdmin);
        }
    }
}
