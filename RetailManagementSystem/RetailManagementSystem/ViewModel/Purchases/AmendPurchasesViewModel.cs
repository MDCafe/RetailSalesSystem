using System.Linq;
using System.Windows;
using System.Windows.Input;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using System.Collections.Generic;
using RetailManagementSystem.ViewModel.Purchases;
using RetailManagementSystem.ViewModel.Reports.Purhcases;

namespace RetailManagementSystem.ViewModel.Sales
{
    class AmendPurchasesViewModel : PurchaseViewModelbase
    {
        bool _showRestrictedSuppliers;                
        Company _selectedSupplier;               
        string _selectedSupplierText;
        IEnumerable<Purchase> _billList;
        RMSEntities _rmsEntities;
        
        public int? BillNo { get; set; }
        public string BillNoText { get; set; }

        public IEnumerable<Company> SupplierList
        {
            get
            {               
                if (_showRestrictedSuppliers)
                    return _rmsEntities.Companies.Local.Where(c => c.CategoryTypeId != Constants.COMPANIES_MAIN);

                return _rmsEntities.Companies.Local.Where(c => c.CategoryTypeId == Constants.COMPANIES_MAIN);
            }
        }

        public Company SelectedSupplier
        {
            get { return _selectedSupplier; }
            set
            {
                _selectedSupplier = value;                
                RaisePropertyChanged("_selectedSupplier");
            }
        }

        public string SelectedSupplierText
        {
            get { return _selectedSupplierText; }
            set
            {
                _selectedSupplierText = value;
                RaisePropertyChanged("SelectedSupplierText");
            }
        }

        public IEnumerable<Purchase> BillList
        {
            get { return _billList; }
            set
            {
                _billList = value;
                //NotifyPropertyChanged(() => this._selectedCustomer);
                RaisePropertyChanged("BillList");
            }
        }

        public AmendPurchasesViewModel(bool showRestrictedSuppliers) : base(showRestrictedSuppliers)
        {
            _showRestrictedSuppliers = showRestrictedSuppliers;
            _rmsEntities = new RMSEntities();          
            _rmsEntities.Companies.ToList();           
        }

        #region Clear Command
        RelayCommand<object> _clearCommand = null;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand<object>((p) => OnClear());
                }

                return _clearCommand;
            }
        }

        private void OnClear()
        {
            BillNo = null;
            SelectedSupplier = null;
            BillList = null;
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
                    _printCommand = new RelayCommand<Window>((w) => OnPrint(w), (w) => CanExecuteMethod(w));
                }

                return _printCommand;
            }
        }

        private void OnPrint(Window window)
        {
            //Call the print on print & save
            PurchaseSummaryViewModel psummVM = new PurchaseSummaryViewModel(_showRestrictedCompanies);
            psummVM.RunningBillNo = BillNo;
            psummVM.PrintCommand.Execute(null);
            _closeWindowCommand.Execute(window);
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
            var companyBill = RMSEntitiesHelper.CheckIfPurchaseBillExists(BillNo.Value, _categoryId);
            if (companyBill == null)
                return;

            var cancelBill = _rmsEntities.Purchases.FirstOrDefault(s => s.RunningBillNo == BillNo && companyBill.CompanyId == s.CompanyId);

            if (cancelBill.IsCancelled.HasValue && cancelBill.IsCancelled.Value)
            {
                Utility.ShowWarningBox(window, "Bill has been cancelled already");
                return;
            }

            View.Entitlements.Login login = new View.Entitlements.Login(true);
            var result = login.ShowDialog();
            if (!result.Value)
            {
                return;
            }

            var purchaseParams = new PurchaseParams() { Billno = BillNo,CompanyId = companyBill.CompanyId,ShowAllCompanies = _showRestrictedCompanies };

            Workspace.This.OpenPurchaseEntryCommand.Execute(purchaseParams);
            _closeWindowCommand.Execute(window);

         
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
            return SelectedSupplier != null;
        }

        private void GetCustomerBills()
        {
            if(BillList == null)
                _rmsEntities.Purchases.ToList();

            BillList = _rmsEntities.Purchases.Local.Where(s => s.CompanyId == SelectedSupplier.Id);                       
        }


        #endregion
    }
}
