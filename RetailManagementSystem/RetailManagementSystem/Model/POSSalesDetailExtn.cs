using System;

namespace RetailManagementSystem.Model
{
    class POSSalesDetailExtn : SaleDetailExtn
    {
        public int? EmptyBottleQty { get; set; }
        public int? EmptyProductValue { get; set; }

        public new decimal? Qty
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

        public override void CalculateAmount()
        {
            if (EmptyProductValue.HasValue)
            {
                var emptyValue = EmptyProductValue.HasValue ? EmptyProductValue : 0;
                var emptyQtyValue = EmptyBottleQty.HasValue ? EmptyBottleQty : 0;

                Amount = ((SellingPrice * Qty) + (Qty * emptyValue)
                         -
                         (emptyQtyValue * emptyValue));
            }
            else
                Amount = SellingPrice * GetQty();
        }
    }
}
