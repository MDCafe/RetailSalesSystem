using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace RetailManagementSystem.ViewModel.Reports
{
    class SalesSummaryViewModel : ReportViewModel
    {
        private DateTime _fromSalesDate;
        private DateTime _toSalesDate;
        private readonly bool _showRestrictedCustomers;

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

        public IEnumerable<User> UsersList { get; set; }

        public int SelectedUserName { get; set; }

        public SalesSummaryViewModel(bool showRestrictedCustomers) : base(false,showRestrictedCustomers, 
                                     showRestrictedCustomers ? "Sales Summary Report *" : "Sales Summary Report")
        {
            FromSalesDate = DateTime.Now;
            ToSalesDate = DateTime.Now;
            UsersList = RMSEntitiesHelper.Instance.GetUsers();
            
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

            var categoryIdSqlParam = new MySqlParameter("categoryId", MySqlDbType.Int32)
            {
                Value = _categoryId
            };            
            
            var userSqlParam = new MySqlParameter("UserId", MySqlDbType.Int32)
            {
                Value = SelectedUserName
            };              
            
            _rptDataSource[0].Value = GetDataTable("GetSales",new MySqlParameter[4] 
                                                 {
                                                     fromSqlParam,toSqlParam,categoryIdSqlParam,userSqlParam
                                                 },
                                                 CommandType.StoredProcedure);
            
            Workspace.This.OpenReport(this);
            CloseWindow(window);
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
