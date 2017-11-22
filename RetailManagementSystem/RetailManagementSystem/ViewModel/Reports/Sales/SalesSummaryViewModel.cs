using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports
{
    class SalesSummaryViewModel : ReportViewModel
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

        public SalesSummaryViewModel(bool showRestrictedCustomers) : base(false,showRestrictedCustomers,"Sales Summary Report")
        {
            FromSalesDate = DateTime.Now;
            ToSalesDate = DateTime.Now;
            
            _showRestrictedCustomers = showRestrictedCustomers;

            ReportPath = @"View\Reports\Sales\SalesSummary.rdl";

            
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
            _rptDataSource[0] = new ReportDataSource();
            _rptDataSource[0].Name = "DataSet1";

            var query = "GetSales";

            using (var conn = MySQLDataAccess.GetConnection())
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

                    _rptDataSource[0].Value = dt;
                }
            }

            Workspace.This.OpenReport(this);
            CloseWindow(window);
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
