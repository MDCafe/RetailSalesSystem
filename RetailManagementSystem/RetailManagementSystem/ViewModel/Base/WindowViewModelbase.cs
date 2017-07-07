using RetailManagementSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.ViewModel.Base
{
    class WindowViewModelbase : ViewModelBase
    {
        protected int _categoryId;
        protected bool _showRestrictedCustomer;

        public WindowViewModelbase(bool showRestrictedCustomer)
        {
            _showRestrictedCustomer = showRestrictedCustomer;
            if (_showRestrictedCustomer)
                _categoryId = Constants.CUSTOMERS_OTHERS;
            else
                _categoryId = Constants.CUSTOMERS_HOTEL;
        }

    }
}
