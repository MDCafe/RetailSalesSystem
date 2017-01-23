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
    
    public partial class Company
    {
        public Company()
        {
            this.Products = new HashSet<Product>();
            this.Purchases = new HashSet<Purchase>();
            this.PurchasePaymentDetails = new HashSet<PurchasePaymentDetail>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Nullable<int> MobileNo { get; set; }
        public Nullable<int> LanNo { get; set; }
        public string Email { get; set; }
        public string VATNo { get; set; }
        public bool IsSupplier { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<PurchasePaymentDetail> PurchasePaymentDetails { get; set; }
    }
}
