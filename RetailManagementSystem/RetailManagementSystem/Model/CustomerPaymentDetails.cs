using System;

namespace RetailManagementSystem.Model
{
    class CustomerPaymentDetails : BaseModel
    {
        public int SerialNo { get; set; }
        public int BillId { get; set; }
        public int RunningBillNo { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal CurrentAmountPaid { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime AddedOn { get; set; }
        public CodeMaster PaymentMode { get; set; }
        public int? ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public bool IsChequeRealised { get; set; }
    }
}
