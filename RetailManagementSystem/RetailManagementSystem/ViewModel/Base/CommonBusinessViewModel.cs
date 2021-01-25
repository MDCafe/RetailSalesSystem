using RetailManagementSystem.Interfaces;
using RetailManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RetailManagementSystem.ViewModel.Base
{
    internal class CommonBusinessViewModel : DocumentViewModel, INotifier
    {
        public delegate void INotifierCollectionChanged();
        public event INotifierCollectionChanged NotifierCollectionChangedEvent;

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
        private IEnumerable<BankBranchDetail> bankBranchDetailList;

        public ObservableCollection<ProductPrice> ProductsPriceList
        {
            get { return _productsPriceList; }
            set
            {
                _productsPriceList = value;
                RaisePropertyChanged(nameof(ProductsPriceList));
            }
        }

        public char SelectedPaymentId
        {
            get { return _selectedPaymentId; }
            set
            {
                _selectedPaymentId = value;
                if (_selectedPaymentId == '2')
                {                                        
                    if (BankDetailList == null)
                    {
                        using (var en = new RMSEntities())
                        {
                            BankDetailList = en.BankDetails.OrderBy(b => b.Name).ToList();
                        }
                    }
                }                
                RaisePropertyChanged(nameof(SelectedPaymentId));
            }
        }

        #region ChequeDetails
        int? _selectedChqBank;
        public decimal? _chqAmount { get; set; }
        public decimal? ChqAmount { get { return _chqAmount; } set { _chqAmount = value; } }
        public int? ChqNo { get; set; }
        public DateTime? ChqDate { get; set; }
        public int? SelectedChqBank
        {
            get { return _selectedChqBank; }
            set
            {

                if (_selectedChqBank == value) return;
                _selectedChqBank = value;
                using (var en = new RMSEntities())
                {
                    BankBranchDetailList = en.BankBranchDetails.Where(b => b.BankId == value).OrderBy(o => o.Name).ToList();
                };
            }
        }
        public int? SelectedChqBranch { get; set; }

        #endregion

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
                RaisePropertyChanged(nameof(RunningBillNo));
            }
        }

        public IEnumerable<PaymentMode> PaymentModes
        {
            get { return _paymentModes; }
            private set
            {
                _paymentModes = value;
                RaisePropertyChanged(nameof(PaymentModes));
            }
        }

        public PaymentMode SelectedPaymentMode
        {
            get { return _selectedPaymentMode; }
            set
            {
                _selectedPaymentMode = value;
                RaisePropertyChanged(nameof(SelectedPaymentMode));
            }
        }

        public bool DiscountEnabled
        {
            get { return _discountEnabled; }
            set
            {
                _discountEnabled = value;
                RaisePropertyChanged(nameof(DiscountEnabled));
            }
        }

        public bool DiscountPercentEnabled
        {
            get { return _discountPercentEnabled; }
            set
            {
                _discountPercentEnabled = value;
                RaisePropertyChanged(nameof(DiscountPercentEnabled));
            }
        }

        public decimal AmountPaid
        {
            get { return _amountPaid; }
            set
            {
                _amountPaid = value;
                RaisePropertyChanged(nameof(AmountPaid));
                RaisePropertyChanged(nameof(BalanceAmount));
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
                RaisePropertyChanged(nameof(IsVisible));
            }
        }

        public Customer DefaultCustomer { get; set; }

        public IEnumerable<BankDetail> BankDetailList { get; set; }
        public IEnumerable<BankBranchDetail> BankBranchDetailList
        {
            get => bankBranchDetailList;
            set 
            { 
                bankBranchDetailList = value; 
            }
        }

        protected CommonBusinessViewModel()
        {
            PaymentMode pm = new PaymentMode();
            _paymentModes = pm.PaymentModes;
            //_productsPriceList = RMSEntitiesHelper.Instance.GetProductPriceList();
            _transcationDate = RMSEntitiesHelper.Instance.GetSystemDBDate();
            SelectedPaymentId = '0';
            RefreshProductList();
            ChqDate = RMSEntitiesHelper.GetServerDate();
        }

        void INotifier.Notify(int runningNo, int categoryId)
        {
            //RefreshProductList();
            if (categoryId != _categoryId) return;
            RunningBillNo = runningNo;
        }

        void INotifier.NotifyPurchaseUpdate()
        {
            RefreshProductList();
        }


        void INotifier.NotifyStockUpdate()
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
            _productsPriceList = RMSEntitiesHelper.GetProductPriceList();
            RaisePropertyChanged(nameof(ProductsPriceList));
            NotifierCollectionChangedEvent?.Invoke();
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
