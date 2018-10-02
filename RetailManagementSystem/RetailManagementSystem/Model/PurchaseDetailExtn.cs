using System;

namespace RetailManagementSystem.Model
{
    class PurchaseDetailExtn : BaseModel
    {
        
        private decimal? _freeIssue;
        private decimal? _itemCoolieCharges;
        private decimal? _itemTransportCharges;
        private decimal? _purchasePrice;

        public decimal? PurchasePrice
        {
            get
            {
                return _purchasePrice;
            }

            set
            {
                if (Nullable.Equals<decimal>(_purchasePrice, value))
                {
                    return;
                }
                _purchasePrice = value;
                //PriceToCalculate = _purchasePrice;
                CalculateAmount();
                CalculateCost();
                OnPropertyChanged("PurchasePrice");
            }
        }
        public decimal? FreeIssue
        {
            get
            {
                return _freeIssue;
            }

            set
            {
                if (Nullable.Equals<decimal>(_freeIssue, value))
                {
                    return;
                }
                _freeIssue = value;
                CalculateCost();
                this.OnPropertyChanged("FreeIssue");
            }
        }
        public decimal? OldCostPrice { get; set; }
        public decimal? OldSellingPrice { get; set; }
        public decimal? Tax { get; set; }
        public decimal? VATAmount { get; set; }
        public decimal? ItemCoolieCharges { get => _itemCoolieCharges;
            set
            {
                if (value != _itemCoolieCharges)
                {
                    _itemCoolieCharges = value;
                    CostPrice = PurchasePrice.Value + GetCoolieCharges() + GetTransportCharges();
                    var coolieCharges = _itemCoolieCharges.HasValue ? _itemCoolieCharges.Value : 0;
                    CalculateAmount();
                    Amount += coolieCharges;
                }
            }
        }
        public decimal? ItemTransportCharges
        {
            get => _itemTransportCharges;
            set
            {
                if (value != _itemTransportCharges)
                {
                    _itemTransportCharges = value;
                    CostPrice = PurchasePrice.Value + GetCoolieCharges() + GetTransportCharges();
                }
            }
        }

        private decimal GetCoolieCharges()
        {
            var coolieCharges = _itemCoolieCharges.HasValue ? _itemCoolieCharges.Value : 0;
            return coolieCharges / Qty.Value;
        }

        private decimal GetTransportCharges()
        {
            var transportCharges = _itemTransportCharges.HasValue ? _itemTransportCharges.Value : 0;
            return transportCharges / Qty.Value;
        }

        public void CalculateCost()
        {
            if (Qty == null) return;
            var totalQtyWithFreeIssue = 0.0M;
            if (_freeIssue.HasValue)
                totalQtyWithFreeIssue = Qty.HasValue ? Qty.Value + _freeIssue.Value : 0;
            else
                totalQtyWithFreeIssue = Qty.HasValue ? Qty.Value : 0;

            CostPrice = Amount.Value / totalQtyWithFreeIssue;
        }

        public override void CalculateAmount()
        {
            var amount = PurchasePrice * Qty;
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

            Amount = amount;
        }
    }

    class ReturnPurchaseDetailExtn : PurchaseDetailExtn
    {
        public decimal ReturnQty { get; set; }
        public decimal ReturnPrice { get; set; }
        public bool Selected { get; set; }
        public string ProductName { get; set; }
        public decimal ReturnAmount { get; set; }
        public CodeMaster SelectedReturnReason { get; set; }
        public string Comments { get; set; }
    }
}
