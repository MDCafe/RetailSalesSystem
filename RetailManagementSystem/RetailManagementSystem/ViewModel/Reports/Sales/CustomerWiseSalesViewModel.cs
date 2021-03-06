﻿using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports
{
    class CustomerWiseSalesViewModel : ReportViewModel
    {
        private DateTime _fromSalesDate;
        private DateTime _toSalesDate;
        private int _customerId;
        //private bool _showRestrictedCustomers;

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
                    return rmsEntities.Customers.ToList().Where(c => c.CustomerTypeId == CategoryId).OrderBy(a => a.Name);
                }
            }
        }

        public CustomerWiseSalesViewModel(bool showRestrictedCustomers) : base(false, showRestrictedCustomers, "Customer Wise Sales Report")
        {
            FromSalesDate = DateTime.Now;
            ToSalesDate = DateTime.Now;
            //_showRestrictedCustomers = showRestrictedCustomers;

            ReportPath = @"View\Reports\Sales\Customers\CustomerWiseSales.rdl";
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

            var query = "GetCustomerWiseSales";

            using (var conn = MySQLDataAccess.GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    var fromSqlParam = new MySqlParameter("fromDate", MySqlDbType.Date)
                    {
                        Value = FromSalesDate.ToString("yyyy-MM-dd")
                    };
                    cmd.Parameters.Add(fromSqlParam);

                    var toSqlParam = new MySqlParameter("toDate", MySqlDbType.Date)
                    {
                        Value = ToSalesDate.ToString("yyyy-MM-dd")
                    };
                    cmd.Parameters.Add(toSqlParam);

                    var categoryIdSqlParam = new MySqlParameter("customerId", MySqlDbType.Int32)
                    {
                        Value = _customerId
                    };
                    cmd.Parameters.Add(categoryIdSqlParam);

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

        internal override void Clear()
        {
            ToSalesDate = DateTime.Now;
            FromSalesDate = DateTime.Now;
            CustomerId = -1;
        }
        #endregion
    }
}
