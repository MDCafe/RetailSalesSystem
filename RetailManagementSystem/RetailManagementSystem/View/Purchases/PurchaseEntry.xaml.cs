using System;
using System.Linq;
using System.Windows.Controls;
using RetailManagementSystem.ViewModel.Extensions;
using RetailManagementSystem.ViewModel.Sales;
using System.Windows;
using RetailManagementSystem.ViewModel.Purchases;
using RetailManagementSystem.Model;

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
                //_salesViewModel.Extensions = SalesExtn.DataContext as IExtensions;
            };
        }      

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var productPrice = selectedItem as ProductPrice;
            _purchaseEntryViewModel.SetProductDetails(productPrice,PurchaseDataGrid.SelectedIndex);
        }
    }   
}
