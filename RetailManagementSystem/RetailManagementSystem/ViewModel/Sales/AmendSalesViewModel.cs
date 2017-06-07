using System.Linq;
using System.Windows;
using System.Windows.Input;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using System.Collections.Generic;

namespace RetailManagementSystem.ViewModel.Sales
{
    class AmendSalesViewModel : ViewModelBase
    {
        bool _showAllCustomers;
        int _othersCategoryId;
        int _categoryId;
        Customer _selectedCustomer;
        RMSEntities _rmsEntities;
        Category _category = null;
        string _selectedCustomerText;
        IEnumerable<Sale> _billList;
        
        public int? BillNo { get; set; }
        public string BillNoText { get; set; }

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                if (_showAllCustomers)
                    return _rmsEntities.Customers.Local;
                return _rmsEntities.Customers.Local.Where(c => c.CustomerTypeId != _othersCategoryId);
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

        public AmendSalesViewModel()
        {
            //show all the customers since it is pwd protected 
            _showAllCustomers = true;
            _rmsEntities = new RMSEntities();
            _rmsEntities.Customers.ToList();           

            var othersCategory = _rmsEntities.Categories.FirstOrDefault(c => c.name == Constants.CUSTOMERS_OTHERS);
            _othersCategoryId = othersCategory.Id;
            
            if (_showAllCustomers)
                _categoryId = _othersCategoryId;
            else
            {
                _category = _rmsEntities.Categories.FirstOrDefault(c => c.name == Constants.CUSTOMERS_HOTEL);
                _categoryId = _category.Id;
            }
            
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
            SelectedCustomer = null;
            BillList = null;
        }
        #endregion

        #region Print Command
        RelayCommand<object> _printCommand = null;
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new RelayCommand<object>((p) => OnPrint(), (p) => CanExecuteMethod(p));
                }

                return _printCommand;
            }
        }

        private void OnPrint()
        {
            
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
            var billExisits = _rmsEntities.Sales.Any(b => b.RunningBillNo == BillNo);
            if (!billExisits)
            {
                Utility.ShowErrorBox(window,"Bill Number doesn't exist");
                return;
            }

            View.Entitlements.Login login = new View.Entitlements.Login(true);
            var result = login.ShowDialog();
            if (!result.Value)
            {
                //Utility.ShowErrorBox(window, "Invalid UserId or Password");
                return;
            }

            var saleParams = new SalesParams() { Billno = BillNo };            

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

            BillList = _rmsEntities.Sales.Local.Where(s => s.CustomerId == SelectedCustomer.Id);                       
        }


        #endregion
    }
}
