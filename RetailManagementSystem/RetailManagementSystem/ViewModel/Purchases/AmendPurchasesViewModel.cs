using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Purchases;
using RetailManagementSystem.ViewModel.Reports.Purhcases;
using RetailManagementSystem.ViewModel.Base;

namespace RetailManagementSystem.ViewModel.Sales
{
    class AmendPurchasesViewModel : ViewModelBase
    {
        readonly bool _showRestrictedSuppliers;                
        Company _selectedSupplier;               
        string _selectedSupplierText;
        IEnumerable<Purchase> _billList;
        readonly RMSEntities _rmsEntities;
        protected int _categoryId;

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
                RaisePropertyChanged(nameof(SelectedSupplierText));
            }
        }

        public IEnumerable<Purchase> BillList
        {
            get { return _billList; }
            set
            {
                _billList = value;
                //NotifyPropertyChanged(() => this._selectedCustomer);
                RaisePropertyChanged(nameof(BillList));
            }
        }

        public AmendPurchasesViewModel(bool showRestrictedSuppliers)
        {
            _showRestrictedSuppliers = showRestrictedSuppliers;
            if (_showRestrictedSuppliers)
                _categoryId = Constants.COMPANIES_OTHERS;
            else
                _categoryId = Constants.COMPANIES_MAIN;

            _rmsEntities = new RMSEntities();          
            _rmsEntities.Companies.ToList();           
        }

        #region Clear Command
       
        internal void Clear()
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
                    _printCommand = new RelayCommand<Window>((w) => OnPrint(w), (w) => CanExecuteMethod());
                }

                return _printCommand;
            }
        }

        private void OnPrint(Window window)
        {
            var companyBill = RMSEntitiesHelper.CheckIfPurchaseBillExists(BillNo.Value, _categoryId, window);
            if (companyBill == null) return;

            PurchaseSummaryViewModel psummVM = new PurchaseSummaryViewModel(_showRestrictedSuppliers, BillNo)
            {
                RunningBillNo = BillNo
            };
            psummVM.PrintCommand.Execute(null);
            closeWindowCommand.Execute(window);
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
                    _amendCommand = new RelayCommand<Window>((w) => OnAmend(w),(p) => CanExecuteMethod());
                }

                return _amendCommand;
            }
        }

        private void OnAmend(Window window)
        {
            var companyBill = RMSEntitiesHelper.CheckIfPurchaseBillExists(BillNo.Value, _categoryId,window);
            if (companyBill == null)
                return;

            var cancelBill = _rmsEntities.Purchases.FirstOrDefault(s => s.RunningBillNo == BillNo && companyBill.CompanyId == s.CompanyId);

            if (cancelBill.IsCancelled.HasValue && cancelBill.IsCancelled.Value)
            {
                Utility.ShowWarningBox(window, "Bill has been cancelled already");
                return;
            }

            View.Entitlements.Login login = new View.Entitlements.Login();
            var result = login.ShowDialog();
            if (!result.Value || !RMSEntitiesHelper.Instance.IsAdmin(login.LoginVM.UserId))
            {
                return;
            }

            var purchaseParams = new PurchaseParams() { Billno = BillNo,CompanyId = companyBill.CompanyId,ShowAllCompanies = _showRestrictedSuppliers };

            Workspace.This.OpenPurchaseEntryCommand.Execute(purchaseParams);
            closeWindowCommand.Execute(window);

         
        }       

        private bool CanExecuteMethod()
        {
            return BillNo != null;
        }
        #endregion

        #region CloseWindow Command
        public RelayCommand<Window> closeWindowCommand { get; private set; }

        public ICommand CloseWindowCommand
        {
            get
            {
                if (closeWindowCommand == null)
                {
                    closeWindowCommand = new RelayCommand<Window>((w) => CloseWindow(w));
                }

                return closeWindowCommand;
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
