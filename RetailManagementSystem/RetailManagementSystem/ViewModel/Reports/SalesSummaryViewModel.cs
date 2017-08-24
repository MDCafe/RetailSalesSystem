using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports
{
    class SalesSummaryViewModel : SalesViewModelbase
    {
        private DateTime _fromSalesDate;
        private DateTime _toSalesDate;
        private bool _showRestrictedCustomers;

        public DateTime FromSalesDate
        {
            get
            {
                return _fromSalesDate;
            }

            set
            {
                _fromSalesDate = value;
                RaisePropertyChanged("FromSalesDate");
            }
        }

        public DateTime ToSalesDate
        {
            get
            {
                return _toSalesDate;
            }

            set
            {
                _toSalesDate = value;
                RaisePropertyChanged("ToSalesDate");
            }
        }

        public SalesSummaryViewModel(bool showRestrictedCustomers) : base(showRestrictedCustomers)
        {
            FromSalesDate = DateTime.Now;
            ToSalesDate = DateTime.Now;
            
            _showRestrictedCustomers = showRestrictedCustomers;
            //RMSEntitiesHelper.Instance.RMSEntities.Customers.ToList();

            //var othersCategory = RMSEntitiesHelper.Instance.RMSEntities.Categories.FirstOrDefault(c => c.name == Constants.CUSTOMERS_OTHERS);
            //_othersCategoryId = othersCategory.Id;           

        }

        #region Print Command
        RelayCommand<Window> _printCommand = null;
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new RelayCommand<Window>((w) => OnPrint(w));
                }

                return _printCommand;
            }
        }

        private void OnPrint(Window window)
        {
            ReportDataSource[] rptDataSource = new ReportDataSource[2];

            rptDataSource[0] = new ReportDataSource();
            rptDataSource[1] = new ReportDataSource();

            rptDataSource[0].Name = "DataSet1";
            rptDataSource[1].Name = "DataSet2";//Name of the report dataset in our .RDLC file


            var query = "GetSales";

            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["RMSConnectionString"].ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    var fromSqlParam = new MySqlParameter("FromSalesDate", MySqlDbType.Date);
                    fromSqlParam.Value = FromSalesDate.ToString("yyyy-MM-dd");
                    cmd.Parameters.Add(fromSqlParam);

                    var toSqlParam = new MySqlParameter("ToSalesDate", MySqlDbType.Date);
                    toSqlParam.Value = ToSalesDate.ToString("yyyy-MM-dd");
                    cmd.Parameters.Add(toSqlParam);

                    DataTable dt = new DataTable();

                    MySqlDataAdapter adpt = new MySqlDataAdapter(cmd);

                    adpt.Fill(dt);

                    var appDt = new DataTable();
                    using (var cmd2 = new MySqlCommand())
                    {
                        cmd2.Connection = conn;
                        cmd2.CommandText = "select * from applicationDetails";
                        cmd2.CommandType = CommandType.Text;

                        adpt.SelectCommand = cmd2;

                        adpt.Fill(appDt);
                    }

                    rptDataSource[0].Value = dt;
                    rptDataSource[1].Value = appDt;
                }
            }

            Workspace.This.OpenSalesSummaryReport(_showRestrictedCustomers, rptDataSource);
            CloseWindow(window);
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

        #region Clear Command
        RelayCommand<object> _clearCommand = null;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand<object>((p) => OnClear());
                }

                return _clearCommand;
            }
        }

        private void OnClear()
        {
            ToSalesDate = DateTime.Now;
            FromSalesDate = DateTime.Now;
        }
        #endregion
    }
}
