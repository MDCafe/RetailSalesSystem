using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using RetailManagementSystem.ViewModel.Reports.Sales;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;

namespace RetailManagementSystem.ViewModel.Sales
{
    class AmendSalesViewModel : ViewModelBase
    {
        bool _showRestrictedCustomers;                
        Customer _selectedCustomer;               
        string _selectedCustomerText;
        IEnumerable<Sale> _billList;
        RMSEntities _rmsEntities;
        int _categoryId;

        public int? BillNo { get; set; }
        public string BillNoText { get; set; }

        public IEnumerable<Customer> CustomersList
        {
            get
            {               
                if (_showRestrictedCustomers)
                    return _rmsEntities.Customers.Local.Where(c => c.CustomerTypeId == Constants.CUSTOMERS_OTHERS).OrderBy(o => o.Name);

                return _rmsEntities.Customers.Local.Where(c => c.CustomerTypeId != Constants.CUSTOMERS_OTHERS).OrderBy(o => o.Name);
            }
        }

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;                
                RaisePropertyChanged("SelectedCustomer");
            }
        }

        public string SelectedCustomerText
        {
            get { return _selectedCustomerText; }
            set
            {
                _selectedCustomerText = value;
                //NotifyPropertyChanged(() => this._selectedCustomer);
                RaisePropertyChanged("SelectedCustomerText");
            }
        }

        public IEnumerable<Sale> BillList
        {
            get { return _billList; }
            set
            {
                _billList = value;
                //NotifyPropertyChanged(() => this._selectedCustomer);
                RaisePropertyChanged("BillList");
            }
        }

        public AmendSalesViewModel(bool showRestrictedCustomers) 
        {
            _rmsEntities = new RMSEntities();
            _showRestrictedCustomers = showRestrictedCustomers;            
            _rmsEntities.Customers.ToList();
            
            if (_showRestrictedCustomers)
                _categoryId = Constants.CUSTOMERS_OTHERS;
            else
                _categoryId = Constants.CUSTOMERS_HOTEL;

        }

        #region Clear Command

        internal void Clear()
        {
            BillNo = null;
            SelectedCustomer = null;
            BillList = null;
            //_rmsEntities = new RMSEntities();
        }
        #endregion

        #region Print Command
        RelayCommand<Window> _printCommand = null;
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new RelayCommand<Window>((p) => OnPrint(p), (p) => CanExecuteMethod(p));
                }

                return _printCommand;
            }
        }

        private void OnPrint(Window window)
        {
            var customerBill = RMSEntitiesHelper.CheckIfBillExists(BillNo.Value, _categoryId, window);
            if (customerBill == null) return;

            SalesBillDetailsViewModel salesReportVM = new SalesBillDetailsViewModel(_showRestrictedCustomers, BillNo);
            salesReportVM.ShowPrintReceiptButton = Visibility.Visible;
            salesReportVM.RunningBillNo = BillNo;
            salesReportVM.PrintCommand.Execute(window);
            salesReportVM.IsActive = true;
            salesReportVM.IsSelected = true;
        }
        #endregion

        #region Amend Command
        RelayCommand<Window> _amendCommand = null;
        public ICommand AmendCommand
        {
            get
            {
                if (_amendCommand == null)
                {
                    _amendCommand = new RelayCommand<Window>((w) => OnAmend(w),(p) => CanExecuteMethod(p));
                }

                return _amendCommand;
            }
        }

        private void OnAmend(Window window)
        {
            var customerBill = RMSEntitiesHelper.CheckIfBillExists(BillNo.Value, _categoryId,window);
            if (customerBill == null)
                return;

            var cancelBill = _rmsEntities.Sales.FirstOrDefault(s => s.RunningBillNo == BillNo && customerBill.CustomerId == s.CustomerId);

            if (cancelBill.IsCancelled.HasValue && cancelBill.IsCancelled.Value)
            {
                Utility.ShowWarningBox(window,"Bill has been cancelled already");
                return;
            }

            View.Entitlements.Login login = new View.Entitlements.Login();
            var result = login.ShowDialog();
            //var t = ;
            if (!result.Value || !RMSEntitiesHelper.Instance.IsAdmin(login.LoginVM.UserId))
            {                
                return;
            }

            var saleParams = new SalesParams() { Billno = BillNo,CustomerId = customerBill.CustomerId,ShowAllCustomers = _showRestrictedCustomers};

            Workspace.This.OpenSalesEntryCommand.Execute(saleParams);
            _closeWindowCommand.Execute(window);

            //window.DialogResult = true;
        }       

        private bool CanExecuteMethod(object parameter)
        {
            return BillNo != null;
        }
        #endregion

        #region CloseWindow Command
        public RelayCommand<Window> _closeWindowCommand { get; private set; }

        public ICommand CloseWindowCommand
        {
            get
            {
                if (_closeWindowCommand == null)
                {
                    _closeWindowCommand = new RelayCommand<Window>((w) => CloseWindow(w));
                }

                return _closeWindowCommand;
            }
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
                _rmsEntities.Dispose();
            }
        }
        #endregion

        #region GetCustomerBills Command
        public RelayCommand<Window> _getCustomerBillsCommand { get; private set; }

        public ICommand GetCustomerBillsCommand
        {
            get
            {
                if (_getCustomerBillsCommand == null)
                {
                    _getCustomerBillsCommand = new RelayCommand<Window>((w) => GetCustomerBills(),(p) => CanGetCustomerBills());
                }

                return _getCustomerBillsCommand;
            }
        }

        private bool CanGetCustomerBills()
        {
            return SelectedCustomer != null;
        }

        private void GetCustomerBills()
        {
            if(BillList == null)
                _rmsEntities.Sales.ToList();

            BillList = _rmsEntities.Sales.Local.Where(s => s.CustomerId == SelectedCustomer.Id).OrderBy(o => o.ModifiedOn );                       
        }


        #endregion
    }
}
