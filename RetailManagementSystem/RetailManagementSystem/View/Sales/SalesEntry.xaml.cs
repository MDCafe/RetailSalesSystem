using System.Windows.Controls;
using RetailManagementSystem.ViewModel.Extensions;
using RetailManagementSystem.ViewModel.Sales;
using RetailManagementSystem.Model;
using System.Windows.Input;
using System.Windows;
using System;

namespace RetailManagementSystem.View.Sales
{
    public partial class SalesEntry : UserControl
    {
        SalesEntryViewModel _salesViewModel;
       
        public SalesEntry()
        {
            InitializeComponent();
            RoutedEventHandler handler = null;
            handler = (object sender, RoutedEventArgs e) =>
            {
                if (_salesViewModel.IsEditMode == false)
                {
                    CboCustomers.SelectedValue = _salesViewModel.DefaultCustomer.Id;
                    CboCustomers.SelectedItem = _salesViewModel.DefaultCustomer;
                    //CboCustomers.Text = _salesViewModel.SelectedCustomerText;
                }
                Loaded -= handler;
            };
            Loaded += handler;

            DataContextChanged += (sender, eventArgs) =>
            {
                _salesViewModel = this.DataContext as SalesEntryViewModel;
                custComboBoxCol.ItemsSource = _salesViewModel.ProductsPriceList;
                custComboBoxCol.FilterPropertyName = "ProductName";
                _salesViewModel.Extensions = SalesExtn.DataContext as IExtensions;

                _salesViewModel.notifierCollectionChangedEvent += () =>
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        custComboBoxCol.ItemsSource = _salesViewModel.ProductsPriceList;
                    }));
                };
            };

            SalesDataGrid.PreviewKeyUp += (s, e) =>
            {
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
            custComboBoxCol.ComboBoxSelectedEvent -= custComboBoxCol_ComboBoxSelectedEvent;

            custComboBoxCol.comboBox.ItemsSource = _salesViewModel.ProductsPriceList;
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
