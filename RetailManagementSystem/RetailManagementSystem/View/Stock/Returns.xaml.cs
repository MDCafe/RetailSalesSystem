using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Purchases;
using System;
using System.Windows.Controls;

namespace RetailManagementSystem.View.Stock
{
    public partial class Returns : UserControl
    {
        ReturnsViewModel _returnsViewModel;

        public Returns()
        {
            InitializeComponent();

            DataContextChanged += (sender, eventArgs) =>
            {
                _returnsViewModel = DataContext as ReturnsViewModel;

                ReturnResonCbo.ItemsSource = _returnsViewModel.ReturnReasons;

                custComboBoxCol.ItemsSource = _returnsViewModel.ProductsPriceList;
                custComboBoxCol.FilterPropertyName = "ProductName";
                _returnsViewModel.NotifierCollectionChangedEvent += () =>
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        custComboBoxCol.ItemsSource = _returnsViewModel.ProductsPriceList;
                        //_returnPurchaseViewModel.SetProductId();
                    }));
                };
            };
        }

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var productPrice = selectedItem as ProductPrice;

            _returnsViewModel.SetProductDetails(productPrice, ReturnPurchaseDataGrid.SelectedIndex);

            //custComboBoxCol.comboBox.ItemsSource = _returnPurchaseViewModel.ProductsList;
            //custComboBoxCol.comboBox.SelectedIndex = -1;
            //custComboBoxCol.ClearSelection();
            //custComboBoxCol.ItemsSource = _returnPurchaseViewModel.GetProductPriceDetails(product.Id);

        }

        //private void cmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var cmb = sender as ComboBox;
        //    if (cmb.SelectedValue == null) return;

        //    var productId = int.Parse(cmb.SelectedValue.ToString());
        //    _returnPurchaseViewModel.SetProductPriceDetails(productId, ReturnPurchaseDataGrid.SelectedIndex);
        //}

        //private void cmbPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var cmb = sender as ComboBox;
        //    if (cmb.SelectedValue == null) return;
        //    var priceId = int.Parse(cmb.SelectedValue.ToString());
        //    _returnPurchaseViewModel.SetPriceDetails(priceId,ReturnPurchaseDataGrid.SelectedIndex);
        //}
    }
}
