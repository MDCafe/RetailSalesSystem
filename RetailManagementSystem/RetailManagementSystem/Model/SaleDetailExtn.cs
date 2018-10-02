using System;

namespace RetailManagementSystem.Model
{
    public class SaleDetailExtn : BaseModel
    {
        //used while edit
       
        private decimal? _lastSoldPrice;

        public virtual Product Product { get; set; }

        public virtual Sale Sale { get; set; }

        public decimal? LastSoldPrice
        {
            get
            {
                return _lastSoldPrice;
            }

            set
            {
                _lastSoldPrice = value;
            }
        }
      

        //public decimal? PriceToCalculate { get => _priceToCalculate; set => _priceToCalculate = value; }

        public void Clear()
        {
            //this.ProductId = -1;
            //this.PriceId = -1;
            this.Amount = null;
            this.AvailableStock = 0;
            this.CostPrice = 0;
            this.SellingPrice = null;
            this.Discount = null;
            this.DiscountAmount = 0;
            this.DiscountPercentage = 0;
            this.LastSoldPrice = null;
            this.Qty = null;
        }

        
    }

    public  class ReturnSaleDetailExtn : SaleDetailExtn
    {
        public decimal ReturnQty { get; set; }
        public decimal ReturnPrice { get; set; }
        public bool Selected { get; set; }
        public string ProductName { get; set; }
        public decimal ReturnAmount { get; set; }
    }
}
