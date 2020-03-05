using System;

namespace RetailManagementSystem.Model
{
    class PurchaseDetailExtn : BaseModel
    {

        private decimal? _freeIssue;
        private decimal? _itemCoolieCharges;
        private decimal? _itemTransportCharges;
        private decimal? _purchasePrice;
        private decimal? _vatAmount;
        private decimal? _vatPercentage;

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
                OnPropertyChanged(nameof(PurchasePrice));
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
                this.OnPropertyChanged(nameof(FreeIssue));
            }
        }
        public decimal? OldCostPrice { get; set; }
        public decimal? OldSellingPrice { get; set; }
        public decimal? Tax { get; set; }

        public decimal? VATAmount { get => _vatAmount; set { _vatAmount = value;CalculateAmount(); CalculateCost(); } }
        public decimal? VATPercentage { get => _vatPercentage; set { _vatPercentage = value; CalculateAmount(); CalculateCost(); } }
        

        public decimal? ItemCoolieCharges
        {
            get => _itemCoolieCharges;
            set
            {
                if (value != _itemCoolieCharges)
                {
                    _itemCoolieCharges = value;
                    //CostPrice = PurchasePrice.Value + GetCoolieCharges() + GetTransportCharges();
                    //var coolieCharges = _itemCoolieCharges.HasValue ? _itemCoolieCharges.Value : 0;
                    CalculateAmount();
                    CalculateCost();
                    //Amount += coolieCharges;
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
                    //CostPrice = PurchasePrice.Value + GetCoolieCharges() + GetTransportCharges();
                    CalculateCost();
                    //CostPrice = +GetTransportCharges();
                }
            }
        }

        private decimal GetCoolieChargesPerItem()
        {
            if (!Qty.HasValue) return 0;            
            return GetCoolieCharges() / Qty.Value;
        }

        private decimal GetVATChargesPerItem()
        {
            if (!Qty.HasValue) return 0;
            return GetVatAmountPerItem();
        }

        private decimal GetCoolieCharges()
        {
            if (!Qty.HasValue) return 0;
            return _itemCoolieCharges.HasValue ? _itemCoolieCharges.Value : 0;
        }

        private decimal GetTransportCharges()
        {
            if (!Qty.HasValue) return 0;
            var transportCharges = _itemTransportCharges.HasValue ? _itemTransportCharges.Value : 0;
            return transportCharges / Qty.Value;
        }

        protected override void CalculateCost()
        {
            if (Qty == null) return;
            var totalQtyWithFreeIssue = 0.0M;
            if (_freeIssue.HasValue)
                totalQtyWithFreeIssue = Qty.HasValue ? Qty.Value + _freeIssue.Value : 0;
            else
                totalQtyWithFreeIssue = Qty.HasValue ? Qty.Value : 0;

            var amount = (PurchasePrice.Value * Qty.Value) + (GetVATChargesPerItem() * Qty.Value);

            decimal calcDiscountAmount = GetDiscountAmount(amount);
            decimal discountPerItem = calcDiscountAmount / Qty.Value;

            CostPrice = (amount / totalQtyWithFreeIssue) - (discountPerItem != 0 ? discountPerItem : 0m) +
                        GetTransportCharges() +
                        GetCoolieChargesPerItem();
                        

            //if (_itemTransportCharges.HasValue)
            //    CostPrice += GetTransportCharges();

            //if (_itemCoolieCharges.HasValue)
            //    CostPrice += GetCoolieChargesPerItem();
        }

        public override void CalculateAmount()
        {
            if (!PurchasePrice.HasValue || !Qty.HasValue) return;

            var amount = PurchasePrice.Value * Qty.Value;
            decimal discountAmount = GetDiscountAmount(amount);

            if (discountAmount != 0)
            {
                Amount = (amount + GetCoolieCharges() + GetVatAmount()) - discountAmount;
                Discount = discountAmount;
                return;
            }

            Amount = (amount + GetCoolieCharges() + GetVatAmount()) - discountAmount;
        }

        private decimal GetVatAmount()
        {
            if (!Qty.HasValue) return 0m;
            return VATAmount.HasValue && VATAmount.Value != 0 ? 
                   VATAmount.Value  : 
                   VATPercentage.HasValue && VATPercentage.Value !=0 ? ((_purchasePrice * (_vatPercentage/100)) * Qty).Value : 0m;
        }

        private decimal GetVatAmountPerItem()
        {
            if (!Qty.HasValue) return 0m;
            return VATAmount.HasValue && VATAmount.Value!=0 ? 
                    VATAmount.Value/Qty.Value : 
                    VATPercentage.HasValue && VATPercentage.Value != 0 ? ((_purchasePrice * (_vatPercentage / 100))).Value : 0m;
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
