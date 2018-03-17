using System;
using System.ComponentModel;

namespace RetailManagementSystem.Model
{    
    public class SaleDetailExtn : BaseModel
    {       
        private int _billId;
        private int _productId;
        private int _priceId;
        private decimal? _sellingPrice;
        private decimal? _qty;
        private decimal? _discount;
        private DateTime? _addedOn;
        private DateTime? _modifiedOn;
        private int? _updatedBy;
        //used while edit
        private decimal? _originalQty;
        private int _serialNo;
        private decimal? _lastSoldPrice;
        private bool _propertyReadOnly;
        private bool _supportsMultiplePrice;
        private DateTime? _expiryDate;

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
                OnPropertyChanged("SupportsMultiplePrice");
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
                OnPropertyChanged("SerialNo");
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
                OnPropertyChanged("BillId");
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
                OnPropertyChanged("ProductId");
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
                OnPropertyChanged("PriceId");
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
                this.OnPropertyChanged("SellingPrice");
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
                if (Nullable.Equals<decimal>(_qty, value))
                {
                    return;
                }
                _qty = value;
                OnPropertyChanged("Qty");
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
                OnPropertyChanged("OriginalQty");
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
                OnPropertyChanged("Discount");
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
                OnPropertyChanged("AddedOn");
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
                OnPropertyChanged("ModifiedOn");
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
                OnPropertyChanged("UpdatedBy");
            }
        }

        public virtual Product Product { get; set; }

        public virtual Sale Sale { get; set; }

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
                this.OnPropertyChanged("ExpiryDate");
            }
        }

      

        public decimal CostPrice { get; set; }
        public decimal? Amount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal AvailableStock { get; set; }
        public decimal? LastSoldPrice
        {
            get
            {
                return _lastSoldPrice;
            }

            set
            {
                _lastSoldPrice = value;
            }
        }
        public bool PropertyReadOnly
        {
            get
            {
                return _propertyReadOnly;
            }

            set
            {
                _propertyReadOnly = value;
                OnPropertyChanged("PropertyReadOnly");
            }
        }
    }

    public  class ReturnSaleDetailExtn : SaleDetailExtn
    {
        public decimal ReturnQty { get; set; }
        public decimal ReturnPrice { get; set; }
        public bool Selected { get; set; }
        public string ProductName { get; set; }
        public decimal ReturnAmount { get; set; }
    }
}
