namespace RetailManagementSystem.Model
{
    public class ProductPrice
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Quantity { get; set; }
        public int PriceId { get; set; }
    }
}
