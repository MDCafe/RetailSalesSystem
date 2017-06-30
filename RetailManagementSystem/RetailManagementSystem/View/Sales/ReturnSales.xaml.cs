using System;
using System.Linq;
using System.Windows.Controls;
using RetailManagementSystem.ViewModel.Extensions;
using RetailManagementSystem.ViewModel.Sales;
using System.Collections;
using System.Collections.Generic;

namespace RetailManagementSystem.View.Sales
{
    public partial class ReturnSales : UserControl
    {
        ReturnSalesViewModel _returnSalesViewModel;
       
        public ReturnSales()
        {
            InitializeComponent();

            DataContextChanged += (sender, eventArgs) =>
            {
                _returnSalesViewModel = this.DataContext as ReturnSalesViewModel;
                custComboBoxCol.ItemsSource = _returnSalesViewModel.ProductsList;
                custComboBoxCol.FilterPropertyName = "Name";
            };                      
        }        

        

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var product = selectedItem as Product;
            _returnSalesViewModel.SetProductDetails(product, ReturnSalesDataGrid.SelectedIndex);
            custComboBoxCol.comboBox.ItemsSource = _returnSalesViewModel.ProductsList;
            custComboBoxCol.comboBox.SelectedIndex = -1;
            custComboBoxCol.ClearSelection();

        }

        private void DataGrid_LoadingRow(object sender, Microsoft.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }

}
