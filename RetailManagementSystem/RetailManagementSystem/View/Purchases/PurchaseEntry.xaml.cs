using System;
using System.Linq;
using System.Windows.Controls;
using RetailManagementSystem.ViewModel.Extensions;
using RetailManagementSystem.ViewModel.Sales;
using System.Windows;
using RetailManagementSystem.ViewModel.Purchases;

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

            //DataContextChanged += (sender, eventArgs) =>
            ////{
            ////    _salesViewModel = this.DataContext as SalesEntryViewModel;
            ////    custComboBoxCol.ItemsSource = _salesViewModel.ProductsPriceList;
            ////    _salesViewModel.Extensions = SalesExtn.DataContext as IExtensions;
            //};


        }

        private void ComboBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            //ComboBox cmb = (ComboBox)sender;

            //cmb.IsDropDownOpen = true;

            //if (!string.IsNullOrEmpty(cmb.Text))
            //{
            //    string fullText = cmb.Text + e.Text;
            //    cmb.ItemsSource = _salesViewModel.ProductsPriceList.Where(s => s.ProductName.StartsWith(fullText, StringComparison.InvariantCultureIgnoreCase)).ToList();
            //    return;
            //}
            ////else if (!string.IsNullOrEmpty(e.Text))
            //if (!string.IsNullOrEmpty(e.Text))
            //{
            //    cmb.ItemsSource = _salesViewModel.ProductsPriceList.Where(s => s.ProductName.StartsWith(cmb.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
            //}
            //else
            //{
            //    cmb.ItemsSource = _salesViewModel.ProductsPriceList;
            //}
        }

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            //var productPrice = selectedItem as ProductPrice;
            //_salesViewModel.SetProductDetails(productPrice);
        }
    }   
}
