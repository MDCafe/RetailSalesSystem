using System.Windows.Controls;
using RetailManagementSystem.ViewModel.Purchases;
using RetailManagementSystem.Model;
using System;

namespace RetailManagementSystem.View.Purchases
{
    /// <summary>
    /// Interaction logic for SalesEntry.xaml
    /// </summary>
    public partial class PurchaseEntry : UserControl
    {
        PurchaseEntryViewModel _purchaseEntryViewModel;


        public PurchaseEntry()
        {
            InitializeComponent();

            DataContextChanged += (sender, eventArgs) =>
            {
                _purchaseEntryViewModel = DataContext as PurchaseEntryViewModel;
                custComboBoxCol.ItemsSource = _purchaseEntryViewModel.ProductsPriceList;
                custComboBoxCol.FilterPropertyName = "ProductName";
                _purchaseEntryViewModel.notifierCollectionChangedEvent +=() =>
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        custComboBoxCol.ItemsSource = _purchaseEntryViewModel.ProductsPriceList;
                    }));
                 };

                //_salesViewModel.Extensions = SalesExtn.DataContext as IExtensions;
            };
        }      

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var productPrice = selectedItem as ProductPrice;

            _purchaseEntryViewModel.SetProductDetails(productPrice,PurchaseDataGrid.SelectedIndex);

            custComboBoxCol.comboBox.ItemsSource = _purchaseEntryViewModel.ProductsPriceList;
            custComboBoxCol.comboBox.SelectedIndex = -1;
            custComboBoxCol.ClearSelection();
        }

        private void DataGrid_LoadingRow(object sender, Microsoft.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }   
}
