using log4net;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Entitlements
{
    internal class LoginViewModel : ViewModelBase
    {
        protected static readonly ILog _log = LogManager.GetLogger(typeof(LoginViewModel));

        public string UserId { get; set; }
        public int UserInternalId { get; set; }
        public string UserName { get; set; }

        #region OK Command
        public RelayCommand<PasswordBox> okCommand { get; private set; }

        public ICommand OkCommand
        {
            get
            {
                if (okCommand == null)
                {
                    okCommand = new RelayCommand<PasswordBox>((p) => Login(p), (p) =>
                    {
                        var password = p.Password;
                        return !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(password);
                    });
                }

                return okCommand;
            }
        }

        protected virtual void Login(PasswordBox passwordBox)
        {
            try
            {
                var password = passwordBox.Password;
                using (RMSEntities rmsEntities = new RMSEntities())
                {
                    // var count = rmsEntities.Users.Local.ToList().Count;
                    var pwdGrid = passwordBox.Parent as Grid;
                    var window = pwdGrid.Parent as Window;
                    ////Check if the user is admin
                    //if (_validateAsAdmin)
                    //{
                    //    if (ValidateAdminUser(rmsEntities, password))
                    //    {
                    //        window.DialogResult = true;
                    //        window.Close();
                    //        return;
                    //    }                   
                    //}

                    if (ValidateUser(rmsEntities, password))
                    {
                        window.DialogResult = true;
                        window.Close();
                        return;
                    }

                    Utility.ShowMessageBox(window, "Invalid UserId or Password");
                }
            }
            catch (System.Exception ex)
            {
                Utility.ShowErrorBox(ex.Message);
                _log.Error("Error on login..", ex);
            }
        }

        //bool ValidateAdminUser(RMSEntities rmsEntities,string password)
        //{
        //    //return rmsEntities.Users.Any(u => u.username == UserId && u.password == password && u.RoleId == 1);

        //    var user = rmsEntities.Users.FirstOrDefault(u => u.username == UserId && u.password == password && u.RoleId == 1);
        //    if (user == null)
        //        return false;

        //    EntitlementInformation.UserInternalId = user.Id;
        //    return true;
        //}

        protected bool ValidateUser(RMSEntities rmsEntities, string password)
        {
            var user = rmsEntities.Users.FirstOrDefault(u => u.username == UserId && u.password == password);
            if (user == null) return false;
            
            EntitlementInformation.UserInternalId = UserInternalId = user.Id;
            EntitlementInformation.UserName = user.username;
            return true;
        }

        #endregion        
    }
}
