using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Base
{
    class PurchaseViewModelbase : CommonBusinessViewModel
    {             
        protected bool _showRestrictedCompanies;

        public PurchaseViewModelbase(bool showRestrictedCompanies)
        {
            _showRestrictedCompanies = showRestrictedCompanies;
            if (_showRestrictedCompanies)
                _categoryId = Constants.COMPANIES_OTHERS;
            else
                _categoryId = Constants.COMPANIES_MAIN;
        }        
    }
 }
