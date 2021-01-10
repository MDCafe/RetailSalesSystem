using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Utilities;
using System.Data;

namespace RetailManagementSystem.ViewModel.Reports.Stock
{
    class ProductsOrderReportViewModel : ReportViewModel
    {
        public ProductsOrderReportViewModel(bool showRestrictedCustomers) : base(false, showRestrictedCustomers, "Products to Order Report")
        {
            ReportPath = @"View\Reports\Stock\ProductsToOrder.rdl";
        }

        public void ShowReport()
        {
            _rptDataSource[0] = new ReportDataSource();
            _rptDataSource[0].Name = "DataSet1";

            var query = "select p.Name ,sum(st.Quantity) 'Available Qty', c.name CategoryName,p.ReorderPoint " +
                        "from stocks st, Products p, Category c " +
                        " where " +
                        "st.ProductId = p.id " +
                        "and p.CategoryId = c.Id " +
                        "group by c.id,st.ProductId,p.ReorderPoint " +
                        "having Sum(st.Quantity) <= p.ReorderPoint " +
                        "order by c.name,p.Name";

            using (var conn = MySQLDataAccess.GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    using (DataTable dt = new DataTable())
                    {
                        using (MySqlDataAdapter adpt = new MySqlDataAdapter(cmd))
                        {
                            adpt.Fill(dt);
                        }
                        _rptDataSource[0].Value = dt;
                    }
                }
            }
            Workspace.This.OpenReport(this);
        }
    }
}
