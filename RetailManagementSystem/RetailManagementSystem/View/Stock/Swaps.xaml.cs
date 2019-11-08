using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Stocks;
using System.Windows.Controls;

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

                //_swapsViewModel.NotifierCollectionChangedEvent += () =>
                //{
                //    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                //    {
                //        custComboBoxCol.ItemsSource = _swapsViewModel.ProductsPriceList;
                //    }));
                //};
            };

        }        

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var productPrice = selectedItem as StockAdjustProductPrice;

            _swapsViewModel.SetProductDetails(productPrice, SalesDataGrid.SelectedIndex);
            custComboBoxCol.ComboBoxSelectedEvent -= custComboBoxCol_ComboBoxSelectedEvent;

            custComboBoxCol.comboBox.ItemsSource = _swapsViewModel.ProductsPriceList;
            custComboBoxCol.comboBox.SelectedIndex = -1;
            custComboBoxCol.ClearSelection();
            custComboBoxCol.ComboBoxSelectedEvent += custComboBoxCol_ComboBoxSelectedEvent;

        }        
    }
}
