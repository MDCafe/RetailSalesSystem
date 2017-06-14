using System.Windows.Controls;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using RetailManagementSystem.ViewModel.Sales;
using RetailManagementSystem.ViewModel.Masters;
using RetailManagementSystem.ViewModel.Purchases;

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

        public DataTemplate SalesViewTemplate
        {
            get;
            set;
        }

        public DataTemplate PurchaseViewTemplate
        {
            get;
            set;
        }

        //public DataTemplate SalesDataTemplate
        //{
        //    get
        //    {
        //        //set up the stack panel
        //        FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(Sales.SalesEntry));
        //        spFactory.Name = "myComboFactory";
        //        //spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

        //        _salesDataTemplate.VisualTree = spFactory;
        //        return _salesDataTemplate;
        //    }
        //    set
        //    {
        //        _salesDataTemplate = value;
        //   }
        //}

        public DataTemplate CustomerDataTemplate
        {
            get;
            set;            
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            //var content = item as ContentPresenter;
            //content.ContentTemplate = CustomerDataTemplate;            

            if (item is SalesEntryViewModel)
            {
                return SalesViewTemplate;
            }

            if (item is CustomerViewModel)
            {
                return CustomerDataTemplate;
            }

            if (item is PurchaseEntryViewModel)
              return PurchaseViewTemplate;

            //if (item is FileStatsViewModel)
            //    return FileStatsViewTemplate;

            //if (item is RecentFilesViewModel)
            //  return RecentFilesViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
