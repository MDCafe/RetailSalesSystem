using RetailManagementSystem.ViewModel.Sales;
using System.Windows;
using System.Windows.Controls;

namespace RetailManagementSystem.View.Sales
{
    public partial class AmendSales : Window
    {
        readonly AmendSalesViewModel amVM;

        public AmendSales(bool showRestrictedCustomers)
        {
            InitializeComponent();
            amVM = new AmendSalesViewModel(showRestrictedCustomers);
            DataContext = amVM;
            BillsDataGrid.MouseDoubleClick += BillsDataGrid_MouseDoubleClick;
        }

        private void BillsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dataGrid = sender as Microsoft.Windows.Controls.DataGrid;
            amVM.BillNo = ((Sale)(((ItemsControl)sender).Items[dataGrid.SelectedIndex])).RunningBillNo;
        }
    }
}
