using RetailManagementSystem.ViewModel;
using RetailManagementSystem.ViewModel.Accounts;
using RetailManagementSystem.ViewModel.Expenses;
using RetailManagementSystem.ViewModel.Graphs;
using RetailManagementSystem.ViewModel.Masters;
using RetailManagementSystem.ViewModel.Purchases;
using RetailManagementSystem.ViewModel.Sales;
using RetailManagementSystem.ViewModel.Stocks;
using System.Windows;
using System.Windows.Controls;

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

        public DataTemplate ExpenseViewTemplate
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

        public DataTemplate PurchaseBillPaymentsTemplate
        {
            get;
            set;
        }

        public DataTemplate GraphViewTemplate
        {
            get;
            set;
        }

        public DataTemplate StockAdjustmentViewTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //var content = item as ContentPresenter;
            //content.ContentTemplate = CustomerDataTemplate;            

            if (item is SalesEntryViewModel) return SalesViewTemplate;

            if (item is ReturnSalesViewModel) return ReturnSalesViewTemplate;

            if (item is CustomerViewModel) return CustomerDataTemplate;

            if (item is PurchaseEntryViewModel) return PurchaseViewTemplate;
            if (item is ExpenseEntryViewModel) return ExpenseViewTemplate;

            if (item is ReturnsViewModel) return ReturnPurchaseViewTemplate;

            if (item is ReportViewModel) return ReportViewTemplate;

            if (item is ProductsViewModel) return ProductsDataTemplate;

            if (item is SwapsViewModel) return StockTansactionViewTemplate;

            if (item is CustomerBillPaymentsViewModel) return CustomerBillPaymentsTemplate;
            if (item is PurchaseBillPaymentsViewModel) return PurchaseBillPaymentsTemplate;

            if (item is GraphsViewModel) return GraphViewTemplate;

            if (item is StockAdjustmentViewModel) return StockAdjustmentViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
