using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Base
{
    class ReportBaseViewModel :  DocumentViewModel
    {
        protected int _categoryId;
        protected bool _showRestrictedPeople;

        public ReportBaseViewModel(bool isSupplier, bool showRestrictedPeople)
        {
            _showRestrictedPeople = showRestrictedPeople;
            if (!isSupplier)
            {
                if (_showRestrictedPeople)
                    _categoryId = Constants.CUSTOMERS_OTHERS;
                else
                    _categoryId = Constants.CUSTOMERS_HOTEL;

                return;
            }

            if (_showRestrictedPeople)
                _categoryId = Constants.COMPANIES_OTHERS;
            else
                _categoryId = Constants.COMPANIES_MAIN;
        }
    }
}
