/*  
 * CustDataGridComboBoxColumn implements data grid column combobox with popup DataGrid control.
 * Bahrudin Hrnjica, bhrnjica@hotmail.com
 * First Release Oct, 2009
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Controls;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

namespace BHCustCtrl
{

    public delegate void OnComboItemSelected(object selectedItem);

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
        //Columns of DataGrid
        private ObservableCollection<DataGridTextColumn> columns;
        public TextBox _cboTextBox;
        //Cust Combobox  cell edit
        public  CustComboBox comboBox;

        public event OnComboItemSelected ComboBoxSelectedEvent;

        public CustDataGridComboBoxColumn()
        {
            comboBox = new CustComboBox();
            comboBox.IsEditable = true;
           // comboBox.PreviewKeyDown += ComboBox_PreviewKeyDown;
            comboBox.Loaded += ComboBox_Loaded;            
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
            else if (e.Property == CustDataGridComboBoxColumn.SelectedValuePathProperty)
            {
                
                comboBox.SelectedValuePath = SelectedValuePath;
                

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
           if(comboBox.Columns.Count==0)
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
            if (editingEventArgs == null) return null;
            DataGridCell cell = editingEventArgs.Source as DataGridCell;
            if (cell != null)
            {
                // Changed to support EF POCOs
                PropertyInfo info = editingElement.DataContext.GetType().GetProperty(comboBox.SelectedValuePath, BindingFlags.Public | BindingFlags.Instance);
                object obj = info.GetValue(editingElement.DataContext, null);
                comboBox.SelectedValue = obj;
            }
            return comboBox.SelectedItem;

        }

        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            //((DataRowView)editingElement.DataContext).Row[this.SelectedValuePath] = comboBox.SelectedValue;
            //BindingExpression binding = editingElement.GetBindingExpression(ComboBox.SelectedValueProperty);
            //if (binding != null) binding.UpdateSource();
            //editingElement.SetValue(ComboBox.SelectedItemProperty, comboBox.SelectedItem);
            //var propInfo = editingElement.DataContext.GetType().GetProperty(this.SelectedValuePath);
            ////propInfo.SetValue(editingElement.Parent, comboBox.SelectedValue, new object[] { });
            ////((Microsoft.Windows.Controls.DataGridCell)editingElement.Parent).SetValue(DataGridCell.ContentProperty, comboBox.SelectedValue);

            ////return true;// base.CommitCellEdit(editingElement);
            //editingElement.DataContext = comboBox.SelectedItem;
            //base.CommitCellEdit(editingElement);
            //return true;

            // Dynamically set the item on our POCO (the DataContext).
            PropertyInfo info = editingElement.DataContext.GetType().GetProperty(comboBox.SelectedValuePath, BindingFlags.Public | BindingFlags.Instance);
            info.SetValue(editingElement.DataContext, comboBox.SelectedValue, null);
            //var grid =FindMyParentHelper<DataGrid>.FindAncestor(editingElement);

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

                    var parentT = parent as T;
                    if (parentT != null) return parentT;

                    dependencyObject = parent;
                }
            }
        }

    }
}
