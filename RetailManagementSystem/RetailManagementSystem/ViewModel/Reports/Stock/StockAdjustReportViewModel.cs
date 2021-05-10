using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports.Stock
{
    class StockAdjustReportViewModel : ReportViewModel
    {
        private Product _selectedProduct;

        public DateTime FromSalesDate { get; set; }
        public DateTime ToSalesDate { get; set; }

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

        public IEnumerable<Product> ProductList { get; set; }

        public StockAdjustReportViewModel() : base(false, false, "Stock Adjustment Report")
        {
            FromSalesDate = DateTime.Now;
            ToSalesDate = DateTime.Now;

            ReportPath = @"View\Reports\Stock\StockAdjustment.rdl";

            using (RMSEntities rmsEntities = new RMSEntities())
            {
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



            string query = "";

            if (SelectedProduct != null)
            {
                var productIdParam = new MySqlParameter("productId", MySqlDbType.Int64)
                {
                    Value = SelectedProduct.Id
                };

                query = "Select p.Name,sa.* from StockAdjustments sa inner join stocks s on (sa.StockId = s.id) " +
                        " inner join products p on (p.Id = s.productId) " +
                        " where date(sa.Addedon) >= @FromSalesDate  and date(sa.Addedon) <= @ToSalesDate and p.id = @productId";

                _rptDataSource[0].Value = GetDataTable(query, new MySqlParameter[3]
                                                 {
                                                     fromSqlParam,toSqlParam,productIdParam
                                                 },
                                                 CommandType.Text);
            }
            else
            {
                query = "Select p.Name,sa.* from StockAdjustments sa inner join stocks s on (sa.StockId = s.id) " +
                            " inner join products p on (p.Id = s.productId) " +
                            " where date(sa.Addedon) >= @FromSalesDate  and date(sa.Addedon) <= @ToSalesDate";

                _rptDataSource[0].Value = GetDataTable(query, new MySqlParameter[2]
                                                {
                                                     fromSqlParam,toSqlParam
                                                },
                                                CommandType.Text);
            }

            ReportParameterValue = new ReportParameter("AdjustmentPeriod", fromSqlParam.Value + " to " + toSqlParam.Value);

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
