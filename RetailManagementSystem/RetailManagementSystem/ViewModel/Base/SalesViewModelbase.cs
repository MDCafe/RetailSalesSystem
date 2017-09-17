using System;
using RetailManagementSystem.Interfaces;
using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Base
{
    class SalesViewModelbase : CommonBusinessViewModel
    {
        protected bool _showRestrictedCustomer;

        public SalesViewModelbase(bool showRestrictedCustomer)
        {
            _showRestrictedCustomer = showRestrictedCustomer;
            if (_showRestrictedCustomer)
                _categoryId = Constants.CUSTOMERS_OTHERS;
            else
                _categoryId = Constants.CUSTOMERS_HOTEL;
        }       
    }
}
