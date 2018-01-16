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

namespace RetailManagementSystem.ViewModel.Reports.Stock
{
    class StockReportViewModel : ReportViewModel
    {
        private Company _selectedCompany;
        private Category _selectedCategory;
        private Product _selectedProduct;
        private DateTime _fromDate;
        private DateTime _toDate;
        private bool _showRestrictedCustomers;
        RMSEntities _rmsEntities;

        IEnumerable<Category> _productsCategory;
        IEnumerable<Company> _companiesList;
        IEnumerable<Product> _productList;

        public Company SelectedCompany
        {
            get
            {
                return _selectedCompany;
            }

            set
            {
                _selectedCompany = value;
                RaisePropertyChanged("SelectedCompany");
            }
        }

        public Category SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }

            set
            {
                _selectedCategory = value;
                RaisePropertyChanged("SelectedCategory");
            }
        }

        public Product SelectedProduct
        {
            get
            {
                return _selectedProduct;
            }

            set
            {
                _selectedProduct = value;
                RaisePropertyChanged("SelectedProduct");
            }
        }

        public DateTime FromDate
        {
            get
            {
                return _fromDate;
            }

            set
            {
                _fromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }

        public DateTime ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                _toDate = value;
                RaisePropertyChanged("ToDate");
            }
        }

        public IEnumerable<Company> Companies
        {
            get
            {
                if (_companiesList == null)
                    _companiesList = _rmsEntities.Companies.ToList();

                return _companiesList;
            }

            private set
            {
                _companiesList = value;
            }
        }

        public IEnumerable<Category> ProductCategories
        {
            get
            {
                return _productsCategory;
            }

            set
            {
                _productsCategory = value;
            }
        }

        public IEnumerable<Product> ProductList
        {
            get
            {
                return _productList;
            }
            set
            {
                _productList = value;
            }
        }

        public StockReportViewModel(bool showRestrictedCustomers) : base(false,showRestrictedCustomers,"Stock Report")
        {
            _showRestrictedCustomers = showRestrictedCustomers;
            ReportPath = @"View\Reports\Stock\StockDetails.rdl";

            _rmsEntities = new RMSEntities();
            _productsCategory = _rmsEntities.Categories.Where(c => c.parentId == 3).ToList().OrderBy(p => p.name);
            _companiesList = _rmsEntities.Companies.ToList().OrderBy(c => c.Name);
            _productList = _rmsEntities.Products.ToList().OrderBy(p => p.Name);
            _fromDate = DateTime.Now;
            _toDate = DateTime.Now;
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
            if(FromDate.CompareTo(ToDate) > 0)
            {
                Utilities.Utility.ShowErrorBox("From date can't be more than To date");
                return;
            }

            _rptDataSource[0] = new ReportDataSource();
            _rptDataSource[0].Name = "DataSet1";

            var query = "GetStockDetails";

            using (var conn = MySQLDataAccess.GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    var companySqlParam = new MySqlParameter("companyId", MySqlDbType.Int32);
                    var categorySqlParam = new MySqlParameter("categoryId", MySqlDbType.Int32);
                    var productSqlParam = new MySqlParameter("productId", MySqlDbType.Int32);
                    var fromDateSqlParam = new MySqlParameter("FromDate", MySqlDbType.Date);
                    var toDateSqlParam = new MySqlParameter("ToDate", MySqlDbType.Date);

                    companySqlParam.Value = (_selectedCompany != null) ? SelectedCompany.Id : 0;
                    categorySqlParam.Value = (_selectedCategory != null) ? _selectedCategory.Id : 0;
                    productSqlParam.Value = (_selectedProduct != null) ? _selectedProduct.Id : 0;
                    fromDateSqlParam.Value = FromDate.ToString("yyyy-MM-dd");
                    toDateSqlParam.Value = ToDate.ToString("yyyy-MM-dd");

                    cmd.Parameters.Add(companySqlParam);
                    cmd.Parameters.Add(categorySqlParam);
                    cmd.Parameters.Add(productSqlParam);
                    cmd.Parameters.Add(fromDateSqlParam);
                    cmd.Parameters.Add(toDateSqlParam);

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
            ToDate = DateTime.Now;
            FromDate = DateTime.Now;
            SelectedCategory = null;
            SelectedCompany = null;
            SelectedProduct = null;
        }
        #endregion
    }
}