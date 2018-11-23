namespace RetailManagementSystem.Model
{
    class ProductOrder
    {
        public string ProductName { get; set; }
        public decimal StockQuantity { get; set; }
        public string CategoryName { get; set; }
        public decimal ReorderPoint { get; set; }

        public override string ToString()
        {
            return this.ProductName + ", Stock :" + this.StockQuantity + " Re.Pt: " + this.ReorderPoint;
        }
    }
}
