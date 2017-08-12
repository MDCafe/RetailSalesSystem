using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RetailManagementSystem.Utilities
{
    public static class Utility
    {
        public static void ShowMessageBox(Window window, string message)
        {
            MessageBox.Show(window, message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Information);
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

        public static MessageBoxResult ShowMessageBoxWithOptions(string message)
        {
            return MessageBox.Show(message, Constants.APPLICATION_NAME, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }
      
    }
}
