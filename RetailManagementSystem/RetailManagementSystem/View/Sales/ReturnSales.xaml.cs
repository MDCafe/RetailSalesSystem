using System.Linq;
using System.Windows.Controls;
using RetailManagementSystem.ViewModel.Sales;

namespace RetailManagementSystem.View.Sales
{
    public partial class ReturnSales : UserControl
    {
        ReturnSalesViewModel _returnSalesViewModel;
       
        public ReturnSales()
        {            
            InitializeComponent();
           // this.DataContext = new ReturnSalesViewModel(false);

            DataContextChanged += (sender, eventArgs) =>
            {
                _returnSalesViewModel = this.DataContext as ReturnSalesViewModel;
                //custComboBoxCol.ItemsSource = _returnSalesViewModel.ProductsList;
                //custComboBoxCol.FilterPropertyName = "Name";

                //custComboBoxColReturnPrice.ItemsSource = _returnSalesViewModel.ReturnPriceList;
                
                _returnSalesViewModel.MakeReadonlyEvent += (s) =>
                {
                    var visibility = System.Windows.Visibility.Hidden;

                    this.Dispatcher.Invoke( () =>
                    {
                        if (s)
                        {
                            visibility = System.Windows.Visibility.Visible;
                            ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Return Product Price").Visibility = System.Windows.Visibility.Hidden;
                            ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Return Price").Visibility = System.Windows.Visibility.Hidden;
                            ReturnSalesDataGrid.CanUserAddRows = false;
                        }
                        else
                        {
                            ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Return Product Price").Visibility = System.Windows.Visibility.Visible;
                            ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Return Price").Visibility = System.Windows.Visibility.Visible;
                            ReturnSalesDataGrid.CanUserAddRows = true;
                        }
                        ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Select").Visibility = visibility;
                        ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Purchased Quantity").Visibility = visibility;
                        //ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Purchased Price").Visibility = visibility;
                        ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Cost Price").Visibility = visibility;
                        ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Selling Price").Visibility = visibility;
                        ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Discount (%)").Visibility = visibility;
                        ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Discount Amount").Visibility = visibility;

                        if (!s)
                        {
                            ReturnSalesDataGrid.LoadingRow += DataGridLoadingRow;
                        }
                        else
                            ReturnSalesDataGrid.LoadingRow -= DataGridLoadingRow;
                    });
                };
            };



            //ReturnSalesDataGrid.AddHandler(CommandManager.PreviewExecutedEvent,
            //(ExecutedRoutedEventHandler)((sender, args) =>
            //{
                
            //    //if (args.Command == DataGrid.routed)
            //    //{                    
            //    DependencyObject focusScope = FocusManager.GetFocusScope(ReturnSalesDataGrid);
            //    FrameworkElement focusedElement = (FrameworkElement)FocusManager.GetFocusedElement(focusScope);
            //    if (focusedElement.GetType() != typeof(Microsoft.Windows.Controls.DataGridCell)) return;
            //    if (((Microsoft.Windows.Controls.DataGridCell)focusedElement).Column.Header.ToString() == "Return Price")
            //    {
            //        var model = focusedElement.DataContext as ReturnSaleDetailExtn;
            //        if (model == null) return;
            //        //custComboBoxColReturnPrice.ItemsSource = _returnSalesViewModel.GetProductPriceDetails(model.ProductId);
            //        args.Handled = false;
            //    }
            //    //if (model.MyIsReadOnly)
            //    //{
            //    //    args.Handled = true;
            //    //}
            //    //}
            //}));
        }       

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var product = selectedItem as Product;           
            //custComboBoxCol.comboBox.ItemsSource = _returnSalesViewModel.ProductsList;
            //custComboBoxCol.comboBox.SelectedIndex = -1;
            //custComboBoxCol.ClearSelection();
            //custComboBoxColReturnPrice.ItemsSource = _returnSalesViewModel.GetProductPriceDetails(product.Id);

        }

        private void DataGridLoadingRow(object sender, Microsoft.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void cmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb.SelectedValue == null) return;

            var productId = int.Parse(cmb.SelectedValue.ToString());
            _returnSalesViewModel.SetProductPriceDetails(productId, ReturnSalesDataGrid.SelectedIndex);
        }

        private void cmbPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb.SelectedValue == null) return;
            var priceId = int.Parse(cmb.SelectedValue.ToString());
            _returnSalesViewModel.SetPriceDetails(priceId,ReturnSalesDataGrid.SelectedIndex);
        }
    }

}
