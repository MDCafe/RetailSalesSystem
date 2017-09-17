using RetailManagementSystem.Utilities;
using RetailManagementSystem.View.Sales;
using RetailManagementSystem.ViewModel;
using RetailManagementSystem.ViewModel.Purchases;
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
            if (!((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))) return;

            if (Keyboard.IsKeyDown(Key.P) && Keyboard.IsKeyDown(Key.R))
            {
                Workspace.This.OpenDailyPurchaseReportCommand.Execute(true);
                return;
            }

            if (Keyboard.IsKeyDown(Key.S) && Keyboard.IsKeyDown(Key.R))
            {
                Workspace.This.OpenReturnSalesCommand.Execute(true);
                return;
            }

            switch (e.Key)
            {
                //Sales Entry
                case Key.S:
                {
                    var saleParams = new SalesParams() { ShowAllCustomers = true, IsTempDataWindow = true };
                    Workspace.This.OpenSalesEntryCommand.Execute(saleParams);
                    return;
                }

                //Sales Amend
                case Key.A:
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
                

                //Sales Return
                case Key.P:
                    {
                        var purchaseParams = new PurchaseParams() { ShowAllCompanies = true};
                        Workspace.This.OpenPurchaseEntryCommand.Execute(purchaseParams);
                        return;
                    }
            }                                 
        }        
    }
}
