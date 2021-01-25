using RetailManagementSystem.ViewModel.Entitlements;
using System.Windows;

namespace RetailManagementSystem.View.Entitlements
{
    public partial class POSLogin : Window
    {
        //internal POSLoginViewModel LoginVM { get; private set; }

        public POSLogin()
        {
            InitializeComponent();            
            DataContext = new POSLoginViewModel();
        }        
    }
}
