using RetailManagementSystem.ViewModel.Sales;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RetailManagementSystem.View.Sales
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class POSSalesEntry : Window
    {
        readonly POSSalesEntryViewModel posSalesEntryViewModel = new POSSalesEntryViewModel(false);

        public POSSalesEntry()
        {
            InitializeComponent();


            Loaded += handler;

            DataContext = posSalesEntryViewModel;

            void handler(object sender, RoutedEventArgs e)
            {
                CboCustomers.SelectedValue = posSalesEntryViewModel.DefaultCustomer.Id;
                CboCustomers.SelectedItem = posSalesEntryViewModel.DefaultCustomer;
                posSalesEntryViewModel.SelectedCustomer = posSalesEntryViewModel.DefaultCustomer;
                //posSalesEntryViewModel.salesDataGrid = this.POSSalesGrid;
                POSSalesGrid.CurrentCell = new DataGridCellInfo(POSSalesGrid.Items[0], POSSalesGrid.Columns[0]);
                //POSSalesGrid.BeginEdit();
                posSalesEntryViewModel.ViewWindow = this;
            }

            posSalesEntryViewModel.SetFocusOnClearEvent += () =>
              {
                  POSSalesGrid.SelectedItem = posSalesEntryViewModel.SaleDetailList[0];
                  POSSalesGrid.ScrollIntoView(POSSalesGrid.Items[0]);
                  DataGridRow dgrow = (DataGridRow)POSSalesGrid.ItemContainerGenerator.ContainerFromItem(POSSalesGrid.Items[0]);
                  dgrow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
              };
        }
    }
}
