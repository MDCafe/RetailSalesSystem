using System;

namespace RetailManagementSystem.Model
{
    class CustomerPaymentDetails : BaseModel
    {
        public int BillId { get; set; }
        public int RunningBillNo { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal CurrentAmountPaid { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
