using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
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

            RoutedEventHandler handler = null;
            handler = (object sender, RoutedEventArgs e) =>
            {
                if (_purchaseEntryViewModel.IsEditMode)
                {
                    PurchaseDataGrid.AddHandler(CommandManager.PreviewExecutedEvent,
                    (ExecutedRoutedEventHandler)((cmdSender, args) =>
                    {
                        if (args.Command == Microsoft.Windows.Controls.DataGrid.BeginEditCommand)
                        {
                            var dataGrid = (Microsoft.Windows.Controls.DataGrid)cmdSender;
                            if (dataGrid.CurrentCell.Column.GetType() != typeof(BHCustCtrl.CustDataGridComboBoxColumn)) return;
                            DependencyObject focusScope = FocusManager.GetFocusScope(dataGrid);
                            FrameworkElement focusedElement = (FrameworkElement)FocusManager.GetFocusedElement(focusScope);
                            var purchaseDetailExtn = focusedElement.DataContext as PurchaseDetailExtn;
                            if (purchaseDetailExtn == null) return;
                            var model = (PurchaseDetailExtn)focusedElement.DataContext;
                            if (model.PropertyReadOnly)
                            {
                                args.Handled = true;
                            }
                        }
                    }));
                }
                Loaded -= handler;
            };
            Loaded += handler;

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
                        _purchaseEntryViewModel.SetProductId();
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
