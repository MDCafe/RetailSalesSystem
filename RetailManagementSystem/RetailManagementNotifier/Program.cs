namespace RetailManagementNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            var retailManagementNotifier = new RetailManagementEmailer();
            
            if(args != null && args.Length > 0)
            {
                retailManagementNotifier.SendDailySalesReport(args);
            }
            else
            {
                retailManagementNotifier.SendDailySalesReport();
            }
        }
    }
}
