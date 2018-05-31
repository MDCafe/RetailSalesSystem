using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports.Accounts
{
    class CustomerPaymentDetailsRptViewModel : ReportViewModel
    {
        private DateTime _fromSalesDate;
        private DateTime _toSalesDate;
        private int _customerId;
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

        public int CustomerId
        {
            get
            {
                return _customerId;
            }

            set
            {
                _customerId = value;
                RaisePropertyChanged("CustomerId");
            }
        }

        public Customer SelectedCustomer { get; set; }

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                using (RMSEntities rmsEntities = new RMSEntities())
                {
                    return rmsEntities.Customers.ToList().Where(c => c.CustomerTypeId == _categoryId).OrderBy(a => a.Name);
                }
            }
        }

        public CustomerPaymentDetailsRptViewModel(bool showRestrictedCustomers) : base(false,showRestrictedCustomers, "Customer Wise Sales Report")
        {
            FromSalesDate = DateTime.Now;
            ToSalesDate = DateTime.Now;
            _showRestrictedCustomers = showRestrictedCustomers;
            _rptDataSource = new ReportDataSource[4];

            ReportPath = @"View\Reports\Accounts\CustomerPaymentDetails.rdl";
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

            var query = "GetCustomerPaymentDetailsReport";

            var fromSqlParam = new MySqlParameter("fromDate", MySqlDbType.Date);
            fromSqlParam.Value = FromSalesDate.ToString("yyyy-MM-dd");
            var toSqlParam = new MySqlParameter("toDate", MySqlDbType.Date);
            toSqlParam.Value = ToSalesDate.ToString("yyyy-MM-dd");
            var categoryIdSqlParam = new MySqlParameter("customerId", MySqlDbType.Int32)
            {
                Value = _customerId
            };
            _rptDataSource[0].Value = GetDataTable(query, new MySqlParameter[] { fromSqlParam,toSqlParam,categoryIdSqlParam }, CommandType.StoredProcedure);                    

            var queryCustomer = "Select Name,OldBalanceDue from customers where Id=@customerId";
            var customerIdSqlParam = new MySqlParameter("customerId", MySqlDbType.Int32)
            {
                Value = _customerId
            };
            _rptDataSource[2].Value = GetDataTable(queryCustomer, new MySqlParameter[1] { customerIdSqlParam }, CommandType.Text);


            var queryDirectPayment = "Select * from DirectPaymentDetails where CustomerId=@customerId";
            var customerIdDirectPaySqlParam = new MySqlParameter("customerId", MySqlDbType.Int32)
            {
                Value = _customerId
            };
            _rptDataSource[3].Value = GetDataTable(queryDirectPayment, new MySqlParameter[1] { customerIdDirectPaySqlParam }, CommandType.Text);

            Workspace.This.OpenReport(this);
            CloseWindow(window);
        }

        private DataTable GetDataTable(string queryCustomer, MySqlParameter[] sqlParameter, CommandType commandType)
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
        #endregion

        #region Clear Command

        internal override void Clear()
        {
            ToSalesDate = DateTime.Now;
            FromSalesDate = DateTime.Now;
            CustomerId = -1;
        }
        #endregion

    }
}
