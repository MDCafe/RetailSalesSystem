/*  
 * CustDataGridComboBoxColumn implements data grid column combobox with popup DataGrid control.
 * Bahrudin Hrnjica, bhrnjica@hotmail.com
 * First Release Oct, 2009
 */
using log4net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BHCustCtrl
{

    public delegate void OnComboItemSelected(object selectedItem);
    public delegate void OnComboLoaded(TextBox txtBox);

    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BHCustCtrl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BHCustCtrl;assembly=BHCustCtrl"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustDataGridComboBoxColumn/>
    ///
    /// </summary>
    [DefaultProperty("Columns")]
    [ContentProperty("Columns")]
    public class CustDataGridComboBoxColumn : DataGridComboBoxColumn
    {
        public static readonly DependencyProperty CustComboBoxSelectedPathProperty = DependencyProperty.Register("CustComboBoxSelectedPathProperty", typeof(string), typeof(CustDataGridComboBoxColumn));
        public static readonly DependencyProperty CustComboBoxSelectedValueProperty = DependencyProperty.Register("CustComboBoxSelectedValueProperty", typeof(string), typeof(CustDataGridComboBoxColumn));

        static readonly ILog _log = LogManager.GetLogger(typeof(CustComboBox));

        public string CustComboBoxSelectedPath
        {
            get { return (string)GetValue(CustComboBoxSelectedPathProperty); }
            set { SetValue(CustComboBoxSelectedPathProperty, value); }
        }

        public string CustComboBoxSelectedValue
        {
            get { return (string)GetValue(CustComboBoxSelectedValueProperty); }
            set { SetValue(CustComboBoxSelectedValueProperty, value); }
        }


        //Columns of DataGrid
        private ObservableCollection<DataGridTextColumn> columns;
        public TextBox _cboTextBox;
        //Cust Combobox  cell edit
        public CustComboBox comboBox;


        public event OnComboItemSelected ComboBoxSelectedEvent;
        public event OnComboLoaded OnComboLoadedEvent;

        public string FilterPropertyName { get; set; }

        public CustDataGridComboBoxColumn()
        {
            comboBox = new CustComboBox
            {
                IsEditable = true
            };
            //comboBox.PreviewKeyDown += ComboBox_PreviewKeyDown;
            comboBox.PreviewTextInput += ComboBoxPreviewTextInput;
            comboBox.Loaded += ComboBox_Loaded;
        }

        private void _cboTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back && string.IsNullOrEmpty(_cboTextBox.Text.Trim()))
            {
                comboBox.ItemsSource = ItemsSource;
            }
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            _cboTextBox = (comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox);
            if (_cboTextBox != null)
            {
                _cboTextBox.Focus();
                _cboTextBox.SelectionStart = _cboTextBox.Text.Length;

            }
            comboBox.IsTextSearchEnabled = false;
            OnComboLoadedEvent?.Invoke(_cboTextBox);
            _cboTextBox.PreviewKeyUp += _cboTextBox_PreviewKeyUp;
        }

        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //var dropdownOpen = comboBox.IsDropDownOpen;
            comboBox.IsDropDownOpen = true;

        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == CustDataGridComboBoxColumn.ItemsSourceProperty)
            {
                comboBox.ItemsSource = ItemsSource;
            }
            //else if (e.Property == CustDataGridComboBoxColumn.SelectedValuePathProperty)
            //{                
            //    comboBox.SelectedValuePath = SelectedValuePath;                
            //}
            else if (e.Property == CustDataGridComboBoxColumn.CustComboBoxSelectedPathProperty)
            {
                comboBox.SelectedValuePath = CustComboBoxSelectedPath;
            }
            else if (e.Property == CustDataGridComboBoxColumn.DisplayMemberPathProperty)
            {
                comboBox.DisplayMemberPath = DisplayMemberPath;
            }

            base.OnPropertyChanged(e);
        }

        //The property is default and Content property for CustComboBox
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<DataGridTextColumn> Columns
        {
            get
            {
                if (this.columns == null)
                {
                    this.columns = new ObservableCollection<DataGridTextColumn>();
                }
                return this.columns;
            }
        }
        /// <summary>
        ///     Creates the visual tree for text based cells.
        /// </summary>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            if (comboBox.Columns.Count == 0)
            {
                //Add columns to DataGrid columns
                for (int i = 0; i < columns.Count; i++)
                    comboBox.Columns.Add(columns[i]);
            }

            //var comboBox = (ComboBox)base.GenerateEditingElement(cell, dataItem);
            //comboBox.SelectionChanged += ComboBox_SelectionChanged;
            comboBox.IsDropDownOpen = true;

            return comboBox;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            //if (editingEventArgs == null) return null;
            //Microsoft.Windows.Controls.DataGridCell cell = editingEventArgs.Source as Microsoft.Windows.Controls.DataGridCell;
            //if (cell != null)
            //{
            // Changed to support EF POCOs
            PropertyInfo info = editingElement.DataContext.GetType().GetProperty(CustComboBoxSelectedValue, BindingFlags.Public | BindingFlags.Instance);
            if (info == null) return null;
            object obj = info.GetValue(editingElement.DataContext, null);
            comboBox.SelectedValue = obj;
            //}
            return comboBox.SelectedItem;

        }

        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            //((System.Data.DataRowView)editingElement.DataContext).Row[this.SelectedValuePath] = comboBox.SelectedValue;
            //System.Windows.Data.BindingExpression binding = editingElement.GetBindingExpression(ComboBox.SelectedValueProperty);
            //if (binding != null) binding.UpdateSource();
            //editingElement.SetValue(ComboBox.SelectedItemProperty, comboBox.SelectedItem);
            //var propInfo = editingElement.DataContext.GetType().GetProperty(this.SelectedValuePath);
            //propInfo.SetValue(editingElement.Parent, comboBox.SelectedValue, new object[] { });
            //((Microsoft.Windows.Controls.DataGridCell)editingElement.Parent).SetValue(DataGridCell.ContentProperty, comboBox.SelectedValue);

            ////return true;// base.CommitCellEdit(editingElement);
            //editingElement.DataContext = comboBox.SelectedItem;
            //base.CommitCellEdit(editingElement);
            //return true;

            // Dynamically set the item on our POCO (the DataContext).
            //_log.Info("CustComboBoxSelectedValue:" + CustComboBoxSelectedValue);
            //PropertyInfo info = editingElement.DataContext.GetType().GetProperty(CustComboBoxSelectedValue, BindingFlags.Public | BindingFlags.Instance);
            //info.SetValue(editingElement.DataContext, comboBox.SelectedValue, null);
            //var grid =FindMyParentHelper<DataGrid>.FindAncestor(editingElement);


            _log.Info("comboBox.SelectedValue:" + comboBox.SelectedValue);
            _log.Info(comboBox.SelectedItem);

            //grid.CommitEdit();
            ComboBoxSelectedEvent?.Invoke(comboBox.SelectedItem);
            return true;

        }

        //protected override FrameworkElement GenerateEditingElement(Microsoft.Windows.Controls.DataGridCell cell, object dataItem)
        //{

        //}        

        public static class FindMyParentHelper<T> where T : DependencyObject
        {
            public static T FindAncestor(DependencyObject dependencyObject)
            {
                while (true)
                {
                    if (dependencyObject == null) return null;

                    var parent = VisualTreeHelper.GetParent(dependencyObject) ??
                                 LogicalTreeHelper.GetParent(dependencyObject);

                    if (parent is T parentT) return parentT;

                    dependencyObject = parent;
                }
            }
        }

        public void ClearSelection()
        {
            comboBox.ClearSelection();
        }

        private void Filter(string searchStr, string searchPropertName)
        {
            //var propsToCheck = typeof(T).GetProperties().Where(a => a.Name == searchPropertName);

            //var filter = propsToCheck.Aggregate(string.Empty, (s, p) => (s == string.Empty ? string.Empty : string.Format("{0} OR ", s)) + string.Format("{0} == @0", p.Name));
            if (string.IsNullOrEmpty(searchStr))
            {
                comboBox.ItemsSource = ItemsSource;
                return;
            }

            List<object> filteredList = new List<object>();

            foreach (var item in ItemsSource)
            {
                if (item.GetType().GetProperty(searchPropertName).GetValue(item, null).ToString().ToUpper().Contains(searchStr.ToUpper()))
                {
                    filteredList.Add(item);
                }
            }

            comboBox.ItemsSource = filteredList;// result.Where(s => s.GetType().GetProperty(searchPropertName).GetValue(s, null).ToString().StartsWith(searchStr, StringComparison.InvariantCultureIgnoreCase)).ToList();            
            //comboBox.Text = searchStr;
        }

        private void ComboBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            cmb.IsDropDownOpen = true;

            if (!string.IsNullOrEmpty(cmb.Text))
            {
                string fullText = cmb.Text + e.Text;
                Filter(fullText, FilterPropertyName);
                //cmb.ItemsSource = _returnSalesViewModel.ProductsList.Where(s => s.Name.StartsWith(fullText, StringComparison.InvariantCultureIgnoreCase)).ToList();
                return;
            }
            //else if (!string.IsNullOrEmpty(e.Text))
            if (!string.IsNullOrEmpty(e.Text))
            {
                //cmb.ItemsSource = _returnSalesViewModel.ProductsPriceList.Where(s => s.ProductName.StartsWith(cmb.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
                Filter(e.Text, FilterPropertyName);
            }
            else
            {
                comboBox.ItemsSource = ItemsSource;
            }
        }
    }
}
