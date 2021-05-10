using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.View;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports
{
    class POSSalesSummaryViewModel : ReportViewModel
    {
        private DateTime _fromSalesDate;
        private DateTime _toSalesDate;

        public DateTime FromSalesDate
        {
            get
            {
                return _fromSalesDate;
            }

            set
            {
                _fromSalesDate = value;
                RaisePropertyChanged(nameof(FromSalesDate));
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
                RaisePropertyChanged(nameof(ToSalesDate));
            }
        }

        public POSSalesSummaryViewModel() : base(false, false, "Sales Summary Report")
        {
            FromSalesDate = DateTime.Now;
            ToSalesDate = DateTime.Now;
            ReportPath = @"View\Reports\Sales\POSSalesSummary.rdl";
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


            var fromSqlParam = new MySqlParameter("FromSalesDate", MySqlDbType.Date)
            {
                Value = FromSalesDate.ToString("yyyy-MM-dd")
            };

            var toSqlParam = new MySqlParameter("ToSalesDate", MySqlDbType.Date)
            {
                Value = ToSalesDate.ToString("yyyy-MM-dd")
            };

            var userSqlParam = new MySqlParameter("internalUserId", MySqlDbType.Int32)
            {
                Value = Entitlements.EntitlementInformation.UserInternalId
            };

            _rptDataSource[0].Value = GetDataTable("GetPOSSales", new MySqlParameter[3]
                                                 {
                                                     fromSqlParam,toSqlParam,userSqlParam
                                                 },
                                                 CommandType.StoredProcedure);

            CloseWindow(window);

            ReportsViewerWindow reportsViewerWindow = new ReportsViewerWindow
            {
                DataContext = this
            };
            reportsViewerWindow.ShowDialog();

        }
        #endregion

        #region Clear Command

        override internal void Clear()
        {
            ToSalesDate = DateTime.Now;
            FromSalesDate = DateTime.Now;
        }
        #endregion
    }
}
