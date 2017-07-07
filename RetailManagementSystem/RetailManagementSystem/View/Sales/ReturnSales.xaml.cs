using System;
using System.Linq;
using System.Windows.Controls;
using RetailManagementSystem.ViewModel.Extensions;
using RetailManagementSystem.ViewModel.Sales;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using RetailManagementSystem.Model;

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
                custComboBoxCol.ItemsSource = _returnSalesViewModel.ProductsList;
                custComboBoxCol.FilterPropertyName = "Name";

                //custComboBoxColReturnPrice.ItemsSource = _returnSalesViewModel.ReturnPriceList;
                
                _returnSalesViewModel.MakeReadonlyEvent += (s) =>
                {
                    ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Products").IsReadOnly = s;
                    ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Select").IsReadOnly = !s;
                    ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "ReturnPriceList").IsReadOnly = !s; 
                    //ReturnSalesDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Return Quantity").IsReadOnly = s;
                    if (!s)
                    {
                        ReturnSalesDataGrid.LoadingRow += DataGridLoadingRow;
                    }
                    else
                        ReturnSalesDataGrid.LoadingRow -= DataGridLoadingRow;
                };
            };



            ReturnSalesDataGrid.AddHandler(CommandManager.PreviewExecutedEvent,
            (ExecutedRoutedEventHandler)((sender, args) =>
            {
                
                //if (args.Command == DataGrid.routed)
                //{                    
                DependencyObject focusScope = FocusManager.GetFocusScope(ReturnSalesDataGrid);
                FrameworkElement focusedElement = (FrameworkElement)FocusManager.GetFocusedElement(focusScope);
                if (focusedElement.GetType() != typeof(Microsoft.Windows.Controls.DataGridCell)) return;
                if (((Microsoft.Windows.Controls.DataGridCell)focusedElement).Column.Header.ToString() == "Return Price")
                {
                    var model = focusedElement.DataContext as ReturnSaleDetailExtn;
                    if (model == null) return;
                    //custComboBoxColReturnPrice.ItemsSource = _returnSalesViewModel.GetProductPriceDetails(model.ProductId);
                    args.Handled = false;
                }
                //if (model.MyIsReadOnly)
                //{
                //    args.Handled = true;
                //}
                //}
            }));
        }       

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var product = selectedItem as Product;           
            //custComboBoxCol.comboBox.ItemsSource = _returnSalesViewModel.ProductsList;
            custComboBoxCol.comboBox.SelectedIndex = -1;
            custComboBoxCol.ClearSelection();
            //custComboBoxColReturnPrice.ItemsSource = _returnSalesViewModel.GetProductPriceDetails(product.Id);

        }

        private void DataGridLoadingRow(object sender, Microsoft.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }

}
