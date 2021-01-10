using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports.Stock
{
    class StockBalanceReportViewModel : ReportViewModel
    {
        private Company _selectedCompany;
        private Category _selectedCategory;
        private Product _selectedProduct;
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

        public StockBalanceReportViewModel(bool showRestrictedCustomers) : base(false, showRestrictedCustomers, "Stock Balance Report")
        {
            ReportPath = @"View\Reports\Stock\StockBalance.rdl";
            using (RMSEntities rmsEntities = new RMSEntities())
            {
                ProductCategories = rmsEntities.Categories.Where(c => c.parentId == 3).ToList().OrderBy(p => p.name);
                _companiesList = rmsEntities.Companies.ToList().OrderBy(c => c.Name);
                ProductList = rmsEntities.Products.ToList().OrderBy(p => p.Name);
            }
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

            var query = "GetStockBalances";

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

                    companySqlParam.Value = (_selectedCompany != null) ? SelectedCompany.Id : 0;
                    categorySqlParam.Value = (_selectedCategory != null) ? _selectedCategory.Id : 0;
                    productSqlParam.Value = (_selectedProduct != null) ? _selectedProduct.Id : 0;

                    cmd.Parameters.Add(companySqlParam);
                    cmd.Parameters.Add(categorySqlParam);
                    cmd.Parameters.Add(productSqlParam);

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
            SelectedCategory = null;
            SelectedCompany = null;
            SelectedProduct = null;
        }
        #endregion
    }
}
