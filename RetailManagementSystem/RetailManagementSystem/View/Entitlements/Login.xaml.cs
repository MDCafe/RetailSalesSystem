using RetailManagementSystem.ViewModel.Entitlements;
using System.Windows;

namespace RetailManagementSystem.View.Entitlements
{
    public partial class Login : Window
    {
        internal LoginViewModel LoginVM { get; private set; }

        public Login()
        {
            InitializeComponent();
            LoginVM = new LoginViewModel();
            DataContext = LoginVM;
        }

        private void Initalize(bool validateAsAdmin, bool InitialLogin)
        {
            
            //loginViewModel = new LoginViewModel(validateAsAdmin, InitialLogin);
            
        }

        //public Login(bool validateAsAdmin,bool InitialLogin)
        //{
        //    Initalize(validateAsAdmin, InitialLogin);
        //}
    }
}
