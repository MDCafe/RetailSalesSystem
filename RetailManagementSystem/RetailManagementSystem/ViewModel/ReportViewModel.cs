using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Data;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel
{
    internal class ReportViewModel : ReportBaseViewModel
    {
        private ReportViewer _rptViewer;
        protected ReportDataSource[] _rptDataSource;
        private string _reportPath;
        private PageSettings _pageSettings;
        private bool _showReportPrintButton = true;
        private Visibility _showPrintReceiptButton = Visibility.Collapsed;

        public bool ShowReportPrintButton
        {
            get { return _showReportPrintButton; }
            set { _showReportPrintButton = value; }
        }

        public ReportViewModel(bool isSupplier, bool showResctricteCustomers, string title) 
            : base(isSupplier,showResctricteCustomers)
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

                var pageSettings = _pageSettings;

                if(_pageSettings == null)
                {
                    //default page settings
                    PageSettings ps = new PageSettings();
                    ps.PaperSize = new PaperSize("A4", 827, 1170);
                    ps.Margins = new Margins(50, 0, 50, 0);
                    ps.PaperSize.RawKind = (int)PaperKind.A4;
                    pageSettings = ps;
                }

                _rptViewer.SetPageSettings(pageSettings);

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

            using (var conn = MySQLDataAccess.GetConnection())
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

        protected DataTable GetDataTable(string queryCustomer, MySqlParameter[] sqlParameter, CommandType commandType)
        {
            using (var conn = MySQLDataAccess.GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = queryCustomer;
                    cmd.Connection = conn;
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(sqlParameter);


                    DataTable dt = new DataTable();
                    MySqlDataAdapter adpt = new MySqlDataAdapter(cmd);

                    adpt.Fill(dt);

                    return dt;
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

        public string ReportPath
        {
            get
            {
                return _reportPath;
            }

            set
            {
                _reportPath = value;
            }
        }

        public PageSettings PageSettings
        {
            get
            {
                return _pageSettings;
            }

            set
            {
                _pageSettings = value;
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

        public RelayCommand<object> _printReceiptCommand { get; private set; }

        public ICommand PrintReceiptCommand
        {
            get
            {
                if (_printReceiptCommand == null)
                {
                    _printReceiptCommand = new RelayCommand<object>((p) => PrintReceipt(p));
                }

                return _printReceiptCommand;
            }
        }

        public Visibility ShowPrintReceiptButton
        {
            get
            {
                return _showPrintReceiptButton;
            }

            set
            {
                _showPrintReceiptButton = value;
            }
        }

        public virtual void PrintReceipt(object p)
        {
            throw new NotImplementedException();
        }
    }
}
