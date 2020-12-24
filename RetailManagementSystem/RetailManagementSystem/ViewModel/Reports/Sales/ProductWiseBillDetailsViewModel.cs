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
    class ProductWiseBillDetailsViewModel : ReportViewModel
    {
        private bool _showRestrictedCustomers;
        public Product SelectedProduct { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public IEnumerable<Product> ProductList { get; set; }

        public ProductWiseBillDetailsViewModel(bool showRestrictedCustomers) :
            base(false, showRestrictedCustomers, "Product Wise Sales Report")
        {
            _showRestrictedCustomers = showRestrictedCustomers;
            ReportPath = @"View\Reports\Sales\ProductWiseBillDetails.rdl";

            using (var rmsEntities = new RMSEntities())
            {
                ProductList = rmsEntities.Products.ToList().OrderBy(p => p.Name);
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

            _rptDataSource[0] = new ReportDataSource();
            _rptDataSource[0].Name = "DataSet1";

            var query = "select " +
                        "    s.BillId,s.RunningbillNo,c.Name,sd.qty,s.TotalAmount,s.IsCancelled,s.CustomerOrderNo,s.AddedOn, " +
                        "    CASE " +
                        "        WHEN s.PaymentMode = 0 THEN 'Cash'" +
                        "        WHEN s.PaymentMode = 1 THEN 'Credit'" +
                        "        ELSE 'Cheque' " +
                        "    END as PaymentMode " +
                        "    from saleDetails sd join sales s on (s.BillId = sd.BillId) " +
                        "                        join customers c on (c.id = s.customerId) " +
                        "    where sd.ProductId = @ProductId and date(sd.AddedOn) >= @FromDate and date(sd.addedon) <= @ToDate";

            var fromSqlParam = new MySqlParameter("FromDate", MySqlDbType.Date)
            {
                Value = FromDate.ToString("yyyy-MM-dd")
            };

            var toSqlParam = new MySqlParameter("ToDate", MySqlDbType.Date)
            {
                Value = ToDate.ToString("yyyy-MM-dd")
            };

            var productIdSqlParam = new MySqlParameter("ProductId", MySqlDbType.Int32)
            {
                Value = SelectedProduct.Id
            };

            _rptDataSource[0].Value = GetDataTable(query, new MySqlParameter[3]
                                                {
                                                     productIdSqlParam,fromSqlParam,toSqlParam
                                                },
                                                CommandType.Text);

            ReportParameters = new List<ReportParameter>(2)
            {
                new ReportParameter("DateRange", fromSqlParam.Value + " to " + toSqlParam.Value),
                new ReportParameter("ProductName", SelectedProduct.Name),
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
