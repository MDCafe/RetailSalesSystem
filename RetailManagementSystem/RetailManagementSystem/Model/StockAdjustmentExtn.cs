using System;

namespace RetailManagementSystem.Model
{
    public class StockAdjustmentExtn : BaseModelNotifier
    {
        private decimal? adjustedQty;

        public Nullable<int> StockId { get; set; }
        public Nullable<decimal> OpeningBalance { get; set; }
        public Nullable<decimal> AdjustedQty
        {
            get => adjustedQty;
            set
            {
                if (adjustedQty == value) return;
                adjustedQty = value;
                ClosingBalance = OpeningBalance - adjustedQty;
                OnPropertyChanged(nameof(ClosingBalance));
            }
        }

        public Nullable<decimal> ClosingBalance { get; set; }
        public Nullable<decimal> CostPrice { get; set; }

        public int ProductId { get; set; }
    }
}
