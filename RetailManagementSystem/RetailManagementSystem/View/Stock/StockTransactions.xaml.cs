using System.Windows.Controls;
using RetailManagementSystem.ViewModel.Extensions;
using RetailManagementSystem.ViewModel.Sales;
using RetailManagementSystem.Model;
using System.Windows.Input;
using System.Windows;
using System;
using RetailManagementSystem.ViewModel.Stocks;

namespace RetailManagementSystem.View.Stock
{
    public partial class StockTransactions : UserControl
    {
        StockTransactionsViewModel _stkTransViewModel;
       
        public StockTransactions()
        {
            InitializeComponent();
            
            DataContextChanged += (sender, eventArgs) =>
            {
                _stkTransViewModel = this.DataContext as StockTransactionsViewModel;
                //custComboBoxCol.ItemsSource = _salesViewModel.ProductsPriceList;
                //custComboBoxCol.FilterPropertyName = "ProductName";
                //_salesViewModel.Extensions = SalesExtn.DataContext as IExtensions;

                //_salesViewModel.notifierCollectionChangedEvent += () =>
                //{
                //    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                //    {
                //        custComboBoxCol.ItemsSource = _salesViewModel.ProductsPriceList;
                //    }));
                //};
            };

        }        

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            //var productPrice = selectedItem as ProductPrice;
         
            //_stkTransViewModel.SetProductDetails(productPrice,SalesDataGrid.SelectedIndex);
            //custComboBoxCol.ComboBoxSelectedEvent -= custComboBoxCol_ComboBoxSelectedEvent;

            //custComboBoxCol.comboBox.ItemsSource = _stkTransViewModel.ProductsPriceList;
            //custComboBoxCol.comboBox.SelectedIndex = -1;
            //custComboBoxCol.ClearSelection();
            //custComboBoxCol.ComboBoxSelectedEvent += custComboBoxCol_ComboBoxSelectedEvent;

        }

        private void DataGrid_LoadingRow(object sender, Microsoft.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
