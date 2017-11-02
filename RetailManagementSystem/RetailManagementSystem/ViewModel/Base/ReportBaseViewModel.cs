using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
            var returnValue = Workspace.This.Close(this);
            //if (!returnValue) return;
        }
        #endregion
    }
}
