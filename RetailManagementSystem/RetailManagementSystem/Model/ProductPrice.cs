﻿namespace RetailManagementSystem.Model
{
    public class ProductPrice
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Quantity { get; set; }
        public int PriceId { get; set; }
        public string ExpiryDate { get; set; }
        public bool SupportsMultiplePrice { get; set; }
        public long? BarCodeNo { get; set; }
        public int UnitOfMeasure { get; set; }

        public int? UnitPerCase { get; set; }

        //This is to satisfy the XML binding Error
        //public bool PropertyReadOnly { get {return true; } set {; } }

        
    }
}
