using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports.Sales
{
    class ProductWiseSalesPercentViewModel : ReportViewModel
    {
        public Product SelectedProduct { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public IEnumerable<Product> ProductList { get; set; }

        public ProductWiseSalesPercentViewModel() :
            base(false, false, "Product Wise Sales Percentage Report")
        {
            ReportPath = @"View\Reports\Sales\ProductSalesPercentage.rdl";

            using (var rmsEntities = new RMSEntities())
            {
                ProductList = rmsEntities.Products.Where(p => p.IsActive == true || p.IsActive == null).ToList().OrderBy(p => p.Name);
            }
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
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

            var productsIds = ProductList.Where(c => c.IsChecked == true).Select(s => s.Id);
            var joinedProductIds = string.Join(",", productsIds);

            var query = " Select  " +
                        " p.Name, sum((sd.SellingPrice - sd.CostPrice) * sd.qty) profit, " +
                        " sum((sd.SellingPrice - sd.CostPrice) * sd.qty) / sum(sd.SellingPrice * sd.qty) * 100 percentage, " +
                        " sum(sd.qty) quantity " +
                        " from saledetails sd " +
                        " join products p on(sd.ProductId = p.id) " +
                        " where date(sd.AddedOn) >= @FromDate and date(sd.addedon) <= @ToDate " +
                        " and sd.ProductId in(" + joinedProductIds + ")" +
                        " group by p.id ";

            var fromSqlParam = new MySqlParameter("FromDate", MySqlDbType.Date)
            {
                Value = FromDate.ToString("yyyy-MM-dd")
            };

            var toSqlParam = new MySqlParameter("ToDate", MySqlDbType.Date)
            {
                Value = ToDate.ToString("yyyy-MM-dd")
            };



            //var productIdSqlParam = new MySqlParameter("ProductId", MySqlDbType.String)
            //{
            //    Value = joinedProductIds
            //};

            _rptDataSource[0].Value = GetDataTable(query, new MySqlParameter[2]
                                                {
                                                     fromSqlParam,toSqlParam
                                                },
                                                CommandType.Text);

            ReportParameters = new List<ReportParameter>(1)
            {
                new ReportParameter("DateRange", fromSqlParam.Value + " to " + toSqlParam.Value),
            };

            Workspace.This.OpenReport(this);
            CloseWindow(window);
        }
        #endregion

        #region Clear Command

        override internal void Clear()
        {
            ToDate = DateTime.Now;
            FromDate = DateTime.Now;
            SelectedProduct = null;
        }
        #endregion
    }
}
