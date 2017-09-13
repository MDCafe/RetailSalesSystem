using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.ViewModel.Sales;
using RetailManagementSystem.View.Sales;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Purchases;
using RetailManagementSystem.View.Reports.Sales;
using RetailManagementSystem.View.Reports.Purchases;

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

        protected Workspace()
        {
            _documentViewModels = new ObservableCollection<DocumentViewModel>();
        }

        static Workspace _this = new Workspace();

        public static Workspace This
        {
          get { return _this; }
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
            if(typeof(SalesParams) != paramValue.GetType())
            {
                var salesParam = new SalesParams() { ShowAllCustomers = bool.Parse(paramValue.ToString()) };
                _documentViewModels.Add(new SalesEntryViewModel(salesParam));                
            }
            else
                _documentViewModels.Add(new SalesEntryViewModel(paramValue as SalesParams));

            ActiveDocument = _documentViewModels.Last();
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
            //_documentViewModels.Add(new SalesEntryViewModel(showAllBool));
            //ActiveDocument = _documentViewModels.Last();
            try
            {
                AmendSales amendSales = new AmendSales(false);
                amendSales.ShowDialog();
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
                    _openAmendPurchaseCommand = new RelayCommand<object>((p) => OnOpenAmendPurchase(p));
                }

                return _openAmendPurchaseCommand;
            }
        }
        private void OnOpenAmendPurchase(object showAll)
        {
            try
            {
                AmendPurchases amendPurchases = new AmendPurchases(false);
                amendPurchases.ShowDialog();
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
                _documentViewModels.Add(new ReturnPurchaseViewModel(showRestrictedCustomers));
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
                    _openCustomerCommand = new RelayCommand<object>((p) => OnOpenCustomerCommand());
                }

                return _openCustomerCommand;
            }
        }        

        private void OnOpenCustomerCommand()
        {            
            try
            {
                //_documentViewModels.Add(new CustomerViewModel());
                //ActiveDocument = _documentViewModels.Last();
                View.Masters.Customer customer = new View.Masters.Customer();
                customer.ShowDialog();
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
              if (ActiveDocumentChanged != null)
                ActiveDocumentChanged(this, EventArgs.Empty);
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
                    _openDailySalesReportCommand = new RelayCommand<object>((p) => OnOpenDailySalesReportCommand(p));
                }

                return _openDailySalesReportCommand;
            }
        }

        private void OnOpenDailySalesReportCommand(object showAll)
        {
            try
            {
                var showRestrictedCustomers = false;
                if (showAll != null)
                    showRestrictedCustomers = bool.Parse(showAll.ToString());

                SalesSummary salesSummary = new SalesSummary(false);
                salesSummary.ShowDialog();

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

        #region OpenDailySalesReportCommand
        RelayCommand<object> _openDailyPurchaseReportCommand = null;
        public ICommand OpenDailyPurchaseReportCommand
        {
            get
            {
                if (_openDailyPurchaseReportCommand == null)
                {
                    _openDailyPurchaseReportCommand = new RelayCommand<object>((p) => OnOpenDailyPurchaseReportCommand(p));
                }

                return _openDailyPurchaseReportCommand;
            }
        }

        private void OnOpenDailyPurchaseReportCommand(object showAll)
        {
            try
            {
                var showRestrictedCustomers = false;
                if (showAll != null)
                    showRestrictedCustomers = bool.Parse(showAll.ToString());

                var PurchaseSummary = new PurchaseSummary(false);
                PurchaseSummary.ShowDialog();

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

        internal bool Close(DocumentViewModel doc)
        {
            {
                DocumentViewModel docToClose = doc as DocumentViewModel;

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
                    _documentViewModels.Remove(docToClose);                    
                }
                return true;
            }
        }

        /// <summary>
        /// Bind a window to some commands to be executed by the viewmodel.
        /// </summary>
        /// <param name="win"></param>
        public void InitCommandBinding(Window win)
        {
            win.CommandBindings.Add(new CommandBinding(OpenSalesEntryCommand,
            (s, e) =>
            {
                this.OnOpenSalesEntryCommand(false);
            }));

            win.CommandBindings.Add(new CommandBinding(OpenCustomerCommand,
            (s, e) =>
            {
                this.OnOpenCustomerCommand();
            }));
        }

            //  win.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open,
            //  (s, e) =>
            //  {
            //    this.OnOpen(null);
            //  }));

            //  win.CommandBindings.Add(new CommandBinding(AppCommand.LoadFile,
            //  (s, e) =>
            //  {
            //    if (e == null)
            //      return;

            //    string filename = e.Parameter as string;

            //    if (filename == null)
            //      return;

            //    this.Open(filename);
            //  }));

            //  win.CommandBindings.Add(new CommandBinding(AppCommand.PinUnpin,
            //  (s, e) =>
            //  {
            //    this.PinCommand_Executed(e.Parameter, e);
            //  }));

            //  win.CommandBindings.Add(new CommandBinding(AppCommand.RemoveMruEntry,
            //  (s, e) =>
            //  {
            //    this.RemoveMRUEntry_Executed(e.Parameter, e);
            //  }));

            //  win.CommandBindings.Add(new CommandBinding(AppCommand.AddMruEntry,
            //  (s, e) =>
            //  {
            //    this.AddMRUEntry_Executed(e.Parameter, e);
            //  }));

            //  win.CommandBindings.Add(new CommandBinding(AppCommand.BrowseURL,
            //  (s, e) =>
            //  {
            //    Process.Start(new ProcessStartInfo("http://Edi.codeplex.com"));
            //  }));

            //  win.CommandBindings.Add(new CommandBinding(AppCommand.ShowStartPage,
            //  (s, e) =>
            //  {
            //    StartPageViewModel spage = this.GetStartPage(true);

            //    if (spage != null)
            //    {
            //      this.ActiveDocument = spage;
            //    }
            //  }));
            //}
        }
}
