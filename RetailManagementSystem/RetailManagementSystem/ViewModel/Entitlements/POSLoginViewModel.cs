﻿using RetailManagementSystem.Utilities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RetailManagementSystem.ViewModel.Entitlements
{
    class POSLoginViewModel : LoginViewModel
    {
        public POSLoginViewModel()
        {
            try
            {
                using (var en = new RMSEntities())
                {
                    ApplicationName = en.ApplicationDetails.FirstOrDefault().Name;
                }
            }
            catch (System.Exception ex)
            {
                Utility.ShowErrorBox(ex.Message);
            }
        }

        public decimal? PettyCash { get; set; }
        public string ApplicationName { get; private set; }

        protected override void Login(PasswordBox passwordBox)
        {
            try
            {
                var password = passwordBox.Password;
                using (RMSEntities rmsEntities = new RMSEntities())
                {
                    var pwdstackPanel = passwordBox.Parent as StackPanel;
                    var gridLogin = pwdstackPanel.Parent as Grid;
                    var window = gridLogin.Parent as Window;


                    if (ValidateUser(rmsEntities, password))
                    {
                        //update the ShiftDetails 
                        var serverDate = RMSEntitiesHelper.GetServerDate();

                        rmsEntities.ShiftDetails.Add(new ShiftDetail()
                        {
                            UserId = this.UserInternalId,
                            LoginDate = serverDate,
                            PettyCash = PettyCash
                        });

                        rmsEntities.SaveChanges();

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
    }
}
