/*  
 * CustComboBox implements combobox with popup System.Windows.Controls.DataGrid control.
 * Bahrudin Hrnjica, bhrnjica@hotmail.com
 * First Release Oct, 2009
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using Microsoft.Windows.Controls.Primitives;
using log4net;

namespace BHCustCtrl
{
    /// <summary>
    /// CustComboBox implements combobox with popup DataGrid control.
    /// Usage:
    ///     <local:CustComboBox Margin="121,0,251,159" VerticalAlignment="Bottom" Height="29" 
	///	                        ItemsSource="{Binding Collection}" DisplayMemberPath="Property1" IsEditable="True" >
    ///	                        
	///		                <toolKit:DataGridTextColumn Header="HeaderTitle1" Binding="{Binding Property1, Mode=Default}" />
    ///		                <toolKit:DataGridTextColumn Header="HeaderTitle2" Binding="{Binding Property2, Mode=Default}"/>
    ///		                <toolKit:DataGridTextColumn Header="HeaderTitle3" Binding="{Binding Property3, Mode=Default}"/>
    ///		                
	///	    </local:CustComboBox>
    ///
    /// </summary>
    [DefaultProperty("Columns")]
    [ContentProperty("Columns")]
    public class CustComboBox : ComboBox
    {
        const string partPopupDataGrid = "PART_PopupDataGrid";
        //Columns of DataGrid
        private ObservableCollection<Microsoft.Windows.Controls.DataGridTextColumn> columns;
        //Attached DataGrid control
        private Microsoft.Windows.Controls.DataGrid popupDataGrid;

        static readonly ILog _log = LogManager.GetLogger(typeof(CustComboBox));

        static CustComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustComboBox), new FrameworkPropertyMetadata(typeof(CustComboBox)));
                        
        }

        //The property is default and Content property for CustComboBox
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<Microsoft.Windows.Controls.DataGridTextColumn> Columns
        {
            get
            {
                if (this.columns == null)
                {
                    this.columns = new ObservableCollection<Microsoft.Windows.Controls.DataGridTextColumn>();
                }
                return this.columns;
            }
        }

        //Apply theme and attach columns to DataGrid popupo control
        public override void OnApplyTemplate()
        {
            if (popupDataGrid == null)
            {
                
                popupDataGrid = this.Template.FindName(partPopupDataGrid, this) as Microsoft.Windows.Controls.DataGrid;
                if (popupDataGrid != null && columns != null)
                {
                    //Add columns to DataGrid columns
                    for (int i = 0; i < columns.Count; i++)
                        popupDataGrid.Columns.Add(columns[i]);
                    
                    //Add event handler for DataGrid popup
                    popupDataGrid.MouseDown += new MouseButtonEventHandler(popupDataGrid_MouseDown);
                    popupDataGrid.SelectionChanged += new SelectionChangedEventHandler(popupDataGrid_SelectionChanged);
                }
            }
            //Call base class method
            base.OnApplyTemplate();
        }

        //Synchronize selection between Combo and DataGrid popup
        void popupDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //When open in Blend prevent raising exception 
          if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Microsoft.Windows.Controls.DataGrid dg = sender as Microsoft.Windows.Controls.DataGrid;
                if (dg != null)
                {
                    _log.Debug("dg.SelectedIndex :" + dg.SelectedIndex);
                    SelectedItem = dg.SelectedItem;
                    SelectedValue = dg.SelectedValue;
                    SelectedIndex = dg.SelectedIndex;
                    SelectedValuePath = dg.SelectedValuePath;

                }
            }
        }
        //Event for Microsoft.Windows.Controls.DataGrid popup MouseDown
        void popupDataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Windows.Controls.DataGrid dg = sender as Microsoft.Windows.Controls.DataGrid;
            if (dg != null)
            {
                DependencyObject dep = (DependencyObject)e.OriginalSource;

                // iteratively traverse the visual tree and stop when dep is one of ..
                while ((dep != null) &&
                        !(dep is Microsoft.Windows.Controls.DataGridCell) &&
                        !(dep is DataGridColumnHeader))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                if (dep == null)
                    return;

                if (dep is DataGridColumnHeader)
                {
                   // do something
                }
                //When user clicks to Microsoft.Windows.Controls.DataGrid cell, popup have to be closed
                if (dep is Microsoft.Windows.Controls.DataGridCell)
                {
                    this.IsDropDownOpen = false;
                }
            }
        }

        //When selection changed in combobox (pressing  arrow key down or up) must be synchronized with 
        //opened Microsoft.Windows.Controls.DataGrid popup
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (popupDataGrid == null)
                return;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (IsDropDownOpen)
                {
                    popupDataGrid.SelectedItem = this.SelectedItem;
                    if (popupDataGrid.SelectedItem != null)
                        popupDataGrid.ScrollIntoView(popupDataGrid.SelectedItem);
                }
            }
        }
        protected override void OnDropDownOpened(EventArgs e)
        { 
            popupDataGrid.SelectedItem = this.SelectedItem;
            
            base.OnDropDownOpened(e);

            if (popupDataGrid.SelectedItem != null)
                popupDataGrid.ScrollIntoView(popupDataGrid.SelectedItem);
        }

        public void ClearSelection()
        {
            //popupDataGrid.SelectedIndex = -1;
            popupDataGrid.UnselectAll();
            //popupDataGrid.ScrollIntoView(popupDataGrid.SelectedIndex);
            //ScrollViewer scrollViewer = GetScrollViewer(popupDataGrid) as ScrollViewer;
            //scrollViewer.ScrollToHome();
        }
     
        public static DependencyObject GetScrollViewer(DependencyObject o)
        {            
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }

            return null;
        }
    }
}
