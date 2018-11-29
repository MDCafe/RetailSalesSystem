using System;

namespace RetailManagementSystem.Model.Graphs
{
    public class SalesGraphCustomModel
    {
        public decimal CashSales { get; set; }
        public decimal CreditSales { get; set; }
        public decimal HotelSales { get; set; }
        public double TotalSales { get; set; }
        public string SaleYearMonth { get; set; }
    }
}
