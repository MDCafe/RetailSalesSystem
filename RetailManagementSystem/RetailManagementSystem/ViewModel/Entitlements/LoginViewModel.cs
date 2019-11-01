using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Entitlements
{
    internal class LoginViewModel : ViewModelBase
    {        
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

        void Login(PasswordBox passwordBox)
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

        //bool ValidateAdminUser(RMSEntities rmsEntities,string password)
        //{
        //    //return rmsEntities.Users.Any(u => u.username == UserId && u.password == password && u.RoleId == 1);

        //    var user = rmsEntities.Users.FirstOrDefault(u => u.username == UserId && u.password == password && u.RoleId == 1);
        //    if (user == null)
        //        return false;

        //    EntitlementInformation.UserInternalId = user.Id;
        //    return true;
        //}

        bool ValidateUser(RMSEntities rmsEntities, string password)
        {
            var user = rmsEntities.Users.FirstOrDefault(u => u.username == UserId && u.password == password);
            if (user == null) return false;

            UserInternalId = user.Id;
            EntitlementInformation.UserName = user.username;
            return true;
        }

        #endregion

        #region CloseWindow Command
        public RelayCommand<Window> closeWindowCommand { get; private set; }

        public ICommand CloseWindowCommand
        {
            get
            {
                if (closeWindowCommand == null)
                {
                    closeWindowCommand = new RelayCommand<Window>((window) =>
                    {
                        if (window != null)
                        {                            
                            window.Close();
                        }
                    });
                }

                return closeWindowCommand;
            }
        }       
        #endregion
    }
}
