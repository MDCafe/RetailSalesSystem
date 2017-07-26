using System;
using System.Collections.Generic;
using System.Windows.Input;
using RetailManagementSystem.Model;
using RetailManagementSystem.Command;
using RetailManagementSystem.Interfaces;

namespace RetailManagementSystem.ViewModel.Base
{
    internal class CommonBusinessViewModel : DocumentViewModel, INotifier
    {
        protected IEnumerable<PaymentMode> _paymentModes;
        protected IEnumerable<ProductPrice> _productsPriceList;
        protected DateTime _transcationDate;
        protected int _runningBillNo;
        protected PaymentMode _selectedPaymentMode;
        protected char _selectedPaymentId;

        protected decimal? _totalAmount = 0;
        protected decimal? _totalDiscountPercent;
        protected decimal? _totalDiscountAmount;

        protected IEnumerable<ProductPrice> ProductsPriceList
        {
            get { return _productsPriceList; }
            private set
            {
                _productsPriceList = value;                
                RaisePropertyChanged("ProductsPriceList");
            }
        }

        protected char SelectedPaymentId
        {
            get { return _selectedPaymentId; }
            set
            {
                _selectedPaymentId = value;
                RaisePropertyChanged("SelectedPaymentId");
            }
        }

        protected DateTime TranscationDate
        {
            get { return _transcationDate; }
            set
            {
                _transcationDate = value;
                RaisePropertyChanged("SaleDate");
            }
        }

        protected int RunningBillNo
        {
            get { return _runningBillNo; }
            set
            {
                _runningBillNo = value;
                RaisePropertyChanged("RunningBillNo");
            }
        }

        protected IEnumerable<PaymentMode> PaymentModes
        {
            get { return _paymentModes; }
            private set
            {
                _paymentModes = value;
                RaisePropertyChanged("PaymentModes");
            }
        }

        protected PaymentMode SelectedPaymentMode
        {
            get { return _selectedPaymentMode; }
            set
            {
                _selectedPaymentMode = value;
                RaisePropertyChanged("SelectedPaymentMode");
            }
        }

        protected CommonBusinessViewModel()
        {
            PaymentMode pm = new PaymentMode();
            _paymentModes = pm.PaymentModes;
            _productsPriceList = RMSEntitiesHelper.Instance.GetProductPriceList();
            _transcationDate = DateTime.Now;
            SelectedPaymentId = '0';

        }
       
        void INotifier.Notify(int runningNo)
        {
            RunningBillNo = runningNo;
        }

        protected decimal? GetDiscountValue()
        {
            if (_totalDiscountAmount.HasValue)
                return _totalDiscountAmount;
            if (_totalDiscountPercent.HasValue)
                return _totalAmount * (_totalDiscountPercent / 100);
            return null;
        }

        #region IsDirty

        private bool _isDirty = false;
        override public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    RaisePropertyChanged("IsDirty");
                    RaisePropertyChanged("FileName");
                }
            }
        }

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
            var returnValue = Workspace.This.Close(this);
            if (!returnValue) return;

            RMSEntitiesHelper.Instance.RemoveNotifier(this);
        }
        #endregion
    }   

}
