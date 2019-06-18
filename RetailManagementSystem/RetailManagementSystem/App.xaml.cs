using log4net;
using RetailManagementSystem.View.Entitlements;
using System.Windows;
 
namespace RetailManagementSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(App));

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var login = new Login();
            var result = login.ShowDialog();
            if (!result.Value)
            {
                Shutdown();
                return;
            }

            var isAdmin = RMSEntitiesHelper.Instance.IsAdmin(login.txtUserId.Text.Trim());
            ViewModel.Entitlements.EntitlementInformation.UserInternalId = login.LoginVM.UserInternalId;

            var mainWindow = new MainWindow(isAdmin)
            {
                ShowActivated = true
            };
            mainWindow.ShowDialog();
            this.Shutdown();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if(e.Exception.GetType() == typeof(MySql.Data.MySqlClient.MySqlException))
            {
                //Mysql Exception
                Utilities.Utility.ShowErrorBox("Database Exception" + e.Exception.ToString());
            }

            Utilities.Utility.ShowErrorBox(e.Exception.ToString());
            _log.Debug("Application Error", e.Exception);
            e.Handled = true;
        }
    }
}
