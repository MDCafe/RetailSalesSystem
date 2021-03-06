﻿using log4net;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Stocks
{
    internal class StockAdjustmentViewModel : DocumentViewModel
    {
        static readonly ILog log = LogManager.GetLogger(typeof(StockAdjustmentViewModel));
        public ObservableCollection<StockAdjustProductPrice> ProductsPriceList { get; private set; }
        public ObservableCollection<StockAdjustmentExtn> StockAdjustmentList { get; private set; }

        public StockAdjustmentViewModel()
        {
            StockAdjustmentList = new ObservableCollection<StockAdjustmentExtn>();
            Title = "Stock Adjustment";

            PopulateProductsList();
        }

        private void PopulateProductsList()
        {
            var productsPriceSQL = "select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price', " +
                                 " st.Quantity as 'Quantity', st.Id as 'StockId',pd.SellingPrice as 'SellingPrice' " +
                                 " from Products p, PriceDetails pd, Stocks st " +
                                 "where p.Id = pd.ProductId and pd.PriceId = st.PriceId " +
                                 " and st.Quantity != 0 and p.Isactive = true" +
                                 " union " +
                                   "select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price'," +
                                   " st.Quantity as 'Quantity', st.Id as 'StockId',pd.SellingPrice as 'SellingPrice' " +
                                   " from Products p, PriceDetails pd, Stocks st " +
                                   " where p.Id = pd.ProductId and pd.PriceId = st.PriceId " +
                                   " and st.Quantity = 0 and p.Isactive = true " +
                                   " and St.ModifiedOn = " +
                                   " (select max(ModifiedOn) from Stocks s " +
                                    "   where s.ProductId = st.ProductId) " +
                                   " order by ProductName ";

            using (RMSEntities rMSEntities = new RMSEntities())
            {
                ProductsPriceList = new
                    ObservableCollection<StockAdjustProductPrice>(rMSEntities.Database.SqlQuery<StockAdjustProductPrice>(productsPriceSQL));
            }
        }

        public void SetProductDetails(StockAdjustProductPrice productPrice, int selectedIndex)
        {

            if (productPrice == null)
            {
                log.Debug("SetProductDetails(); Product Price is null");
                return;
            }
            try
            {
                log.Info("SetProductDetails:SelIndex:" + selectedIndex);
                if (selectedIndex == -1 || selectedIndex > StockAdjustmentList.Count - 1)
                {
                    //_log.Info("Inside Return : selectedIndex" + selectedIndex);
                    //_log.Info("Inside Return : SaleDetailList.Count" + StockAdjustmentList.Count);
                    StockAdjustmentList.Add(new StockAdjustmentExtn());
                    selectedIndex -= 1;
                    //SaleDetailList[selectedIndex] = ;
                    //return;
                }
                var stockAdjustDetail = StockAdjustmentList[selectedIndex];
                //StockAdjustmentList.Clear();
                if (stockAdjustDetail != null)
                {
                    stockAdjustDetail.CostPrice = productPrice.Price;
                    stockAdjustDetail.OpeningBalance = productPrice.Quantity;
                    stockAdjustDetail.StockId = productPrice.StockId;
                    stockAdjustDetail.ProductId = productPrice.ProductId;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error on SetProductDetails", ex);
                //_log.ErrorFormat("SelectedIndex: {0}. ProductId Id:{1} Price Id:{2}", selectedIndex, productPrice.ProductId, productPrice.PriceId);
            }
        }

        internal override void Clear()
        {
            StockAdjustmentList = new ObservableCollection<StockAdjustmentExtn>();
            //Populate again as the Quantity needs to change after adjustment
            PopulateProductsList();
        }


        #region SaveCommand
        RelayCommand<object> _saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand<object>((p) => OnSave(), (p) =>
                    {
                        return StockAdjustmentList != null && StockAdjustmentList.Count != 0;
                    });
                }

                return _saveCommand;
            }
        }

        private void OnSave()
        {
            using (var rmsEntities = new RMSEntities())
            {
                try
                {
                    var combinedDateTime = RMSEntitiesHelper.GetCombinedDateTime();
                    foreach (var item in StockAdjustmentList)
                    {
                        var stockadjustment = new StockAdjustment()
                        {
                            StockId = item.StockId,
                            AdjustedQty = item.AdjustedQty,
                            OpeningBalance = item.OpeningBalance,
                            ClosingBalance = item.ClosingBalance,
                            CostPrice = item.CostPrice,
                            AddedOn = combinedDateTime
                        };
                        rmsEntities.StockAdjustments.Add(stockadjustment);

                        var stock = rmsEntities.Stocks.FirstOrDefault(s => s.Id == item.StockId);
                        if (stock != null)
                        {
                            stock.Quantity = stockadjustment.ClosingBalance.Value;
                        }

                        var stockTrns = rmsEntities.StockTransactions.Where(st => st.StockId == item.StockId).OrderByDescending(d => d.AddedOn).FirstOrDefault();
                        //while Adjusting always add new row..

                        var newStockTrans = new StockTransaction()
                        {
                            StockId = item.StockId,
                            OpeningBalance = stockTrns != null ? stockTrns.ClosingBalance : stock.Quantity, //Closing balance of last transaction if trans is not null
                            ClosingBalance = item.ClosingBalance,
                            AddedOn = combinedDateTime
                        };

                        stockadjustment.StockTransaction = newStockTrans;
                        rmsEntities.StockTransactions.Add(newStockTrans);
                        rmsEntities.SaveChanges();
                        //Notify all open sales and purchase window that the stock has been updatedc
                        RMSEntitiesHelper.Instance.NotifyAllPurchaseAndSalesOnStockUpdate();

                        Clear();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while saving", ex);
                    Utilities.Utility.ShowErrorBox(ex.Message);
                }
            }
        }
        #endregion
    }
}
