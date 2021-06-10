using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Stocks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RetailManagementSystem.View.Stock
{
    /// <summary>
    /// Interaction logic for StockAdjustment.xaml
    /// </summary>
    public partial class StockAdjustment : UserControl
    {
        StockAdjustmentViewModel stockAdjViewModel;
        public StockAdjustment()
        {
            InitializeComponent();

            DataContextChanged += (sender, eventArgs) =>
            {
                stockAdjViewModel = this.DataContext as StockAdjustmentViewModel;
                custComboBoxCol.ItemsSource = stockAdjViewModel.ProductsPriceList;
                custComboBoxCol.FilterPropertyName = "ProductName";
            };
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

        private void CustComboBoxColComboBoxSelectedEvent(object selectedItem)
        {
            var productPrice = selectedItem as StockAdjustProductPrice;

            //_log.Debug("Before SetProductDetails:" + productPrice);
            stockAdjViewModel.SetProductDetails(productPrice, StockAdjustDataGrid.SelectedIndex);
            ////custComboBoxCol.ComboBoxSelectedEvent -= custComboBoxCol_ComboBoxSelectedEvent;
            //_log.Debug("After SetProductDetails:" + SalesDataGrid.SelectedIndex);

            //custComboBoxCol.comboBox.ItemsSource = stockAdjViewModel.ProductsPriceList;
            ////custComboBoxCol.comboBox.SelectedIndex = -1;

            ////custComboBoxCol.ComboBoxSelectedEvent += custComboBoxCol_ComboBoxSelectedEvent;
            ////var rowHeader = SalesDataGrid.sele [SalesDataGrid.SelectedIndex]
            ////SalesDataGrid.SelectedIndex

            custComboBoxCol._cboTextBox.Text = "";
            custComboBoxCol.ClearSelection();
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
                if (child as T == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    // If the child's name is set for search
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
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
