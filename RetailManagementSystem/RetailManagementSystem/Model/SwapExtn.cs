namespace RetailManagementSystem.Model
{
    class SwapExtn : BaseModelNotifier
    {
        private decimal quantity;
        private decimal sellingPrice;

        public int ProductId { get; set; }
        public decimal SellingPrice
        {
            get => sellingPrice;
            set
            {
                if (sellingPrice == value) return;
                sellingPrice = value;
                CalculateAmount();
            }
        }
        public decimal Amount { get; set; }
        public decimal AvailableStock { get; set; }
        public decimal CostPrice { get; set; }

        public int StockId { get; set; }
        public decimal Quantity
        {
            get => quantity;
            set
            {
                if (quantity == value) return;
                quantity = value;
                CalculateAmount();
            }
        }

        private void CalculateAmount()
        {
            Amount = quantity * SellingPrice;
        }
    }
}
