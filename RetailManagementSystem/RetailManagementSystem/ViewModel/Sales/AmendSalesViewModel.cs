using RetailManagementSystem.Command;
using RetailManagementSystem.Exceptions;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

        public AmendSalesViewModel(bool showAllCustomers)
        {
            _rmsEntities = new RMSEntities();
            var cnt = _rmsEntities.Customers.ToList();

            var othersCategory = _rmsEntities.Categories.FirstOrDefault(c => c.name == Constants.CUSTOMERS_OTHERS);
            _othersCategoryId = othersCategory.Id;
            _showAllCustomers = showAllCustomers;

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
            //billList = null;
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
                MessageBox.Show("Bill Number doesn't exist");
                return;
            }

            var saleParams = new SalesParams() { Billno = BillNo };
            Workspace.This.OpenSalesEntryCommand.Execute(saleParams);
            _closeWindowCommand.Execute(window);
        }       

        private bool CanExecuteMethod(object parameter)
        {
            return BillNo != null || SelectedCustomer != null;
        }
        #endregion

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

        //CloseWindowCommand = new RelayCommand<Window>(this.CloseWindow);

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
    }
}
