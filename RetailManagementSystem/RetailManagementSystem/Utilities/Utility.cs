using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace RetailManagementSystem.Utilities
{
    public static class Utility
    {
        public static void ShowMessageBox(Window window, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(window, message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Information);

            });
        }

        public static void ShowMessageBox(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(Application.Current.MainWindow, message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        public static void ShowWarningBox(Window window, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(window, message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        }

        public static void ShowWarningBox(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(Application.Current.MainWindow, message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        }

        public static void ShowErrorBox(Window window, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(window, message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public static void ShowErrorBox(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(Application.Current.MainWindow, message, Constants.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public static MessageBoxResult ShowMessageBoxWithOptions(string message)
        {
            return MessageBox.Show(message, Constants.APPLICATION_NAME, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        public static MessageBoxResult ShowMessageBoxWithOptions(Window window, string message, MessageBoxButton buttonOptions)
        {
            return MessageBox.Show(window, message, Constants.APPLICATION_NAME, buttonOptions, MessageBoxImage.Question);
        }

        public static MessageBoxResult ShowMessageBoxWithOptions(string message, MessageBoxButton buttonOptions)
        {
            return MessageBox.Show(message, Constants.APPLICATION_NAME, buttonOptions, MessageBoxImage.Question);
        }

        public static bool IsValid(DependencyObject parent)
        {
            // Validate all the bindings on the parent
            bool valid = true;
            LocalValueEnumerator localValues = parent.GetLocalValueEnumerator();
            while (localValues.MoveNext())
            {
                LocalValueEntry entry = localValues.Current;
                if (BindingOperations.IsDataBound(parent, entry.Property))
                {
                    Binding binding = BindingOperations.GetBinding(parent, entry.Property);
                    foreach (ValidationRule rule in binding.ValidationRules)
                    {
                        ValidationResult result = rule.Validate(parent.GetValue(entry.Property), null);
                        if (!result.IsValid)
                        {
                            BindingExpression expression = BindingOperations.GetBindingExpression(parent, entry.Property);
                            System.Windows.Controls.Validation.MarkInvalid(expression, new ValidationError(rule, expression, result.ErrorContent, null));
                            valid = false;
                        }
                    }
                }
            }

            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child)) { valid = false; }
            }

            return valid;
        }

        public static bool ShutdownRemoteMachine()
        {
            using (var securePwd = new System.Security.SecureString())
            {
                var pwdString = "woodlands";
                var pwdArray = pwdString.ToCharArray();
                foreach (var item in pwdArray)
                {
                    securePwd.AppendChar(item);
                }
                var psi1 = new ProcessStartInfo("cmd.exe", @"/C net use \\swadeshi\IPC$ woodlands /USER:woodlandstrader & shutdown /m  \\swadeshi /s /t 0");

                //var psi = new ProcessStartInfo("shutdown", "/m  \\swadeshi /s /t 0")
                //{

                //    //UseShellExecute = false,
                //    //UserName = "WoodlandsTrader",
                //    //Password = securePwd
                //};



                //using (Process process = Process.Start(psi1))
                //{
                //    process.WaitForExit();
                //}
                using (Process process = Process.Start(psi1))
                {
                    process.WaitForExit();
                }
            }
            return true;
        }
    }
}
