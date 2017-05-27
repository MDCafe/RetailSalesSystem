using System.Windows.Input;
using System.Linq;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System;

namespace RetailManagementSystem.ViewModel.Masters
{
    class CustomerViewModel : DocumentViewModel
    {
        public CustomerViewModel()
        {
            var names = RMSEntitiesHelper.RMSEntities.Customers.SelectMany(c => c.Name);
        }


        #region Public Variables

        //public  CustomerNames { get; set; }

        #endregion

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

        private void OnClose()
        {
            Workspace.This.Close(this);
        }
        #endregion

        #region SaveCommand
        RelayCommand<object> _saveCommand = null;
        override public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand<object>((p) => OnSave(p), (p) => CanSave(p));
                }

                return _saveCommand;
            }
        }

        public override bool IsDirty
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool CanSave(object parameter)
        {
            //return _selectedCustomer != null && _selectedCustomer.Id != 0 && _salesDetailsList.Count != 0 &&
            //        _salesDetailsList[0].ProductId != 0 && _selectedCustomerText == _selectedCustomer.Name;
            //return IsDirty;
            return true;
        }

        private void OnSave(object parameter)
        {
            
        }        

        #endregion
    }
}
