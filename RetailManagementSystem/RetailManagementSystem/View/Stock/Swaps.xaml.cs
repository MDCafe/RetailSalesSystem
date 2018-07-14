using System.Windows.Controls;
using RetailManagementSystem.Model;
using System;
using RetailManagementSystem.ViewModel.Stocks;

namespace RetailManagementSystem.View.Stock
{
    public partial class Swaps : UserControl
    {
        SwapsViewModel _swapsViewModel;
       
        public Swaps()
        {
            InitializeComponent();
            
            DataContextChanged += (sender, eventArgs) =>
            {
                _swapsViewModel = this.DataContext as SwapsViewModel;
                custComboBoxCol.ItemsSource = _swapsViewModel.ProductsPriceList;
                custComboBoxCol.FilterPropertyName = "ProductName";

                _swapsViewModel.NotifierCollectionChangedEvent += () =>
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        custComboBoxCol.ItemsSource = _swapsViewModel.ProductsPriceList;
                    }));
                };
            };

        }        

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var productPrice = selectedItem as ProductPrice;

            _swapsViewModel.SetProductDetails(productPrice, SalesDataGrid.SelectedIndex);
            custComboBoxCol.ComboBoxSelectedEvent -= custComboBoxCol_ComboBoxSelectedEvent;

            custComboBoxCol.comboBox.ItemsSource = _swapsViewModel.ProductsPriceList;
            custComboBoxCol.comboBox.SelectedIndex = -1;
            custComboBoxCol.ClearSelection();
            custComboBoxCol.ComboBoxSelectedEvent += custComboBoxCol_ComboBoxSelectedEvent;

        }

        private void DataGrid_LoadingRow(object sender, Microsoft.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
