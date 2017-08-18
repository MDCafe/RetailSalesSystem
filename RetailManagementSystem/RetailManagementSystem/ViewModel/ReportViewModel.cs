using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel
{
    internal class ReportViewModel : ReportBaseViewModel
    {

        public ReportViewModel(bool showResctricteCustomers, string title) : base(showResctricteCustomers)
        {
            this.Title = title;
        }


        #region Clear Command
        RelayCommand<object> _clearCommand = null;


        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand<object>((p) => Clear());
                }

                return _clearCommand;
            }
        }

        private void Clear()
        {
            //_returnPurchaseDetailsList = new ObservableCollection<ReturnPurchaseDetailExtn>();
            //RaisePropertyChanged("ReturnSaleDetailList");
            //BillNo = null;
            //IsBillBasedReturn = false;
            //TotalAmount = null;
            ////sCompanyName = "";
            //ModeOfPayment = "";
            //PurchaseDate = null;
        }

        #endregion
    }
}
