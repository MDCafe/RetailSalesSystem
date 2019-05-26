using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.View.Masters;
using RetailManagementSystem.View.Reports.Accounts;
using RetailManagementSystem.View.Reports.Purchases;
using RetailManagementSystem.View.Reports.Sales;
using RetailManagementSystem.View.Reports.Sales.Customers;
using RetailManagementSystem.View.Reports.Stock;
using RetailManagementSystem.View.Sales;
using RetailManagementSystem.ViewModel.Accounts;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.ViewModel.Graphs;
using RetailManagementSystem.ViewModel.Purchases;
using RetailManagementSystem.ViewModel.Reports.Accounts;
using RetailManagementSystem.ViewModel.Reports.Stock;
using RetailManagementSystem.ViewModel.Sales;
using RetailManagementSystem.ViewModel.Stocks;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel
{
    class Workspace : ViewModelBase
    {
        ObservableCollection<DocumentViewModel> _documentViewModels = null;
        ReadOnlyObservableCollection<DocumentViewModel> _readOnlyDocView = null;
        public ReadOnlyObservableCollection<DocumentViewModel> DocumentViews
        {
            get
            {
                if (_readOnlyDocView == null)
                    _readOnlyDocView = new ReadOnlyObservableCollection<DocumentViewModel>(_documentViewModels);

                return _readOnlyDocView;
            }
        }

        public Xceed.Wpf.AvalonDock.Layout.LayoutDocumentPane LayoutDocPane { get; set; }

        public Window MainDockingWindow;

        protected Workspace()
        {
            _documentViewModels = new ObservableCollection<DocumentViewModel>();
        }

        public static Workspace This { get; } = new Workspace();


        private void ShowWindowDialog(Window dialogWindow)
        {
            dialogWindow.Owner = MainDockingWindow;
            dialogWindow.ShowDialog();
        }

        #region OpenSalesEntryCommand
        RelayCommand<object> _openSalesEntryCommand = null;
        public ICommand OpenSalesEntryCommand
        {
          get
          {
            if (_openSalesEntryCommand == null)
            {
                _openSalesEntryCommand = new RelayCommand<object>((p) => OnOpenSalesEntryCommand(p), (p) => CanNew(p));
            }

            return _openSalesEntryCommand;
          }
        }

        private bool CanNew(object parameter) 
        {
          return true;
        }

        private void OnOpenSalesEntryCommand(object paramValue)
        {
            //Notification.NotificationViewModel nvm = new Notification.NotificationViewModel();
            //nvm.ShowNotificationExecute();
            //return;

            if (typeof(SalesParams) != paramValue.GetType())
            {
                var salesParam = new SalesParams() { ShowAllCustomers = bool.Parse(paramValue.ToString()) };
                _documentViewModels.Add(new SalesEntryViewModel(salesParam));                
            }
            else
                _documentViewModels.Add(new SalesEntryViewModel(paramValue as SalesParams));

            ActiveDocument = _documentViewModels.Last();
            //ActiveDocument.IsSelected = true;
            //ActiveDocument.IsActive = true;
            //MakeActiveLayout(ActiveDocument.Title);

        }

        public void MakeActiveLayout(String layoutTitle)
        {
            foreach (Xceed.Wpf.AvalonDock.Layout.LayoutDocument child in LayoutDocPane.Children)
            {
                if (child.Title == layoutTitle)
                {
                    child.IsSelected = true;
                }
            }
        }

        #endregion

        #region OpenSalesEntryTempCommand
        RelayCommand<object> _openSalesEntryTempCommand = null;
        public ICommand OpenSalesEntryTempCommand
        {
            get
            {
                if (_openSalesEntryTempCommand == null)
                {
                    _openSalesEntryTempCommand = new RelayCommand<object>((p) => OnOpenSalesEntryTempCommand(p));
                }

                return _openSalesEntryTempCommand;
            }
        }      

        private void OnOpenSalesEntryTempCommand(object paramValue)
        {
            //check if temp data exists 
            if (RMSEntitiesHelper.Instance.RMSEntities.SaleTemps.Count() == 0)
            {
                Utility.ShowErrorBox("No temporary data is available");
                return;
            }

            var tempRecords = RMSEntitiesHelper.Instance.GetNewInstanceOfRMSEntities().SaleTemps.GroupBy(g => g.Guid);

            foreach (var item in tempRecords)
            {
                var salesParams = new SalesParams() { GetTemproaryData = true,Guid = item.Key};
                _documentViewModels.Add(new SalesEntryViewModel(salesParams));
                ActiveDocument = _documentViewModels.Last();
            }
            
        }

        #endregion

        #region OpenAmendSalesCommand
        RelayCommand<object> _openAmendSalesCommand = null;
        public ICommand OpenAmendSalesCommand
        {
            get
            {
                if (_openAmendSalesCommand == null)
                {
                    _openAmendSalesCommand = new RelayCommand<object>((p) => OnOpenAmendSalesCommand(p), (p) => CanAmendNew(p));
                }

                return _openAmendSalesCommand;
            }
        }

        private bool CanAmendNew(object parameter)
        {
            return true;
        }

        private void OnOpenAmendSalesCommand(object showAll)
        {            
            try
            {
                var amendSales = new AmendSales(false);
                ShowWindowDialog(amendSales);                
            }
            catch (Exceptions.RMSException ex)
            {
                Utility.ShowErrorBox(ex.Message);                
            }
            catch (Exception ex)
            {
                Utility.ShowErrorBox(ex.Message);
                //log here
            }
        }

        #endregion

        #region OpenReturnSalesCommand
        RelayCommand<object> _openReturnSalesCommand = null;
        public ICommand OpenReturnSalesCommand
        {
            get
            {
                if (_openReturnSalesCommand == null)
                {
                    _openReturnSalesCommand = new RelayCommand<object>((p) => OnOpenReturnSalesCommand(p));
                }

                return _openReturnSalesCommand;
            }
        }        

        private void OnOpenReturnSalesCommand(object showAll)
        {            
            try
            {
                var showRestrictedCustomers = false;
                if (showAll != null)
                    showRestrictedCustomers= bool.Parse(showAll.ToString());
                _documentViewModels.Add(new ReturnSalesViewModel(showRestrictedCustomers));
                ActiveDocument = _documentViewModels.Last();
            }
            catch (Exceptions.RMSException ex)
            {
                Utility.ShowErrorBox(ex.Message);
            }
            catch (Exception ex)
            {
                Utility.ShowErrorBox(ex.Message);
                //log here
            }
        }

        #endregion

        #region OpenPurchaseEntryCommand
        RelayCommand<object> _openPurchaseEntryCommand = null;
        public ICommand OpenPurchaseEntryCommand
        {
            get
            {
                if (_openPurchaseEntryCommand == null)
                {
                    _openPurchaseEntryCommand = new RelayCommand<object>((p) => OnOpenPurchaseEntryCommand(p));
                }

                return _openPurchaseEntryCommand;
            }
        }        

        private void OnOpenPurchaseEntryCommand(object param)
        {            
            try
            {
                var purchaseParams = param as PurchaseParams;
                _documentViewModels.Add(new PurchaseEntryViewModel(purchaseParams));
                ActiveDocument = _documentViewModels.Last();
            }
            catch (Exceptions.RMSException ex)
            {
                Utility.ShowErrorBox(ex.Message);
            }
            catch (Exception ex)
            {
                Utility.ShowErrorBox(ex.Message);
                //log here
            }
        }

        #endregion

        #region OpenAmendPurchaseCommand
        RelayCommand<object> _openAmendPurchaseCommand = null;
        public ICommand OpenAmendPurchaseCommand
        {
            get
            {
                if (_openAmendPurchaseCommand == null)
                {
                    _openAmendPurchaseCommand = new RelayCommand<object>((p) =>
                    {
                        try
                        {
                            AmendPurchases amendPurchases = new AmendPurchases(false);
                            ShowWindowDialog(amendPurchases);
                        }
                        catch (Exceptions.RMSException ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                            //log here
                        }
                    });
                }

                return _openAmendPurchaseCommand;
            }
        }      

        #endregion

        #region OpenReturnPurchaseCommand
        RelayCommand<object> _openReturnPurchaseCommand = null;
        public ICommand OpenReturnPurchaseCommand
        {
            get
            {
                if (_openReturnPurchaseCommand == null)
                {
                    _openReturnPurchaseCommand = new RelayCommand<object>((p) => OnopenReturnPurchaseCommand(p));
                }

                return _openReturnPurchaseCommand;
            }
        }

        private void OnopenReturnPurchaseCommand(object showAll)
        {
            try
            {
                var showRestrictedCustomers = false;
                if (showAll != null)
                    showRestrictedCustomers = bool.Parse(showAll.ToString());
                _documentViewModels.Add(new ReturnsViewModel(showRestrictedCustomers));
                ActiveDocument = _documentViewModels.Last();
            }
            catch (Exceptions.RMSException ex)
            {
                Utility.ShowErrorBox(ex.Message);
            }
            catch (Exception ex)
            {
                Utility.ShowErrorBox(ex.Message);
                //log here
            }
        }

        #endregion

        #region OpenCustomerCommand
        RelayCommand<object> _openCustomerCommand = null;
        public ICommand OpenCustomerCommand
        {
            get
            {
                if (_openCustomerCommand == null)
                {
                    _openCustomerCommand = new RelayCommand<object>((p) =>
                    {
                        try
                        {
                            View.Masters.Customer customer = new View.Masters.Customer();
                            ShowWindowDialog(customer);
                        }
                        catch (Exceptions.RMSException ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                            //log here
                        }
                    });
                }

                return _openCustomerCommand;
            }
        }        

        #endregion

        #region OpenCompaniesCommand
        RelayCommand<object> _openCompaniesCommand = null;
        public ICommand OpenCompaniesCommand
        {
            get
            {
                if (_openCompaniesCommand == null)
                {
                    _openCompaniesCommand = new RelayCommand<object>((p) =>
                    {
                        try
                        {
                            var companiesView = new Companies();
                            ShowWindowDialog(companiesView);
                        }
                        catch (Exceptions.RMSException ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                            //log here
                        }
                    });
                }

                return _openCompaniesCommand;
            }
        }

        # endregion

        #region OpenProductCommand
        RelayCommand<object> _openProductCommand = null;
        public ICommand OpenProductCommand
        {
            get
            {
                if (_openProductCommand == null)
                {
                    _openProductCommand = new RelayCommand<object>((p) =>
                    {
                        try
                        {
                            Products productView = new Products();
                            ShowWindowDialog(productView);
                        }
                        catch (Exceptions.RMSException ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                            //log here
                        }
                    }
                    );
                }
                return _openProductCommand;
            }
        }     

        #endregion

        #region OpenStockTransactionCommand
        RelayCommand<object> _openStockTransactionCommand = null;
        public ICommand OpenStockTransactionCommand
        {
            get
            {
                if (_openStockTransactionCommand == null)
                {
                    _openStockTransactionCommand = new RelayCommand<object>((p) => OnOpenStockTransactionCommand(p));
                }
                return _openStockTransactionCommand;
            }
        }

        private void OnOpenStockTransactionCommand(object paramValue)
        {
            _documentViewModels.Add(new SwapsViewModel());
            ActiveDocument = _documentViewModels.Last();
        }

        #endregion

        #region OpenCustomerBillPaymentsCommand
        RelayCommand<object> _openCustomerBillPaymentsCommand = null;
        public ICommand OpenCustomerBillPaymentsCommand
        {
            get
            {
                if (_openCustomerBillPaymentsCommand == null)
                {
                    _openCustomerBillPaymentsCommand = new RelayCommand<object>((p) => OnOpenCustomerBillPaymentsCommand(p));
                }
                return _openCustomerBillPaymentsCommand;
            }
        }

        private void OnOpenCustomerBillPaymentsCommand(object paramValue)   
        {
            bool showAll = false;
            if (paramValue !=null)
            {
                showAll = bool.Parse(paramValue.ToString());
            }

            _documentViewModels.Add(new CustomerBillPaymentsViewModel(showAll));
            ActiveDocument = _documentViewModels.Last();
        }

        #endregion


        #region OpenCustomerBillPaymentsReportCommand
        RelayCommand<object> _openCustomerBillPaymentsReportCommand = null;
        public ICommand OpenCustomerBillPaymentsReportCommand
        {
            get
            {
                if (_openCustomerBillPaymentsReportCommand == null)
                {
                    _openCustomerBillPaymentsReportCommand = new RelayCommand<object>((showAll) =>
                    {
                        try
                        {
                            var showRestrictedCustomers = false;
                            if (showAll != null)
                                showRestrictedCustomers = bool.Parse(showAll.ToString());

                            var custPayReport = new CustomerPaymentDetailsReport(showRestrictedCustomers);
                            ShowWindowDialog(custPayReport);

                        }
                        catch (Exceptions.RMSException ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                            //log here
                        }
                    });
                }
                return _openCustomerBillPaymentsReportCommand;
            }
        }
        #endregion


        #region ActiveDocument

        private DocumentViewModel _activeDocument = null;
        public DocumentViewModel ActiveDocument
        {
          get { return _activeDocument; }
          set
          {
            if (_activeDocument != value)
            {
              _activeDocument = value;
              RaisePropertyChanged("ActiveDocument");
                    ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
                }
          }
        }

        public event EventHandler ActiveDocumentChanged;

        #endregion

        #region Reports

        #region OpenDailySalesReportCommand
        RelayCommand<object> _openDailySalesReportCommand = null;
        public ICommand OpenDailySalesReportCommand
        {
            get
            {
                if (_openDailySalesReportCommand == null)
                {
                    _openDailySalesReportCommand = new RelayCommand<object>((showAll) =>
                    {
                        try
                        {
                            var showRestrictedCustomers = false;
                            if (showAll != null)
                                showRestrictedCustomers = bool.Parse(showAll.ToString());

                            var salesSummary = new SalesSummary(showRestrictedCustomers);
                            ShowWindowDialog(salesSummary);

                        }
                        catch (Exceptions.RMSException ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                            //log here
                        }
                    });
                }

                return _openDailySalesReportCommand;
            }
        }

        #endregion

        #region OpenCustomerWiseSalesReportCommand
        RelayCommand<object> _openCustomerWiseSalesReportCommand = null;
        public ICommand OpenCustomerWiseSalesReportCommand
        {
            get
            {
                if (_openCustomerWiseSalesReportCommand == null)
                {
                    _openCustomerWiseSalesReportCommand = new RelayCommand<object>((showAll) =>
                    {
                        try
                        {
                            var showRestrictedCustomers = false;
                            if (showAll != null)
                                showRestrictedCustomers = bool.Parse(showAll.ToString());

                            var customerWiseSales = new CustomerWiseSales(showRestrictedCustomers);
                            ShowWindowDialog(customerWiseSales);

                        }
                        catch (Exceptions.RMSException ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                            //log here
                        }
                    });
                }

                return _openCustomerWiseSalesReportCommand;
            }
        }

        #endregion

        #region OpenDailyPurchaseReportCommand
        RelayCommand<object> _openDailyPurchaseReportCommand = null;
        public ICommand OpenDailyPurchaseReportCommand
        {
            get
            {
                if (_openDailyPurchaseReportCommand == null)
                {
                    _openDailyPurchaseReportCommand = new RelayCommand<object>((showAll) =>
                    {
                        try
                        {
                            var showRestrictedCustomers = false;
                            if (showAll != null)
                                showRestrictedCustomers = bool.Parse(showAll.ToString());

                            var PurchaseSummary = new PurchaseSummary(showRestrictedCustomers);
                            ShowWindowDialog(PurchaseSummary);

                        }
                        catch (Exceptions.RMSException ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                            //log here
                        }
                    });
                }

                return _openDailyPurchaseReportCommand;
            }
        }       

        #endregion

        #region OpenStockReportCommand
        RelayCommand<object> _openStockReportCommand = null;
        public ICommand OpenStockReportCommand
        {
            get
            {
                if (_openStockReportCommand == null)
                {
                    _openStockReportCommand = new RelayCommand<object>((showAll) =>
                    {
                        try
                        {
                            var showRestrictedCustomers = false;
                            if (showAll != null)
                                showRestrictedCustomers = bool.Parse(showAll.ToString());

                            var stockReport = new StockReport(showRestrictedCustomers);
                            ShowWindowDialog(stockReport);

                        }
                        catch (Exceptions.RMSException ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                            //log here
                        }
                    });
                }

                return _openStockReportCommand;
            }
        }       
        #endregion

        #region OpenStockBalanceReportCommand
        RelayCommand<object> _openStockBalanceReportCommand = null;
        public ICommand OpenStockBalanceReportCommand
        {
            get
            {
                if (_openStockBalanceReportCommand == null)
                {
                    _openStockBalanceReportCommand = new RelayCommand<object>((p) =>
                    {
                        try
                        {
                            var stockBalanceReport = new StockBalanceReport(false);
                            ShowWindowDialog(stockBalanceReport);

                        }
                        catch (Exceptions.RMSException ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                            //log here
                        }
                    });
                }
                return _openStockBalanceReportCommand;
            }
        }      
        #endregion

        #region OpenOrderProductReportCommand
        RelayCommand<object> _openOrderProductReportCommand = null;
        public ICommand OpenOrderProductReportCommand
        {
            get
            {
                if (_openOrderProductReportCommand == null)
                {
                    _openOrderProductReportCommand = new RelayCommand<object>((p) => OnOpenOrderProductReportCommand());
                }

                return _openOrderProductReportCommand;
            }
        }

        private void OnOpenOrderProductReportCommand()
        {
            try
            {
                ProductsOrderReportViewModel proVM = new ProductsOrderReportViewModel(false);
                proVM.ShowReport();

            }
            catch (Exceptions.RMSException ex)
            {
                Utility.ShowErrorBox(ex.Message);
            }
            catch (Exception ex)
            {
                Utility.ShowErrorBox(ex.Message);
                //log here
            }
        }
        #endregion

        #region  OpenAllPendingCreditReportCommand
        RelayCommand<object> _openAllPendingCreditReportCommand = null;
        public ICommand OpenAllPendingCreditReportCommand
        {
            get
            {
                if (_openAllPendingCreditReportCommand == null)
                {
                    _openAllPendingCreditReportCommand = new RelayCommand<object>((p) => OnOpenAllPendingCreditReportCommand(p));
                }

                return _openAllPendingCreditReportCommand;
            }
        }
        private void OnOpenAllPendingCreditReportCommand(object showAll)
        {
            try
            {
                var showAllCustomers = bool.Parse(showAll.ToString());
                var allPendingCreditReportViewModel = new AllPendingCreditReportViewModel(showAllCustomers);
            }
            catch (Exceptions.RMSException ex)
            {
                Utility.ShowErrorBox(ex.Message);
            }
            catch (Exception ex)
            {
                Utility.ShowErrorBox(ex.Message);
                //log here
            }
        }

        #endregion

        public void OpenReport(ReportViewModel rptViewModel)
        {
            _documentViewModels.Add(rptViewModel);
            ActiveDocument = _documentViewModels.Last();
        }

        #endregion

        #region Graphs

        #region OpenSalesGraphCommand
        RelayCommand<object> _openSalesGraphCommand = null;
        public ICommand OpenSalesGraphCommand
        {
            get
            {
                if (_openSalesGraphCommand == null)
                {
                    _openSalesGraphCommand = new RelayCommand<object>((p) => OnOpenSalesGraphCommand(p));
                }

                return _openSalesGraphCommand;
            }
        }

        private void OnOpenSalesGraphCommand(object showAll)
        {
            try
            {
                var showRestrictedCustomers = false;
                if (showAll != null)
                    showRestrictedCustomers = bool.Parse(showAll.ToString());

                SalesGraphViewModel SalesGraphViewModel = new SalesGraphViewModel();
                _documentViewModels.Add(SalesGraphViewModel);
                ActiveDocument = _documentViewModels.Last();
            }
            catch (Exceptions.RMSException ex)
            {
                Utility.ShowErrorBox(ex.Message);
            }
            catch (Exception ex)
            {
                Utility.ShowErrorBox(ex.Message);
                //log here
            }
        }

        #endregion

        #endregion

        #region System Operations

        RelayCommand<object> _changeSystemDBDateCommand = null;
        public ICommand ChangeSystemDBDateCommand
        {
            get
            {
                if (_changeSystemDBDateCommand == null)
                {
                    _changeSystemDBDateCommand = new RelayCommand<object>((p) => OnChangeSystemDBDateCommand(p));
                }

                return _changeSystemDBDateCommand;
            }
        }

        private void OnChangeSystemDBDateCommand(object showAll)
        {
            try
            {
                if (RMSEntitiesHelper.Instance.CheckSystemDBDate())
                {
                    Utility.ShowErrorBox("System Date is already changed to next date");
                    return;
                }

                var login = new View.Entitlements.Login(true);
                var loginResult = login.ShowDialog();
                if (!loginResult.Value)
                {                   
                    return;
                }

                if (!RMSEntitiesHelper.Instance.IsAdmin(login.txtUserId.Text))
                {
                    Utility.ShowMessageBox("You are not authorized to perform this operation");
                    return;
                }
                var result =Utility.ShowMessageBoxWithOptions("Do you want to change the system date to tommorrow's date?", System.Windows.MessageBoxButton.YesNo);
                if(result == MessageBoxResult.No) return;

                RMSEntitiesHelper.Instance.UpdateSystemDBDate();
            }
            catch (Exceptions.RMSException ex)
            {
                Utility.ShowErrorBox(ex.Message);
            }
            catch (Exception ex)
            {
                Utility.ShowErrorBox(ex.Message);
                //log here
            }
        }

        #endregion

        internal bool Close(DocumentViewModel doc)
        {
            {
                var docToClose = doc as DocumentViewModel;

                if (docToClose != null)
                {
                    //if (docToClose.IsDirty)
                    //{
                    //    var res = MessageBox.Show("Unsaved changes..!! Do you want to save?", "Product Info here--", MessageBoxButton.YesNoCancel);
                    //    if (res == MessageBoxResult.Cancel)
                    //        return false;

                    //    if (res == MessageBoxResult.Yes)
                    //    {
                    //        //doc.GetBillCommand.Execute(null);
                    //    }
                    //}
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _documentViewModels.Remove(docToClose);
                    }
                    );
                }
                return true;
            }
        }

     }
}
