using System.Linq;
using System.Windows.Controls;
using RetailManagementSystem.ViewModel.Purchases;
using System;
using RetailManagementSystem.Model;

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

                custComboBoxCol.ItemsSource = _returnPurchaseViewModel.ProductsPriceList;
                custComboBoxCol.FilterPropertyName = "ProductName";
                _returnPurchaseViewModel.notifierCollectionChangedEvent += () =>
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        custComboBoxCol.ItemsSource = _returnPurchaseViewModel.ProductsPriceList;
                        //_returnPurchaseViewModel.SetProductId();
                    }));
                };
            };
        }       

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var productPrice = selectedItem as ProductPrice;

            _returnPurchaseViewModel.SetProductDetails(productPrice, ReturnPurchaseDataGrid.SelectedIndex);

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
