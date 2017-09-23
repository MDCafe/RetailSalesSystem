using log4net;
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

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if(e.Exception.GetType() == typeof(MySql.Data.MySqlClient.MySqlException))
            {
                //Mysql Exception
                Utilities.Utility.ShowErrorBox("Database Exception" + e.Exception.ToString());
            }

            MessageBox.Show(e.Exception.ToString());
            _log.Debug("Application Error", e.Exception);
        }
    }
}
