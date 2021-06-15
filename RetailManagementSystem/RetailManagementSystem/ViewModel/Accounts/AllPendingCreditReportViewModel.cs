using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using System.Data;

namespace RetailManagementSystem.ViewModel.Reports.Accounts
{
    class AllPendingCreditReportViewModel : ReportViewModel
    {
        public AllPendingCreditReportViewModel(bool showRestrictedCustomers) : base(false, showRestrictedCustomers, "All Pending Credit Report")
        {
            ReportPath = @"View\Reports\Accounts\AllPendingCredits.rdl";
            _rptDataSource[0] = new ReportDataSource
            {
                Name = "DataSet1"
            };

            var queryDirectPayment = "Select * from customers where CustomerTypeId=@customerTypeId order by Name";
            var customerIdDirectPaySqlParam = new MySqlParameter("customerTypeId", MySqlDbType.Int32)
            {
                Value = CategoryId
            };
            _rptDataSource[0].Value = GetDataTable(queryDirectPayment, new MySqlParameter[1] { customerIdDirectPaySqlParam }, CommandType.Text);
            Workspace.This.OpenReport(this);
        }
    }
}
