using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace RetailManagementSystem.ViewModel.Entitlements
{
    class LoginViewModel : ViewModelBase
    {
        public string UserId { get; set; }

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
            //var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;

            if(RMSEntitiesHelper.RMSEntities.Users.Local.Any(u => u.username == UserId && u.password == password))
            {
                return true;
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
