using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RetailManagementSystem.Utilities
{
    static class Constants
    {
        public const string AMOUNT = "Amount";
        public const string CUSTOMERS_OTHERS = "Other Customers";
        public const string CUSTOMERS_HOTEL = "Hotels";
        public const string APPLICATION_NAME = "Retail Management System";
        //public const string DISCOUNT_PERCENT = "DiscountPercentage";
        //public const string DISCOUNT_AMT = "DiscountAmount";
    }

    public static class Utility
    {
        public static void ShowMessageBox(Window window,string message)
        {
            MessageBox.Show(window, message, Constants.APPLICATION_NAME,MessageBoxButton.OK,MessageBoxImage.Information);
        }

        public static void ShowMessageBox(string message)
        {
            MessageBox.Show(message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarningBox(Window window, string message)
        {
            MessageBox.Show(window, message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowWarningBox(string message)
        {
            MessageBox.Show(message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowErrorBox(Window window, string message)
        {
            MessageBox.Show(window, message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowErrorBox(string message)
        {
            MessageBox.Show(message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
