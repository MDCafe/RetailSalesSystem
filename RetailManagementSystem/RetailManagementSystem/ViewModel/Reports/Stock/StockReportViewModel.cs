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

namespace RetailManagementSystem.ViewModel.Reports.Stock
{
    class StockReportViewModel : ReportViewModel
    {
        private Company _selectedCompany;
        private Category _selectedCategory;
        private Product _selectedProduct;
        private DateTime _fromDate;
        private DateTime _toDate;
        IEnumerable<Company> _companiesList;

        public Company SelectedCompany
        {
            get
            {
                return _selectedCompany;
            }

            set
            {
                _selectedCompany = value;
                RaisePropertyChanged(nameof(SelectedCompany));
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
                RaisePropertyChanged(nameof(SelectedCategory));
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
                RaisePropertyChanged(nameof(SelectedProduct));
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
                RaisePropertyChanged(nameof(FromDate));
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
                RaisePropertyChanged(nameof(ToDate));
            }
        }

        public IEnumerable<Company> Companies
        {
            get
            {
                if (_companiesList == null)
                {
                    using (RMSEntities rmsEntities = new RMSEntities())
                    {
                        _companiesList = rmsEntities.Companies.ToList();
                    }
                }
                return _companiesList;
            }

            private set
            {
                _companiesList = value;
            }
        }

        public IEnumerable<Category> ProductCategories { get; set; }

        public IEnumerable<Product> ProductList { get; set; }

        public StockReportViewModel(bool showRestrictedCustomers) : base(false, showRestrictedCustomers, "Stock Report")
        {

            ReportPath = @"View\Reports\Stock\StockDetails.rdl";
            using (RMSEntities rmsEntities = new RMSEntities())
            {
                ProductCategories = rmsEntities.Categories.Where(c => c.parentId == 3).ToList().OrderBy(p => p.name);
                _companiesList = rmsEntities.Companies.ToList().OrderBy(c => c.Name);
                ProductList = rmsEntities.Products.ToList().OrderBy(p => p.Name);
            }
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
            if (FromDate.CompareTo(ToDate) > 0)
            {
                Utilities.Utility.ShowErrorBox("From date can't be more than To date");
                return;
            }

            _rptDataSource[0] = new ReportDataSource
            {
                Name = "DataSet1"
            };

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

        override internal void Clear()
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
