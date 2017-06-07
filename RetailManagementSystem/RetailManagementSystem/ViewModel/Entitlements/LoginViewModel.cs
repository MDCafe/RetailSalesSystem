using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Entitlements
{
    class LoginViewModel : ViewModelBase
    {
        bool _validateAsAdmin;

        public string UserId { get; set; }

        public LoginViewModel(bool validateAsAdmin)
        {
            _validateAsAdmin = validateAsAdmin;
        }

        #region OK Command
        public RelayCommand<PasswordBox> _okCommand { get; private set; }

        public ICommand OkCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand<PasswordBox>((p) => Login(p), (p) => CanLogin(p));
                }

                return _okCommand;
            }
        }

        private bool CanLogin(PasswordBox p)
        {
            var password = p.Password;
            return !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(password);
        }

        void Login(PasswordBox passwordBox)
        {            
            var password = passwordBox.Password;
            var count = RMSEntitiesHelper.Instance.RMSEntities.Users.Local.ToList().Count;           

            //RMSEntitiesHelper.RMSEntities.Configuration.LazyLoadingEnabled = false;

            //Check if the user is admin
            if (_validateAsAdmin)
            {
                var pwdGrid = passwordBox.Parent as Grid;
                var window = pwdGrid.Parent as Window;
                if (RMSEntitiesHelper.Instance.RMSEntities.Users.Any(u => u.username == UserId && u.password == password && u.RoleId == 1))
                {                    
                    window.DialogResult = true;
                    window.Close();
                    return;
                }

                Utility.ShowMessageBox(window, "Invalid UserId or Password");
                //window.DialogResult = false;                
            }            
        }

        #endregion

        #region CloseWindow Command
        public RelayCommand<Window> _closeWindowCommand { get; private set; }

        public ICommand CloseWindowCommand
        {
            get
            {
                if (_closeWindowCommand == null)
                {
                    _closeWindowCommand = new RelayCommand<Window>((w) => CloseWindow(w));
                }

                return _closeWindowCommand;
            }
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
        #endregion
    }
}
