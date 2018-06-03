﻿using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System;
using RetailManagementSystem.ViewModel.Extensions;
using RetailManagementSystem.ViewModel.Sales;
using RetailManagementSystem.Model;
using System.Windows.Media;

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
                }else
                {
                    SalesDataGrid.AddHandler(CommandManager.PreviewExecutedEvent,
                    (ExecutedRoutedEventHandler)((cmdSender, args) =>
                    {
                        if (args.Command == Microsoft.Windows.Controls.DataGrid.BeginEditCommand)
                        {
                            var dataGrid = (Microsoft.Windows.Controls.DataGrid)cmdSender;
                            if (dataGrid.CurrentCell.Column.GetType() != typeof(BHCustCtrl.CustDataGridComboBoxColumn)) return;
                            DependencyObject focusScope = FocusManager.GetFocusScope(dataGrid);
                            FrameworkElement focusedElement = (FrameworkElement)FocusManager.GetFocusedElement(focusScope);
                            var saleDetailExtn = focusedElement.DataContext as SaleDetailExtn;
                            if (saleDetailExtn == null) return;
                            var model = (SaleDetailExtn)focusedElement.DataContext;
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
                _salesViewModel = this.DataContext as SalesEntryViewModel;
                custComboBoxCol.ItemsSource = _salesViewModel.ProductsPriceList;
                custComboBoxCol.FilterPropertyName = "ProductName";
                _salesViewModel.Extensions = SalesExtn.DataContext as IExtensions;

                _salesViewModel.notifierCollectionChangedEvent += () =>
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        custComboBoxCol.ItemsSource = _salesViewModel.ProductsPriceList;
                        _salesViewModel.SetProductId();
                    }));
                };
            };

            

            //SalesDataGrid.SelectedCellsChanged += (g, ev) =>
            // {
            //     if (ev.AddedCells[0].Column.Header.ToString() != "Products") return; 
            //     var grid = g as DataGrid;
            //     grid.BeginEdit();
            // };

            SalesDataGrid.PreviewKeyUp += (s, e) =>
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
                    //custComboBoxCol_ComboBoxSelectedEvent(null);
                    e.Handled = true;
                    //}
                }
                else if(isLetter || isNumber)
                {
                    if (!custComboBoxCol.comboBox.IsDropDownOpen && e.Key != Key.Tab)
                    {
                        var grid = s as Microsoft.Windows.Controls.DataGrid;
                        grid.BeginEdit();
                        //custComboBoxCol.comboBox.IsDropDownOpen = true;
                        //custComboBoxCol.comboBox.Text = e.Key.ToString();
                        TextCompositionManager.StartComposition(new TextComposition(InputManager.Current, custComboBoxCol.comboBox, e.Key.ToString()));
                        custComboBoxCol.comboBox.Text = e.Key.ToString();
                        //custComboBoxCol.comboBox.ClearSelection();
                    }
                }
            };
        }        

        private void CustComboBoxColComboBoxSelectedEvent(object selectedItem)
        {
            var barcodeNo = custComboBoxCol._cboTextBox.Text;          
            var productPrice = selectedItem as ProductPrice;
         
            _salesViewModel.SetProductDetails(barcodeNo, productPrice,SalesDataGrid.SelectedIndex);
            //custComboBoxCol.ComboBoxSelectedEvent -= custComboBoxCol_ComboBoxSelectedEvent;

            custComboBoxCol.comboBox.ItemsSource = _salesViewModel.ProductsPriceList;
            //custComboBoxCol.comboBox.SelectedIndex = -1;
            //custComboBoxCol.ClearSelection();
            //custComboBoxCol.ComboBoxSelectedEvent += custComboBoxCol_ComboBoxSelectedEvent;
            //var rowHeader = SalesDataGrid.sele [SalesDataGrid.SelectedIndex]
            //SalesDataGrid.SelectedIndex

            //custComboBoxCol._cboTextBox.Text = "";
        }

        void Cell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is DataGridCell)
            {
                DataGridCell cell = (sender as DataGridCell);
                Control elem = FindChild<Control>(cell, null);
                elem.Focus();
            }
        }

        void Cell_GotFocus(object sender, RoutedEventArgs e)
        {
            DataGridCell cell = (sender as DataGridCell);
            cell.IsEditing = true;
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
        where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}
