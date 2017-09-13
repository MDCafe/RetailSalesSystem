//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RetailManagementSystem
{
    using System;
    using System.Collections.Generic;
    
    public partial class PriceDetail
    {
        public PriceDetail()
        {
            this.SaleDetails = new HashSet<SaleDetail>();
            this.PurchaseDetails = new HashSet<PurchaseDetail>();
            this.Stocks = new HashSet<Stock>();
        }
    
        public int PriceId { get; set; }
        public int BillId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public decimal SellingPrice { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual ICollection<SaleDetail> SaleDetails { get; set; }
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
