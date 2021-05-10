using System;

namespace RetailManagementSystem.Model
{
    public delegate void AmountChanged();

    public class BaseModel : BaseModelNotifier
    {
        public event AmountChanged OnAmountChanged;

        private int _billId;
        private int _productId;
        private int _priceId;
        private decimal? _sellingPrice;
        protected decimal? _qty;
        private decimal? _discount;
        private DateTime? _addedOn;
        private DateTime? _modifiedOn;
        private int? _updatedBy;
        private bool _supportsMultiplePrice;
        private DateTime? _expiryDate;
        private decimal _discountPercentage;
        private decimal _discountAmount;
        private decimal? _amount;
        private bool _propertyReadOnly;
        private int _serialNo;
        private decimal? _originalQty;

        private int _caseQuantity;
        private AmountChanged _amountChangedDelegate;

        public int UnitPerCase { get; set; }

        public int CaseQuantity
        {
            get { return _caseQuantity; }
            set
            {
                _caseQuantity = value;
                CalculateAmount();
            }
        }


        public bool SupportsMultiplePrice
        {
            get
            {
                return _supportsMultiplePrice;
            }

            set
            {
                if (_supportsMultiplePrice == value)
                {
                    return;
                }
                _supportsMultiplePrice = value;
                OnPropertyChanged(nameof(SupportsMultiplePrice));
            }
        }

        public decimal? SellingPrice
        {

            get
            {
                return _sellingPrice;
            }

            set

            {
                if (Nullable.Equals<decimal>(_sellingPrice, value))
                {
                    return;
                }
                _sellingPrice = value;
                CalculateAmount();
                this.OnPropertyChanged(nameof(SellingPrice));
            }
        }

        public decimal? Qty
        {

            get
            {
                return _qty;
            }

            set
            {
                if (Nullable.Equals<decimal>(_qty, value)) return;

                _qty = value;
                CalculateAmount();
                CalculateCaseForUnits();

                OnPropertyChanged(nameof(Qty));
            }
        }

        public int BillId
        {
            get
            {
                return _billId;
            }

            set
            {
                if (_billId == value)
                {
                    return;
                }
                _billId = value;
                OnPropertyChanged(nameof(BillId));
            }
        }

        public int ProductId
        {

            get
            {
                return _productId;
            }

            set
            {
                //if (_productId == value)
                //{
                //    return;
                //}
                _productId = value;
                OnPropertyChanged(nameof(ProductId));
            }
        }

        public int PriceId
        {

            get
            {
                return _priceId;
            }

            set
            {
                if (_priceId == value)
                {
                    return;
                }
                _priceId = value;
                OnPropertyChanged(nameof(PriceId));
            }
        }

        public decimal? Discount
        {

            get
            {
                return _discount;
            }

            set
            {
                if (Nullable.Equals<decimal>(_discount, value))
                {
                    return;
                }
                _discount = value;
                //CalculateAmount();
                OnPropertyChanged(nameof(Discount));
            }
        }

        public DateTime? AddedOn
        {

            get
            {
                return _addedOn;
            }

            set
            {
                if (Nullable.Equals<DateTime>(_addedOn, value))
                {
                    return;
                }
                _addedOn = value;
                OnPropertyChanged(nameof(AddedOn));
            }
        }

        public DateTime? ModifiedOn
        {

            get
            {
                return _modifiedOn;
            }

            set
            {
                if (Nullable.Equals<DateTime>(_modifiedOn, value))
                {
                    return;
                }
                _modifiedOn = value;
                OnPropertyChanged(nameof(ModifiedOn));
            }
        }

        public int? UpdatedBy
        {

            get
            {
                return _updatedBy;
            }

            set
            {
                if (Nullable.Equals<int>(_updatedBy, value))
                {
                    return;
                }
                _updatedBy = value;
                OnPropertyChanged(nameof(UpdatedBy));
            }
        }

