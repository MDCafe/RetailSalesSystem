using System.Windows.Controls;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using RetailManagementSystem.ViewModel.Sales;
using RetailManagementSystem.ViewModel.Masters;
using RetailManagementSystem.ViewModel.Purchases;
using RetailManagementSystem.ViewModel;
using RetailManagementSystem.ViewModel.Stocks;
using RetailManagementSystem.ViewModel.Accounts;
using RetailManagementSystem.ViewModel.Graphs;

namespace RetailManagementSystem.View.Pane
{
    class PanesTemplateSelector : DataTemplateSelector
    {        
        public DataTemplate SalesViewTemplate
        {
            get;
            set;
        }

        public DataTemplate ReturnSalesViewTemplate
        {
            get;
            set;
        }

        public DataTemplate PurchaseViewTemplate
        {
            get;
            set;
        }

        public DataTemplate ReturnPurchaseViewTemplate
        {
            get;
            set;
        }

        public DataTemplate ReportViewTemplate
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

        public DataTemplate ProductsDataTemplate
        {
            get;
            set;
        }

        public DataTemplate StockTansactionViewTemplate
        {
            get;
            set;
        }

        public DataTemplate CustomerBillPaymentsTemplate
        {
            get;
            set;
        }

        public DataTemplate GraphViewTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            //var content = item as ContentPresenter;
            //content.ContentTemplate = CustomerDataTemplate;            

            if (item is SalesEntryViewModel) return SalesViewTemplate;            
            
            if (item is ReturnSalesViewModel) return ReturnSalesViewTemplate;

            if (item is CustomerViewModel)  return CustomerDataTemplate;
            
            if (item is PurchaseEntryViewModel) return PurchaseViewTemplate;

            if (item is ReturnsViewModel) return ReturnPurchaseViewTemplate;

            if (item is ReportViewModel) return ReportViewTemplate;

            if (item is ProductsViewModel) return ProductsDataTemplate;

            if (item is SwapsViewModel) return StockTansactionViewTemplate;

            if (item is CustomerBillPaymentsViewModel) return CustomerBillPaymentsTemplate;

            if (item is GraphsViewModel) return GraphViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
