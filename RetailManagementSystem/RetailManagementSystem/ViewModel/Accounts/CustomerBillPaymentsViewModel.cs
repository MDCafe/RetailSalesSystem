using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Accounts
{
    class CustomerBillPaymentsViewModel : DocumentViewModel
    {
        private bool _showRestrictedCustomer;
        private int _categoryId;

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                using (var rmsEntities = new RMSEntities())
                {
                    return rmsEntities.Customers.ToList().Where(c => c.CustomerTypeId == _categoryId).OrderBy(a => a.Name);
                }
            }
        }

        public CustomerBillPaymentsViewModel(bool showRestrictedCustomer)
        {
            _showRestrictedCustomer = showRestrictedCustomer;
            if (_showRestrictedCustomer)
                _categoryId = Constants.CUSTOMERS_OTHERS;
            else
                _categoryId = Constants.CUSTOMERS_HOTEL;

            Title = "Customer Bill Payment";
        }


        private void LoadDataGrid()
        {

        }

        #region CloseCommand
        RelayCommand<object> _closeCommand = null;
        override public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand<object>((p) => OnClose(), (p) => CanClose());
                }

                return _closeCommand;
            }
        }

        private bool CanClose()
        {
            return true;
        }

        protected void OnClose()
        {
            Workspace.This.Close(this);
        }

        #endregion
    }
}
