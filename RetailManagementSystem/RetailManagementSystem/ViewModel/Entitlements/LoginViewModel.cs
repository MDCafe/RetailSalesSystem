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
            using (RMSEntities rmsEntities = new RMSEntities())
            {
                var count = rmsEntities.Users.Local.ToList().Count;
                var pwdGrid = passwordBox.Parent as Grid;
                var window = pwdGrid.Parent as Window;
                //Check if the user is admin
                if (_validateAsAdmin)
                {
                    if (ValidateAdminUser(rmsEntities, password))
                    {
                        window.DialogResult = true;
                        window.Close();
                        return;
                    }                   
                }

                if (ValidateUser(rmsEntities, password))
                {
                    window.DialogResult = true;
                    window.Close();
                    return;
                }

                Utility.ShowMessageBox(window, "Invalid UserId or Password");
            }
        }

        bool ValidateAdminUser(RMSEntities rmsEntities,string password)
        {
            return rmsEntities.Users.Any(u => u.username == UserId && u.password == password && u.RoleId == 1);
        }

        bool ValidateUser(RMSEntities rmsEntities, string password)
        {
            return rmsEntities.Users.Any(u => u.username == UserId && u.password == password);
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
