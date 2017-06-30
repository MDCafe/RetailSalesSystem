using System;
using System.Linq;
using System.Windows.Controls;
using RetailManagementSystem.ViewModel.Extensions;
using RetailManagementSystem.ViewModel.Sales;

namespace RetailManagementSystem.View.Sales
{
    public partial class SalesEntry : UserControl
    {
        SalesEntryViewModel _salesViewModel;
       
        public SalesEntry()
        {
            InitializeComponent();

            DataContextChanged += (sender, eventArgs) =>
            {
                _salesViewModel = this.DataContext as SalesEntryViewModel;
                custComboBoxCol.ItemsSource = _salesViewModel.ProductsPriceList;
                custComboBoxCol.FilterPropertyName = "ProductName";
                _salesViewModel.Extensions = SalesExtn.DataContext as IExtensions;
            };

            //custComboBoxCol.comboBox.PreviewTextInput += ComboBox_PreviewTextInput;
            //custComboBoxCol.OnComboLoadedEvent += (txt) =>
            //{
            //    custComboBoxCol._cboTextBox.PreviewKeyUp += (s, e) =>
            //    {
            //        if (e.Key == System.Windows.Input.Key.Back && string.IsNullOrWhiteSpace(custComboBoxCol.comboBox.Text))
            //        {                        
            //            custComboBoxCol.comboBox.ItemsSource = _salesViewModel.ProductsPriceList;
            //        }
            //    };
            //};
        }        

        //private void ComboBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        //{             
        //    ComboBox cmb = (ComboBox)sender;

        //    cmb.IsDropDownOpen = true;

        //    if (!string.IsNullOrEmpty(cmb.Text))
        //    {
        //        string fullText = cmb.Text +  e.Text;
        //        cmb.ItemsSource = _salesViewModel.ProductsPriceList.Where(s => s.ProductName.StartsWith(fullText, StringComparison.InvariantCultureIgnoreCase)).ToList();
        //        return;
        //    }
        //    //else if (!string.IsNullOrEmpty(e.Text))
        //    if (!string.IsNullOrEmpty(e.Text))
        //    {
        //        cmb.ItemsSource = _salesViewModel.ProductsPriceList.Where(s => s.ProductName.StartsWith(cmb.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
        //    }
        //    else
        //    {
        //        cmb.ItemsSource = _salesViewModel.ProductsPriceList;
        //    }        
        //}

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var productPrice = selectedItem as ProductPrice;
            _salesViewModel.SetProductDetails(productPrice,SalesDataGrid.SelectedIndex);
            custComboBoxCol.comboBox.ItemsSource = _salesViewModel.ProductsPriceList;
            custComboBoxCol.comboBox.SelectedIndex = -1;
            custComboBoxCol.ClearSelection();
            
        }

        private void DataGrid_LoadingRow(object sender, Microsoft.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
