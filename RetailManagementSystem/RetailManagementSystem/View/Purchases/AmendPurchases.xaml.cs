using RetailManagementSystem.ViewModel.Sales;
using System.Windows;
using System.Windows.Controls;

namespace RetailManagementSystem.View.Sales
{
    public partial class AmendPurchases : Window
    {
        AmendPurchasesViewModel amVM;

        public AmendPurchases(bool showRestrictedCustomers)
        {
            InitializeComponent();
            amVM = new AmendPurchasesViewModel(showRestrictedCustomers);
            DataContext = amVM;
            BillsDataGrid.MouseDoubleClick += BillsDataGrid_MouseDoubleClick;
        }

        private void BillsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dataGrid = sender as Microsoft.Windows.Controls.DataGrid;
            amVM.BillNo = ((Purchase)(((ItemsControl)sender).Items[dataGrid.SelectedIndex])).RunningBillNo;            
        }
    }
}
