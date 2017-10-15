using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports.Sales
{
    class SalesBillDetailsViewModel : ReportViewModel
    {
        private bool _showRestrictedCustomers;

        public int? RunningBillNo { get; set; }

        public SalesBillDetailsViewModel(bool showRestrictedCustomers) : base(false,showRestrictedCustomers,"Sales Summary")
        {

            _showRestrictedCustomers = showRestrictedCustomers;

            ReportPath = @"View\Reports\Sales\SalesBill.rdl";
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

            var query = "GetSalesDetailsForBillId";

            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["RMSConnectionString"].ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    var runningBillNoSqlParam = new MySqlParameter("runningBillNo", MySqlDbType.Int32);
                    runningBillNoSqlParam.Value = RunningBillNo;
                    cmd.Parameters.Add(runningBillNoSqlParam);

                    var categorySqlParam = new MySqlParameter("category", MySqlDbType.Int32);
                    categorySqlParam.Value = _categoryId;
                    cmd.Parameters.Add(categorySqlParam);

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
    }
}
