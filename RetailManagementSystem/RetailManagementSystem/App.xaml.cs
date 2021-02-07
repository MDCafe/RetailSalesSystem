using log4net;
using RetailManagementSystem.View.Entitlements;
using RetailManagementSystem.View.Sales;
using System;
using System.Windows;
using System.Linq;

namespace RetailManagementSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(App));
        Window mainWindow;

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void ApplicationStart(object sender, StartupEventArgs e)
        {                        
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Start(() =>
            {
                using (RMSEntities context = new RMSEntities())
                {
                    context.Users.FirstOrDefault();
                }
            });


#if DebugPOS
            var poslogin = new POSLogin();
                var posResult = poslogin.ShowDialog();
                if (!posResult.Value)
                {
                    Shutdown();
                    return;
                }

            var posSalesEntry = new POSSalesEntry();
            posSalesEntry.ShowDialog();
            this.Shutdown();
            return;
#endif


            var login = new Login();
            var result = login.ShowDialog();
            if (!result.Value)
            {
                Shutdown();
                return;
            }

            var isAdmin = RMSEntitiesHelper.Instance.IsAdmin(login.txtUserId.Text.Trim());
            ViewModel.Entitlements.EntitlementInformation.UserInternalId = login.LoginVM.UserInternalId;

            mainWindow = new MainWindow(isAdmin)
            {
                ShowActivated = true
            };
            mainWindow.ShowDialog();
            this.Shutdown();
        }

        private void Start(Action a)
        {
            a.BeginInvoke(null, null);
        }
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception.GetType() == typeof(MySql.Data.MySqlClient.MySqlException))
            {
                //Mysql Exception
                Utilities.Utility.ShowErrorBox(mainWindow, "Database Exception" + e.Exception.ToString());
            }

            Utilities.Utility.ShowErrorBox(mainWindow, e.Exception.ToString());
            _log.Debug("Application Error", e.Exception);
            e.Handled = true;
        }
    }
}
