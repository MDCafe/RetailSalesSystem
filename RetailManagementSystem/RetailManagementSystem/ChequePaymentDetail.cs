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
    
    public partial class ChequePaymentDetail
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public Nullable<int> ChequeNo { get; set; }
        public Nullable<System.DateTime> ChequeDate { get; set; }
        public Nullable<bool> IsChequeRealised { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
    
        public virtual PaymentDetail PaymentDetail { get; set; }
    }
}
