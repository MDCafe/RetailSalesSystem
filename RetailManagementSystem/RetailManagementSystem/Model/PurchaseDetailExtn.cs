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

        public decimal? VATAmount { get => _vatAmount; set { _vatAmount = value; CalculateAmount(); CalculateCost(); } }
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
            var qty = GetQty();
            if (!qty.HasValue || qty == 0) return 0;
            return GetCoolieCharges() / qty.Value;
        }

        private decimal GetVATChargesPerItem()
        {
            if (!Qty.HasValue) return 0;
            return GetVatAmountPerItem();
        }

        private decimal GetCoolieCharges()
        {
            var qty = GetQty();
            if (!qty.HasValue || qty == 0) return 0;
            return _itemCoolieCharges ?? 0;
        }

        private decimal GetTransportCharges()
        {
            var qty = GetQty();

            if (!qty.HasValue || qty == 0) return 0;
            var transportCharges = _itemTransportCharges ?? 0;
            return transportCharges / qty.Value;
        }

        protected override void CalculateCost()
        {
            var qty = GetQty();

            if (qty == null || qty == 0) return;
            decimal totalQtyWithFreeIssue;
            if (_freeIssue.HasValue)
                totalQtyWithFreeIssue = qty.HasValue ? qty.Value + _freeIssue.Value : 0;
            else
                totalQtyWithFreeIssue = qty ?? 0;

            var amount = (PurchasePrice.Value * qty.Value) + (GetVATChargesPerItem() * qty.Value);

            decimal calcDiscountAmount = GetDiscountAmount(amount);
            decimal discountPerItem = calcDiscountAmount / qty.Value;

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
            if (!PurchasePrice.HasValue || (!Qty.HasValue && CaseQuantity == 0)) return;

            var qty = GetQty();
            var amount = PurchasePrice.Value * qty.Value;
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
            var qty = GetQty();
            if (qty == null || qty == 0) return 0m;

            return VATAmount.HasValue && VATAmount.Value != 0 ?
                   VATAmount.Value :
                   VATPercentage.HasValue && VATPercentage.Value != 0 ? ((_purchasePrice * (_vatPercentage / 100)) * Qty).Value : 0m;
        }

        private decimal GetVatAmountPerItem()
        {
            var qty = GetQty();
            if (qty == null || qty == 0) return 0m;
            return VATAmount.HasValue && VATAmount.Value != 0 ?
                    VATAmount.Value / Qty.Value :
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
