using System.Linq;
using System.Windows.Controls;
using RetailManagementSystem.ViewModel.Purchases;

namespace RetailManagementSystem.View.Purchases
{
    public partial class ReturnPurchase : UserControl
    {
        ReturnPurchaseViewModel _returnPurchaseViewModel;
       
        public ReturnPurchase()
        {            
            InitializeComponent();

            DataContextChanged += (sender, eventArgs) =>
            {
                _returnPurchaseViewModel = DataContext as ReturnPurchaseViewModel;

                ReturnResonCbo.ItemsSource = _returnPurchaseViewModel.ReturnReasons;

                //custComboBoxCol.ItemsSource = _returnSalesViewModel.ProductsList;
                //custComboBoxCol.FilterPropertyName = "Name";

                //custComboBoxColReturnPrice.ItemsSource = _returnSalesViewModel.ReturnPriceList;

                _returnPurchaseViewModel.MakeReadonlyEvent += (s) =>
                {
                    var visibility = System.Windows.Visibility.Hidden;

                    this.Dispatcher.Invoke( () =>
                    {
                        if (s)
                        {
                            visibility = System.Windows.Visibility.Visible;
                            ReturnPurchaseDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Return Product Price").Visibility = System.Windows.Visibility.Hidden;
                            ReturnPurchaseDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Return Price").Visibility = System.Windows.Visibility.Hidden;
                            ReturnPurchaseDataGrid.CanUserAddRows = false;
                        }
                        else
                        {
                            ReturnPurchaseDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Return Product Price").Visibility = System.Windows.Visibility.Visible;
                            ReturnPurchaseDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Return Price").Visibility = System.Windows.Visibility.Visible;
                            ReturnPurchaseDataGrid.CanUserAddRows = true;
                        }
                        ReturnPurchaseDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Select").Visibility = visibility;
                        ReturnPurchaseDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Purchased Quantity").Visibility = visibility;
                        ReturnPurchaseDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Purchased Price").Visibility = visibility;
                        ReturnPurchaseDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Purchased Price").Visibility = visibility;
                        ReturnPurchaseDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Selling Price").Visibility = visibility;
                        ReturnPurchaseDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Discount (%)").Visibility = visibility;
                        ReturnPurchaseDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Discount Amount").Visibility = visibility;

                        if (!s)
                        {
                            ReturnPurchaseDataGrid.LoadingRow += DataGridLoadingRow;
                        }
                        else
                            ReturnPurchaseDataGrid.LoadingRow -= DataGridLoadingRow;
                    });
                };
            };
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
            _returnPurchaseViewModel.SetProductPriceDetails(productId, ReturnPurchaseDataGrid.SelectedIndex);
        }

        private void cmbPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb.SelectedValue == null) return;
            var priceId = int.Parse(cmb.SelectedValue.ToString());
            _returnPurchaseViewModel.SetPriceDetails(priceId,ReturnPurchaseDataGrid.SelectedIndex);
        }
    }

}
