﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using RetailManagementSystem.Model;
using RetailManagementSystem.Command;
using RetailManagementSystem.Interfaces;
using System.Collections.ObjectModel;

namespace RetailManagementSystem.ViewModel.Base
{
    internal class CommonBusinessViewModel : DocumentViewModel, INotifier
    {
        public delegate void INotifierCollectionChanged();
        public event INotifierCollectionChanged notifierCollectionChangedEvent;

        protected IEnumerable<PaymentMode> _paymentModes;
        protected ObservableCollection<ProductPrice> _productsPriceList;
        protected DateTime _transcationDate;
        protected int _runningBillNo;
        protected PaymentMode _selectedPaymentMode;
        protected char _selectedPaymentId;                
        protected decimal? _totalAmount = 0;
        protected decimal? _totalDiscountPercent;
        protected decimal? _totalDiscountAmount;
        protected decimal _amountPaid = 0.0M;
        protected bool _discountEnabled = true, _discountPercentEnabled = true, _isEditMode;
        protected int _categoryId;

        System.Windows.Visibility _isVisible = System.Windows.Visibility.Collapsed;

        public ObservableCollection<ProductPrice> ProductsPriceList
        {
            get { return _productsPriceList; }
            set
            {
                _productsPriceList = value;                
                RaisePropertyChanged("ProductsPriceList");
            }
        }

        public char SelectedPaymentId
        {
            get { return _selectedPaymentId; }
            set
            {
                _selectedPaymentId = value;
                RaisePropertyChanged("SelectedPaymentId");
            }
        }

        public DateTime TranscationDate
        {
            get { return _transcationDate; }
            set
            {
                _transcationDate = value;
                RaisePropertyChanged("SaleDate");
            }
        }

        public int RunningBillNo
        {
            get { return _runningBillNo; }
            set
            {
                _runningBillNo = value;
                RaisePropertyChanged("RunningBillNo");
            }
        }

        public IEnumerable<PaymentMode> PaymentModes
        {
            get { return _paymentModes; }
            private set
            {
                _paymentModes = value;
                RaisePropertyChanged("PaymentModes");
            }
        }

        public PaymentMode SelectedPaymentMode
        {
            get { return _selectedPaymentMode; }
            set
            {
                _selectedPaymentMode = value;
                RaisePropertyChanged("SelectedPaymentMode");
            }
        }

        public bool DiscountEnabled
        {
            get { return _discountEnabled; }
            set
            {
                _discountEnabled = value;
                RaisePropertyChanged("DiscountEnabled");
            }
        }

        public bool DiscountPercentEnabled
        {
            get { return _discountPercentEnabled; }
            set
            {
                _discountPercentEnabled = value;
                RaisePropertyChanged("DiscountPercentEnabled");
            }
        }

        public decimal AmountPaid
        {
            get { return _amountPaid; }
            set
            {
                _amountPaid = value;
                RaisePropertyChanged("AmountPaid");
                RaisePropertyChanged("BalanceAmount");
            }
        }

        public decimal BalanceAmount
        {
            get { return Math.Abs(_amountPaid != 0 ? _totalAmount.Value - _amountPaid : 0.00M); }
        }

        public bool IsEditMode
        {
            get { return _isEditMode; }
        }

        public bool NegateIsEditMode
        {
            get { return !_isEditMode; }
        }

        public System.Windows.Visibility IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                _isVisible = value;
                RaisePropertyChanged("VisibIsVisiblele");
            }
        }

        public Customer DefaultCustomer { get; set; }

        protected CommonBusinessViewModel()
        {
            PaymentMode pm = new PaymentMode();
            _paymentModes = pm.PaymentModes;
            //_productsPriceList = RMSEntitiesHelper.Instance.GetProductPriceList();
            _transcationDate = DateTime.Now;
            SelectedPaymentId = '0';
            RefreshProductList();
        }
       
        void INotifier.Notify(int runningNo,int categoryId)
        {
            //RefreshProductList();
            if (categoryId != _categoryId) return;
            RunningBillNo = runningNo;
        }

        void INotifier.NotifyPurchaseUpdate()
        {
            RefreshProductList(); 
        }

        protected decimal? GetDiscountValue()
        {
            if (_totalDiscountAmount.HasValue && _totalDiscountAmount.Value > 0)
                return _totalDiscountAmount;
            if (_totalDiscountPercent.HasValue && _totalDiscountPercent.Value > 0)
                return _totalAmount * (_totalDiscountPercent / 100);
            return null;
        }


        public void RefreshProductList()
        { 
            _productsPriceList =  RMSEntitiesHelper.Instance.GetProductPriceList();
            RaisePropertyChanged("ProductsPriceList");
            notifierCollectionChangedEvent?.Invoke();
        }

        #region CloseCommand                
        override protected bool OnClose()
        {
            var returnValue = Workspace.This.Close(this);
            if (!returnValue) return returnValue;

            RMSEntitiesHelper.Instance.RemovePurchaseNotifier(this);
            return true;
        }

        #endregion
    }   

}
