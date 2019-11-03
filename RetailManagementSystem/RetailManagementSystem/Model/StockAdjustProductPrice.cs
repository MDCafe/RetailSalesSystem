namespace RetailManagementSystem.Model
{
    class StockAdjustProductPrice
    {        
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int StockId { get; set; }
        public decimal Quantity { get; set; }
        public decimal SellingPrice { get; set; }
    }
}
