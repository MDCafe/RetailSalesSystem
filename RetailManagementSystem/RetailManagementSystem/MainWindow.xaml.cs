using RetailManagementSystem.Utilities;
using RetailManagementSystem.View.Sales;
using RetailManagementSystem.ViewModel;
using RetailManagementSystem.ViewModel.Sales;
using System;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Workspace.This;
            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);
            
        }

        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            //Sales Entry
            if (e.Key == Key.S && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                var saleParams = new SalesParams() { ShowAllCustomers = true, IsTempDataWindow = true};
                Workspace.This.OpenSalesEntryCommand.Execute(saleParams);
                return;
            }

            //Sales Amend
            if (e.Key == Key.A && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                try
                {
                    AmendSales amendSales = new AmendSales(true);
                    amendSales.ShowDialog();
                }
                catch (Exceptions.RMSException ex)
                {
                    Utility.ShowErrorBox(ex.Message);
                }
                catch (Exception ex)
                {
                    Utility.ShowErrorBox(ex.Message);                    
                }
                return;
            }
            
        }        
    }
}
