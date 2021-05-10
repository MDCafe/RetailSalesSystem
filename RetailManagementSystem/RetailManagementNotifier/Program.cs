namespace RetailManagementNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            var retailManagementNotifier = new RetailManagementNotifier.RetailManagementEmailer();
            retailManagementNotifier.SendDailySalesReport(new string[2] { "2021-02-10", "2021-02-10" });
        }
    }
}
