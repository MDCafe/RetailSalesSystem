using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Reports.Sales
{
    class SalesBillDetailsViewModel : ReportViewModel
    {
        private readonly bool _showRestrictedCustomers;

        public int? RunningBillNo { get; set; }
        

        public SalesBillDetailsViewModel(bool showRestrictedCustomers,int? runningNo) : 
            base(false,showRestrictedCustomers,"Sales Bill Report - " + runningNo)
        {
            
            _showRestrictedCustomers = showRestrictedCustomers;

            ReportPath = @"View\Reports\Sales\SalesBill.rdl";
            ShowReportPrintButton = false;
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

        public override void PrintReceipt(object p)
        {
            var dataTable = GetDataTable();
            var billSales = new Sale();
            var salesDetails = new List<SaleDetailExtn>();
            var customer = "";
            billSales.RunningBillNo = RunningBillNo.Value;
            

            foreach (var item in dataTable.Rows)
            {
                var itemArray = item as DataRow;
                customer =  itemArray.Field<string>("Customer");
                billSales.CustomerOrderNo = itemArray.Field<string>("CustomerOrderNo");
                billSales.TransportCharges = itemArray.IsNull("TransportCharges") ? 0.0M : itemArray.Field<decimal>("TransportCharges");
                billSales.Discount = itemArray.IsNull("Discount") ? 0.0M : itemArray.Field<decimal>("Discount");
                billSales.TotalAmount   = itemArray.Field<decimal>("TotalAmount");
                billSales.PaymentMode = itemArray.Field<string>("PaymentMode");
                billSales.AddedOn = itemArray.Field<System.DateTime>("AddedOn");
                salesDetails.Add(
                    new SaleDetailExtn()
                    {
                        AddedOn = itemArray.Field<System.DateTime>("AddedOn"),
                        SellingPrice = itemArray.Field<decimal>("SellingPrice"),
                        Qty = itemArray.Field<decimal>("Qty"),
                        Discount = itemArray.IsNull("ItemDiscount") ? 0.0M : itemArray.Field<decimal>("ItemDiscount"),
                        ProductId = itemArray.Field<int>("ProductId"),                        
                        CostPrice = itemArray.Field<decimal>("Price")
                    }
                );
            }
            using (RMSEntities rmsEntities = new RMSEntities())
            {
                UserControls.SalesBillPrint sp = new UserControls.SalesBillPrint(rmsEntities);
                sp.Print(customer, salesDetails, billSales, billSales.TotalAmount.Value, null, null, _showRestrictedCustomers);
            }
        }

        private void OnPrint(Window window)
        {
            _rptDataSource[0] = new ReportDataSource
            {
                Name = "DataSet1",
                Value = GetDataTable()
            };

            Workspace.This.OpenReport(this);
            CloseWindow(window);
        }

        private DataTable GetDataTable()
        {
            var query = "GetSalesDetailsForBillId";
            using (var conn = MySQLDataAccess.GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    var runningBillNoSqlParam = new MySqlParameter("runningBillNo", MySqlDbType.Int32)
                    {
                        Value = RunningBillNo
                    };
                    cmd.Parameters.Add(runningBillNoSqlParam);

                    var categorySqlParam = new MySqlParameter("category", MySqlDbType.Int32)
                    {
                        Value = _categoryId
                    };
                    cmd.Parameters.Add(categorySqlParam);

                    DataTable dt = new DataTable();
                    using (MySqlDataAdapter adpt = new MySqlDataAdapter(cmd))
                    {
                        adpt.Fill(dt);
                    }

                    dt.Columns.Add("QtyString", typeof(string));
                    //format Qty
                    foreach (var row in dt.Rows)
                    {
                        var r = (DataRow)row;
                        var qty = r.Field<decimal>("Qty");
                        if(qty.ToString().Split(new char[] { '.'})[1] == "000")
                        {
                            //var qtyInt = (int)qty;
                            r.SetField<string>("QtyString", qty.ToString("######"));
                        }else
                        r.SetField<string>("QtyString", qty.ToString("0.##0"));
                    }
                    return dt;
                }
            }
        }
        #endregion
    }
}
