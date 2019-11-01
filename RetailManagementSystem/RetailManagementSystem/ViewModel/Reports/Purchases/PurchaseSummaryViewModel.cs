using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using Microsoft.Reporting.WinForms;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Reports.Purhcases
{
    class PurchaseSummaryViewModel : ReportViewModel
    {
        private DateTime _fromPurchaseDate;
        private DateTime _toPurchaseDate;        

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

            //_showRestrictedCustomers = showRestrictedPeople;

             ReportPath = @"View\Reports\Purchases\PurchaseAllDetails.rdl";

            //override base array
            _rptDataSource = new ReportDataSource[4];
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
            _rptDataSource[0] = new ReportDataSource
            {
                Name = "DataSet1"
            };

            _rptDataSource[2] = new ReportDataSource
            {
                Name = "DataSet3"
            };

            _rptDataSource[3] = new ReportDataSource
            {
                Name = "DataSet4"
            };

            var purchaseQuery = "GetPurchases";
            var purchaseDetailsQuery = "GetPurchaseDetails";

            var returnQuery = "GetReturnsForBillid";
            
            SetReportDataSource(purchaseQuery, _rptDataSource[0]);
            SetReportDataSource(purchaseDetailsQuery, _rptDataSource[3]);
            SetReportDataSource(returnQuery, _rptDataSource[2]);
      
            Workspace.This.OpenReport(this);
            CloseWindow(window);
        }

        private void SetReportDataSource(string query, ReportDataSource rptDatasource)
        {
            using (var conn = MySQLDataAccess.GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    var runningBillNoSqlParam = new MySqlParameter("runningBillNo", MySqlDbType.Int32)
                    {
                        Value = RunningBillNo
                    };
                    cmd.Parameters.Add(runningBillNoSqlParam);

                    var categorySqlParam = new MySqlParameter("category", MySqlDbType.Int32)
                    {
                        Value = _categoryId
                    };
                    cmd.Parameters.Add(categorySqlParam);

                    using (DataTable dt = new DataTable())
                    {
                        using (MySqlDataAdapter adpt = new MySqlDataAdapter(cmd))
                        {
                            adpt.Fill(dt);
                        }
                        rptDatasource.Value = dt;
                    }
                }
            }
        }
        #endregion

        #region Clear Command

        override internal void Clear()
        {
            ToPurchaseDate = DateTime.Now;
            FromPurchaseDate = DateTime.Now;
            RunningBillNo = null;
        }
        #endregion
    }
}
