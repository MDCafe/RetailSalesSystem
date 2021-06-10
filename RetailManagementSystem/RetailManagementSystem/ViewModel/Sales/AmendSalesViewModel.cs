using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.ViewModel.Reports.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Sales
{
    class AmendSalesViewModel : ViewModelBase
    {
        bool _showRestrictedCustomers;
        Customer _selectedCustomer;
        string _selectedCustomerText;
        IEnumerable<Sale> _billList;        
        int _categoryId;

        public int? BillNo { get; set; }
        public string BillNoText { get; set; }
        public DateTime FromSalesDate { get; set; }
        public DateTime ToSalesDate { get; set; }
        public IList<Customer> CustomersList
        {
            get
            {
                using (var rmsEntities = new RMSEntities())
                {
                    if (_showRestrictedCustomers)
                        return rmsEntities.Customers.Where(c => c.CustomerTypeId == Constants.CUSTOMERS_OTHERS).OrderBy(o => o.Name).ToList();

                    return rmsEntities.Customers.Where(c => c.CustomerTypeId != Constants.CUSTOMERS_OTHERS).OrderBy(o => o.Name).ToList();
                }
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
            _showRestrictedCustomers = showRestrictedCustomers;         

            _categoryId = _showRestrictedCustomers ? Constants.CUSTOMERS_OTHERS : Constants.CUSTOMERS_HOTEL;

            SetServerDate();
        }


        private void SetServerDate()
        {
            var serverDate = RMSEntitiesHelper.GetServerDate();
            FromSalesDate = serverDate;
            ToSalesDate = serverDate;
        }

        #region Clear Command
        RelayCommand<object> _clearCommand = null;

        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand<object>((p) => Clear());
                }

                return _clearCommand;
            }
        }

        internal void Clear()
        {
            BillNo = null;
            SelectedCustomer = null;
            BillList = null;
            SetServerDate();
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
                    _printCommand = new RelayCommand<Window>((p) => OnPrint(p), (p) => { return BillNo != null; });
                }

                return _printCommand;
            }
        }

        private void OnPrint(Window window)
        {
            var customerBill = RMSEntitiesHelper.CheckIfBillExists(BillNo.Value, _categoryId, window);
            if (customerBill == null) return;

            SalesBillDetailsViewModel salesReportVM = new SalesBillDetailsViewModel(_showRestrictedCustomers, BillNo)
            {
                ShowPrintReceiptButton = Visibility.Visible,
                RunningBillNo = BillNo
            };
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
                    _amendCommand = new RelayCommand<Window>((w) => OnAmend(w), (p) => { return BillNo != null; });
                }

                return _amendCommand;
            }
        }

        private void OnAmend(Window window)
        {
            var customerBill = RMSEntitiesHelper.CheckIfBillExists(BillNo.Value, _categoryId, window);
            if (customerBill == null)
                return;

            using (var rmsEntities = new RMSEntities())
            {
                var cancelBill = rmsEntities.Sales.FirstOrDefault(s => s.RunningBillNo == BillNo && customerBill.CustomerId == s.CustomerId);

                if (cancelBill.IsCancelled.HasValue && cancelBill.IsCancelled.Value)
                {
                    Utility.ShowWarningBox(window, "Bill has been cancelled already");
                    return;
                }

                View.Entitlements.Login login = new View.Entitlements.Login();
                var result = login.ShowDialog();
                //var t = ;
                if (!result.Value || !RMSEntitiesHelper.Instance.IsAdmin(login.LoginVM.UserId))
                {
                    return;
                }

                var saleParams = new SalesParams() { Billno = BillNo, CustomerId = customerBill.CustomerId, ShowAllCustomers = _showRestrictedCustomers };

                Workspace.This.OpenSalesEntryCommand.Execute(saleParams);
                _closeWindowCommand.Execute(window);
            }
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
                    _getCustomerBillsCommand = new RelayCommand<Window>((w) => GetCustomerBills(), (p) => CanGetCustomerBills());
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
            using (var rmsEntities = new RMSEntities())
            {
                if (BillList == null)
                    rmsEntities.Sales.ToList();

                var sql = "Select * from Sales s where s.CustomerId ={0}  and date(s.addedOn) >= {1} and date(s.addedOn) <= {2} order by s.ModifiedOn";
                var billList = rmsEntities.Database.SqlQuery<Sale>(sql, SelectedCustomer.Id, FromSalesDate.ToString("yyyy-MM-dd"), ToSalesDate.ToString("yyyy-MM-dd"));

                BillList = billList.ToList();
            }
        }


        #endregion
    }
}
