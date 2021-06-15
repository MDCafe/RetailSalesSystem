using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Base
{
    public class ReportBaseViewModel : DocumentViewModel
    {
        private int _categoryId;
        private bool _showRestrictedPeople;

        public int CategoryId { get { return _categoryId; } set { _categoryId = value; } }
        public bool ShowRestrictedPeople { get { return _showRestrictedPeople; } set { _showRestrictedPeople = value; } }

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
