namespace RetailManagementSystem.ViewModel.Sales
{
    public class SalesParams
    {
        public bool ShowAllCustomers { get; set; }
        public int? Billno { get; set; }
        public bool GetTemproaryData { get; set; }
        public bool IsTempDataWindow { get; set; }
        public bool CancelBill { get; set; }
        public int  CustomerId { get; set; }
        public string Guid { get; set; }
    }
}
