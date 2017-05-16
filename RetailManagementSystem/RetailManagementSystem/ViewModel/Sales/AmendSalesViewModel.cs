using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        public int BillNo { get; set; }

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

        #region GetBill Command
        RelayCommand _clearCommand = null;

        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand((p) => OnClear());
                }

                return _clearCommand;
            }
        }

        private void OnClear()
        {
            BillNo = 0;
            SelectedCustomer = null;
            //billList = null;

        }
        #endregion
    }
}
