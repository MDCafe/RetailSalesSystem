using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports.Purhcases
{
    class PurchaseSummaryViewModel : ReportViewModel
    {
        private DateTime _fromPurchaseDate;
        private DateTime _toPurchaseDate;
        private bool _showRestrictedCustomers;

        public DateTime FromPurchaseDate
        {
            get
            {
                return _fromPurchaseDate;
            }

            set
            {
                _fromPurchaseDate = value;
                RaisePropertyChanged("FromPurchaseDate");
            }
        }

        public DateTime ToPurchaseDate
        {
            get
            {
                return _toPurchaseDate;
            }

            set
            {
                _toPurchaseDate = value;
                RaisePropertyChanged("ToPurchaseDate");
            }
        }

        public PurchaseSummaryViewModel(bool showRestrictedCustomers) : base(showRestrictedCustomers,"Purchase Summary")
        {
            FromPurchaseDate = DateTime.Now;
            ToPurchaseDate = DateTime.Now;

            _showRestrictedCustomers = showRestrictedCustomers;

            _reportPath = @"View\Reports\Purchases\PurchasesSummary.rdl";

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

            var query = "GetPurchases";

            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["RMSConnectionString"].ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    var fromSqlParam = new MySqlParameter("FromPurchaseDate", MySqlDbType.Date);
                    fromSqlParam.Value = FromPurchaseDate.ToString("yyyy-MM-dd");
                    cmd.Parameters.Add(fromSqlParam);

                    var toSqlParam = new MySqlParameter("ToPurchaseDate", MySqlDbType.Date);
                    toSqlParam.Value = ToPurchaseDate.ToString("yyyy-MM-dd");
                    cmd.Parameters.Add(toSqlParam);

                    DataTable dt = new DataTable();
                    using (MySqlDataAdapter adpt = new MySqlDataAdapter(cmd))
                    {
                        adpt.Fill(dt);
                    }

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
            ToPurchaseDate = DateTime.Now;
            FromPurchaseDate = DateTime.Now;
        }
        #endregion
    }
}
