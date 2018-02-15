using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using Microsoft.Reporting.WinForms;

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

        public int? RunningBillNo { get; set; }

        public PurchaseSummaryViewModel(bool showRestrictedPeople,int? billNo) : 
            base(true, showRestrictedPeople, "Purchase Detail Report - " + billNo)
        {
            FromPurchaseDate = DateTime.Now;
            ToPurchaseDate = DateTime.Now;

            _showRestrictedCustomers = showRestrictedPeople;

             ReportPath = @"View\Reports\Purchases\PurchaseAllDetails.rdl";
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

            using (var conn = MySQLDataAccess.GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    var runningBillNoSqlParam = new MySqlParameter("runningBillNo", MySqlDbType.Int32);
                    runningBillNoSqlParam.Value = RunningBillNo;
                    cmd.Parameters.Add(runningBillNoSqlParam);

                    var categorySqlParam = new MySqlParameter("category", MySqlDbType.Int32);
                    categorySqlParam.Value = _categoryId;
                    cmd.Parameters.Add(categorySqlParam);

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
            RunningBillNo = null;
        }
        #endregion
    }
}
