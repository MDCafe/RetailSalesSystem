using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Purchases;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            void handler(object sender, RoutedEventArgs e)
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
                            if (!(focusedElement.DataContext is PurchaseDetailExtn purchaseDetailExtn)) return;
                            var model = (PurchaseDetailExtn)focusedElement.DataContext;
                            if (model.PropertyReadOnly)
                            {
                                args.Handled = true;
                            }
                        }
                    }));
                }
                Loaded -= handler;
            }

            Loaded += handler;

            DataContextChanged += (sender, eventArgs) =>
            {
                _purchaseEntryViewModel = DataContext as PurchaseEntryViewModel;
                custComboBoxCol.ItemsSource = _purchaseEntryViewModel.ProductsPriceList;
                custComboBoxCol.FilterPropertyName = "ProductName";
                _purchaseEntryViewModel.NotifierCollectionChangedEvent += () =>
                 {
                     App.Current.Dispatcher.BeginInvoke((Action)(() =>
                     {
                         custComboBoxCol.ItemsSource = _purchaseEntryViewModel.ProductsPriceList;
                         _purchaseEntryViewModel.SetProductId();
                     }));
                 };

                //_salesViewModel.Extensions = SalesExtn.DataContext as IExtensions;
            };


            PurchaseDataGrid.PreviewKeyUp += (s, e) =>
            {

                var isNumber = e.Key >= Key.D0 && e.Key <= Key.D9;
                var isLetter = e.Key >= Key.A && e.Key <= Key.Z;

                if ((e.Key == Key.Enter) || (e.Key == Key.Return))
                {
                    var grid = s as Microsoft.Windows.Controls.DataGrid;

                    //if (grid.CurrentColumn.Header.ToString().Equals("Barcode", StringComparison.OrdinalIgnoreCase))
                    //{
                    //if (grid.SelectionUnit == DataGridSelectionUnit.Cell || grid.SelectionUnit == DataGridSelectionUnit.CellOrRowHeader)
                    //{
                    //var focusedElement = Keyboard.FocusedElement as UIElement;
                    //focusedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                    //grid.SelectedIndex = grid.SelectedIndex + 1;
                    //grid.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                    //}

                    // get the selected row
                    //var selectedRow = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                    //// selectedRow can be null due to virtualization
                    //if (selectedRow != null)
                    //{
                    // there should always be a selected cell
                    if (grid.SelectedCells.Count != 0)
                    {
                        // get the cell info
                        Microsoft.Windows.Controls.DataGridCellInfo currentCell = grid.SelectedCells[0];

                        // get the display index of the cell's column + 1 (for next column)
                        int columnDisplayIndex = currentCell.Column.DisplayIndex;

                        // if display index is valid
                        if (columnDisplayIndex < grid.Columns.Count)
                        {
                            // get the DataGridColumn instance from the display index
                            Microsoft.Windows.Controls.DataGridColumn nextColumn = grid.ColumnFromDisplayIndex(0);

                            // now telling the grid, that we handled the key down event
                            e.Handled = true;

                            // setting the current cell (selected, focused)
                            grid.CurrentCell = new Microsoft.Windows.Controls.DataGridCellInfo(grid.SelectedItem, nextColumn);

                            // tell the grid to initialize edit mode for the current cell
                            //grid.BeginEdit();
                        }
                    }
                    //grid.BeginEdit();
                    e.Handled = true;
                    //}
                }
                else if (isLetter || isNumber)
                {
                    if (!custComboBoxCol.comboBox.IsDropDownOpen && e.Key != Key.Tab)
                    {
                        var grid = s as Microsoft.Windows.Controls.DataGrid;
                        grid.BeginEdit();
                        custComboBoxCol.comboBox.IsDropDownOpen = true;
                        //custComboBoxCol.comboBox.Text = e.Key.ToString();
                        TextCompositionManager.StartComposition(new TextComposition(InputManager.Current, custComboBoxCol.comboBox, e.Key.ToString()));
                    }
                }

            };
        }

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var productPrice = selectedItem as ProductPrice;

            _purchaseEntryViewModel.SetProductDetails(productPrice, PurchaseDataGrid.SelectedIndex);

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
