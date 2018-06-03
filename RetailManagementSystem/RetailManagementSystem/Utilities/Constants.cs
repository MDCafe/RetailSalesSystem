using System.Collections.Generic;
using System.Collections.Specialized;

namespace RetailManagementSystem.Utilities
{
    static class Constants
    {
        public const string AMOUNT = "Amount";
        public const string RETURN_QTY = "ReturnQty";
        public const int CUSTOMERS_OTHERS = 9;        
        public const int CUSTOMERS_HOTEL = 7;
        public const int COMPANIES_MAIN = 10;
        public const int COMPANIES_OTHERS = 11;
        public const string APPLICATION_NAME = "Retail Management System";
        public const string FREE_ISSUE = "FreeIssue";
        public const string PURCHASE_PRICE = "PurchasePrice";
        //public const string DISCOUNT_PERCENT = "DiscountPercentage";
        //public const string DISCOUNT_AMT = "DiscountAmount";
    }

    class BooleanValue
    {
        public List<KeyValuePair<bool, string>> BooleanValues { get; private set; }
        


        public BooleanValue()
        {
            BooleanValues = new List<KeyValuePair<bool, string>>()
            {
                new KeyValuePair<bool, string>(true,"Yes"),
                new KeyValuePair<bool, string>(false,"No")
            };
        }
    }  
}
