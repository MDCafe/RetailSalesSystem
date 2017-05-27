using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using RetailManagementSystem.ViewModel;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.ViewModel.Sales;
using RetailManagementSystem.ViewModel.Masters;

namespace RetailManagementSystem.View.Pane
{
  class PanesTemplateSelector : DataTemplateSelector
    {
        DataTemplate _salesDataTemplate;
        DataTemplate _customerDataTemplate;

        public PanesTemplateSelector()
        {
            _salesDataTemplate = new DataTemplate(typeof(Sales.SalesEntry));
            _customerDataTemplate = new DataTemplate(typeof(Masters.Customer));
        }

        public DataTemplate DocumentViewTemplate
        {
            get;
            set;
        }

        public DataTemplate SalesDataTemplate
        {
            get
            {
                //set up the stack panel
                FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(Sales.SalesEntry));
                spFactory.Name = "myComboFactory";
                //spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

                _salesDataTemplate.VisualTree = spFactory;
                return _salesDataTemplate;
            }
            set
            {
                _salesDataTemplate = value;
           }
        }

        public DataTemplate CustomerDataTemplate
        {
            get;
            set;
            //get
            //{
            //    //set up the stack panel
            //    FrameworkElementFactory spFactory = new FrameworkElementFactory(Masters.Customer);
            //    spFactory.Name = "Masters";
            //    //spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            //    _customerDataTemplate.VisualTree = spFactory;
            //    return _customerDataTemplate;
            //}
            //set
            //{
            //    _customerDataTemplate = value;
            //}
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            //var content = item as ContentPresenter;
            //content.ContentTemplate = CustomerDataTemplate;

            if (item is SalesEntryViewModel)
            {
                return DocumentViewTemplate;
            }

            if (item is CustomerViewModel)
            {
                return CustomerDataTemplate;
            }

            //if (item is StartPageViewModel)
            //  return StartPageViewTemplate;

            //if (item is FileStatsViewModel)
            //    return FileStatsViewTemplate;

            //if (item is RecentFilesViewModel)
            //  return RecentFilesViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
