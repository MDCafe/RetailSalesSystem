using Microsoft.Reporting.WinForms;
using RetailManagementSystem.Command;
using System;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports.Accounts
{
    class DayStatementReportViewModel : ReportViewModel
    {
        private readonly bool _showRestrictedCustomers;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public DayStatementReportViewModel(bool showRestrictedCustomers) : base(false, showRestrictedCustomers,
                                     showRestrictedCustomers ? "Day Statement Report *" : "Day Statement Report")
        {
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
            _showRestrictedCustomers = showRestrictedCustomers;
            ReportPath = @"View\Reports\Accounts\DayStatement.rdl";
            _rptDataSource = new ReportDataSource[6];
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
            var salesQuery = "select sum(TotalAmount) TotalSalesAmount from ( " +
                            " select sum(AmountPaid) as TotalAmount from sales where date(AddedOn) = @fromDate and(PaymentMode = 1 or PaymentMode = 2) " +
                            " union all " +
                            " select sum(TotalAmount) as TotalAmount from sales where date(AddedOn) = @fromDate and PaymentMode = 0" +
                            " ) a; ";

            _rptDataSource[0] = GetReportDataSource(salesQuery, "SalesDataSet");

            var expenseQuery = "select cm.Description,sum(amount) Amount from ExpenseDetails exp, CodeMaster cm  " +
                                " where " +
                                " date(addedOn) = @fromDate and cm.Id = exp.ExpenseTypeId and cm.Description !='InCash'" +
                                " group by exp.ExpenseTypeId";

            _rptDataSource[2] = GetReportDataSource(expenseQuery, "ExpDataSet");

            var cashPurchaseQuery = "select c.Name,p.TotalBillAmount from Purchases p, companies c " +
                                    " where paymentMode = 0 and date(p.addedOn) = @fromDate " +
                                    " and p.CompanyId = c.Id";

            _rptDataSource[3] = GetReportDataSource(cashPurchaseQuery, "CashPurchaseDataSet");

            var amountPaidQuery = "select c.name, sum(AmountPaid) AmonutPaid from PaymentDetails pd, Customers c  " +
                                  " where date(paymentDate) = @fromDate and pd.customerId = c.Id " +
                                  " group by c.Id having AmonutPaid !=0";

            _rptDataSource[4] = GetReportDataSource(amountPaidQuery, "CustomerPaymentsDataSet");


            var IncashQuery = "select cm.Description,sum(amount) Amount from ExpenseDetails exp, CodeMaster cm  " +
                                " where " +
                                " date(addedOn) = @fromDate and cm.Id = exp.ExpenseTypeId and cm.Description ='InCash'" +
                                " group by exp.ExpenseTypeId";

            _rptDataSource[5] = GetReportDataSource(IncashQuery, "InCashDataSet");

            ReportParameterValue = new ReportParameter("DateFilter", FromDate.ToString("dd-MM-yyyy"));

            Workspace.This.OpenReport(this);
            CloseWindow(window);
        }

        private ReportDataSource GetReportDataSource(string query, string datasetName)
        {
            return new ReportDataSource
            {
                Name = datasetName,
                Value = GetDataTable(query, new MySql.Data.MySqlClient.MySqlParameter[1]
                        {
                            new MySql.Data.MySqlClient.MySqlParameter("fromDate",FromDate.ToString("yyyy-MM-dd"))
                        }
                        , System.Data.CommandType.Text
                        )
            };
        }
        #endregion



        #region Clear Command

        override internal void Clear()
        {
            ToDate = DateTime.Now;
            FromDate = DateTime.Now;
        }

        #endregion
    }
}
