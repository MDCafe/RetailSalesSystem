using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel
{
    internal class ReportViewModel : ReportBaseViewModel
    {
        private ReportViewer _rptViewer;
        protected ReportDataSource[] _rptDataSource;
        protected string _reportPath;

        public ReportViewModel(bool showResctricteCustomers, string title) 
            : base(showResctricteCustomers)
        {
            this.Title = title;
            _rptDataSource = new ReportDataSource[2];
        }

        public ReportViewer RptViewer
        {
            get
            {
                return _rptViewer;
            }

            set
            {
                _rptViewer = value;

                AddApplicationDetailsToReportDataSource();

                foreach (var dataSource in _rptDataSource)
                {
                    _rptViewer.LocalReport.DataSources.Add(dataSource);
                }
                _rptViewer.LocalReport.ReportPath = _reportPath;

                _rptViewer.RefreshReport();
            }
        }


        public void AddApplicationDetailsToReportDataSource()
        {
            _rptDataSource[1] = new ReportDataSource();

            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["RMSConnectionString"].ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    var appDt = new DataTable();
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from applicationDetails";
                    cmd.CommandType = CommandType.Text;
                    using (var adpt = new MySqlDataAdapter(cmd))
                    {
                        adpt.SelectCommand = cmd;
                        adpt.Fill(appDt);
                    }
                    _rptDataSource[1].Value = appDt;
                    _rptDataSource[1].Name = "DataSet2";
                }
            }
        }

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

        protected void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
        #endregion
    }
}
