using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Stocks
{
    class StockTransactionsViewModel : DocumentViewModel
    {
        CodeMaster _selectedSwapMode;
        RMSEntities _rmsEntities;

        IEnumerable<CodeMaster> _swapsCodeList;

        public CodeMaster SelectedSwapMode
        {
            get
            {
                return _selectedSwapMode;
            }

            set
            {
                _selectedSwapMode = value;
                RaisePropertyChanged("SelectedSwapMode");
            }
        }

        public IEnumerable<CodeMaster> SwapsCodeList
        {
            get
            {
                if (_swapsCodeList == null)
                    return _rmsEntities.CodeMasters.Local.Where(c => c.Code == "SWP");
                return _swapsCodeList;
            }

            set
            {
                _swapsCodeList = value;
            }
        }

        public StockTransactionsViewModel()
        {
            _rmsEntities = new RMSEntities();
            var cnt =_rmsEntities.CodeMasters.ToList().Count();

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