        public DateTime? ExpiryDate
        {
            get
            {
                if (_expiryDate == null)
                {
                    return _expiryDate;// = DateTime.Now.AddMonths(1);
                }
                return _expiryDate;
            }

            set
            {
                //if (Nullable.Equals(_expiryDate, value))
                //{
                //    //_expiryDate = DateTime.Now.AddMonths(1);
                //}
                _expiryDate = value;
                this.OnPropertyChanged(nameof(ExpiryDate));
            }
        }

        public decimal? Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnAmountChanged?.Invoke();
            }
        }

        public decimal CostPrice { get; set; }

        public decimal DiscountPercentage { get => _discountPercentage; set { _discountPercentage = value; CalculateAmount(); CalculateCost(); } }
        public decimal DiscountAmount { get => _discountAmount; set { _discountAmount = value; CalculateAmount(); CalculateCost(); } }
        public decimal AvailableStock { get; set; }
        public bool PropertyReadOnly
        {
            get
            {
                return _propertyReadOnly;
            }

            set
            {
                _propertyReadOnly = value;
                OnPropertyChanged(nameof(PropertyReadOnly));
            }
        }

        public int SerialNo
        {
            get
            {
                return _serialNo;
            }

            set
            {
                if (_serialNo == value)
                {
                    return;
                }
                _serialNo = value;
                OnPropertyChanged(nameof(SerialNo));
            }
        }

        public decimal? OriginalQty
        {

            get
            {
                return _originalQty;
            }

            set
            {
                if (Nullable.Equals<decimal>(_originalQty, value))
                {
                    return;
                }
                _originalQty = value;
                OnPropertyChanged(nameof(OriginalQty));
            }
        }

        //public virtual void CalculateAmount()
        //{
        //    if (!SellingPrice.HasValue || !Qty.HasValue) return;
        //    var amount = SellingPrice.Value * Qty.Value;
        //    decimal discountAmount = GetDiscountAmount(amount);
        //    Amount = amount - discountAmount;
        //    Discount = discountAmount;
        //}

        public virtual void CalculateAmount()
        {
            var amount = SellingPrice * GetQty();
            var discountAmount = DiscountPercentage != 0 ?
                                 amount - (amount * (DiscountPercentage / 100)) :
                                 DiscountAmount != 0 ?
                                 amount - DiscountAmount :
                                 0;

            if (discountAmount != 0)
            {
                Amount = discountAmount;
                Discount = amount - discountAmount;
                return;
            }
            else
            {
                Discount = 0;
            }

            Amount = amount;
        }

        protected virtual void CalculateCost() { }

        protected decimal GetDiscountAmount(decimal amount)
        {
            var dispercentAmt = DiscountPercentage != 0 ? (amount * (DiscountPercentage / 100)) : 0m;
            var disAmt = DiscountAmount != 0 ? DiscountAmount : 0m;
            return dispercentAmt + disAmt;
        }

        public void SubscribeToAmountChange(AmountChanged amountChangedDelegate)
        {
            if (OnAmountChanged == null)
            {
                OnAmountChanged += amountChangedDelegate;
                _amountChangedDelegate = amountChangedDelegate;
            }
        }

        public void UnSubscribeToAmountChange()
        {
            if (OnAmountChanged == null)
                OnAmountChanged -= _amountChangedDelegate;
        }


        protected void CalculateCaseForUnits()
        {
            if (!_qty.HasValue || _qty.Value == 0 || UnitPerCase == 0) return;
            var caseAmount = Math.Truncate(_qty.Value / UnitPerCase);
            var remainingAmt = _qty.Value % UnitPerCase;
            _qty = remainingAmt;
            if (caseAmount != 0)
                CaseQuantity += Convert.ToInt32(caseAmount);
        }

        public decimal? GetQty()
        {
            var qtyValue = Qty.HasValue ? Qty : 0;
            return CaseQuantity > 0 ? (UnitPerCase * CaseQuantity) + qtyValue : qtyValue;
        }
    }
}
