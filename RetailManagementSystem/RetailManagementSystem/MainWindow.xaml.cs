using RetailManagementSystem.Utilities;
using RetailManagementSystem.View.Sales;
using RetailManagementSystem.ViewModel;
using RetailManagementSystem.ViewModel.Purchases;
using RetailManagementSystem.ViewModel.Sales;
using System;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using log4net;

namespace RetailManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly ILog log = LogManager.GetLogger(typeof(SalesEntryViewModel));
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Workspace.This;
            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);

            try
            {
                this.Title = RMSEntitiesHelper.Instance.RMSEntities.ApplicationDetails.FirstOrDefault().Name + " - " +
               "Retail Management System";
            }
            catch (System.Data.DataException entityEx)
            {
                log.Error("Database Connection Exception", entityEx);
                Utility.ShowErrorBox("Unable to connect to the database");
                Application.Current.Shutdown();
            }
       }

        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (!((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))) return;

            #region Sales

            if (Keyboard.IsKeyDown(Key.S))
            {
                switch(e.Key)
                {
                    case Key.R:
                        Workspace.This.OpenReturnSalesCommand.Execute(true);
                        return;
                    case Key.A:
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
                    case Key.E:
                        {
                            var saleParams = new SalesParams() { ShowAllCustomers = true, IsTempDataWindow = true };
                            Workspace.This.OpenSalesEntryCommand.Execute(saleParams);
                            return;
                        }
                } 
            }
            #endregion

            #region Purchases

            if (Keyboard.IsKeyDown(Key.P))
            {
                switch (e.Key)
                {
                    case Key.R:
                        Workspace.This.OpenReturnSalesCommand.Execute(true);
                        return;
                    case Key.A:
                        try
                        {
                            AmendPurchases amendPurchase = new AmendPurchases(true);
                            amendPurchase.ShowDialog();
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
                    case Key.E:
                        {
                            var purchaseParams = new PurchaseParams() { ShowAllCompanies = true };
                            Workspace.This.OpenPurchaseEntryCommand.Execute(purchaseParams);
                            return;
                        }
                }
            }
            #endregion

            #region Reports
            if (Keyboard.IsKeyDown(Key.R))
            {
                switch (e.Key)
                {
                    case Key.P:
                        Workspace.This.OpenDailyPurchaseReportCommand.Execute(true);
                        return;
                    case Key.S:
                        Workspace.This. OpenDailySalesReportCommand.Execute(true);
                        return;
                    case Key.C:
                        Workspace.This.OpenCustomerWiseSalesReportCommand.Execute(true);
                        return;
                }
            }
            #endregion

            #region 
            if (Keyboard.IsKeyDown(Key.T))
            {
                switch (e.Key)
                {
                    case Key.E:
                        Workspace.This.OpenReturnPurchaseCommand.Execute(true);
                        return;
                }
            }
            #endregion
        }
    }
}
