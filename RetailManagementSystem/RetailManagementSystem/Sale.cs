//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RetailManagementSystem
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sale
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sale()
        {
            this.SaleDetails = new HashSet<SaleDetail>();
            this.PaymentDetails = new HashSet<PaymentDetail>();
        }
    
        public int BillId { get; set; }
        public int CustomerId { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> TransportCharges { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<bool> IsCancelled { get; set; }
        public string CustomerOrderNo { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public int RunningBillNo { get; set; }
        public string PaymentMode { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SaleDetail> SaleDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentDetail> PaymentDetails { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
