using log4net;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Stocks
{
    class SwapsViewModel : DocumentViewModel
    {
        static readonly ILog log = LogManager.GetLogger(typeof(SwapsViewModel));
        public ObservableCollection<StockAdjustProductPrice> ProductsPriceList { get; private set; }
        public DateTime TranscationDate { get; set; }
        public int SelectedSwapModeId { get; set; }
        public Customer SelectedCustomer { get; set; }
        public IEnumerable<CodeMaster> SwapsCodeList { get; set; }
        public static IEnumerable<Customer> SwapsCustomersList { get; set; }
        public decimal TotalAmount { get; set; }
        public ObservableCollection<SwapExtn> SwapsDetailList { get; private set; }

        public SwapsViewModel()
        {
            Title = "Lend/Borrow Products";
            SwapsDetailList = new ObservableCollection<SwapExtn>();
            SwapsDetailList.CollectionChanged += SwapsDetailList_CollectionChanged;

            using (var rmsEntities = new RMSEntities())
            {
                SwapsCustomersList = rmsEntities.Customers.Where(i => i.IsLenderBorrower == true).ToList();
                SwapsCodeList = rmsEntities.CodeMasters.Where(c => c.Code == "SWP").ToList();
            }

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
            TranscationDate = RMSEntitiesHelper.Instance.GetSystemDBDate();
            //SelectedSwapModeId = 5;
        }

        private void SwapsDetailList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                TotalAmount = SwapsDetailList.Sum(sw => sw.Amount);
            }
        }

        internal override void Clear()
        {
            SwapsDetailList = new ObservableCollection<SwapExtn>();
            TotalAmount = 0;
            SelectedCustomer = null;
            SelectedSwapModeId = -1;
        }

        internal void SetProductDetails(StockAdjustProductPrice productPrice, int selectedIndex)
        {
            if (productPrice == null) return;
            //var saleItem = SwapsDetailList.FirstOrDefault(s => s.ProductId == productPrice.ProductId && s.PriceId == productPrice.PriceId);
            var selRowSwapExtn = SwapsDetailList[selectedIndex];
            if (selRowSwapExtn != null)
            {
                selRowSwapExtn.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "Amount")
                    {
                        TotalAmount = SwapsDetailList.Sum(t => t.Amount);
                    }
                };
                selRowSwapExtn.Quantity = 1;
                selRowSwapExtn.SellingPrice = productPrice.SellingPrice;
                selRowSwapExtn.AvailableStock = productPrice.Quantity;
                selRowSwapExtn.CostPrice = productPrice.Price;
                selRowSwapExtn.Amount = productPrice.SellingPrice * selRowSwapExtn.Quantity;
                selRowSwapExtn.StockId = productPrice.StockId;
            }
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
                        return SelectedCustomer != null && SelectedCustomer.Id != 0 && SwapsDetailList.Count != 0 &&
                          SwapsDetailList[0].ProductId != 0;
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
                    var swaps = new Swap()
                    {
                        LendOrBorrowId = SelectedSwapModeId,
                        CustomerId = SelectedCustomer.Id,
                        TotalAmount = TotalAmount,
                        AddedOn = TranscationDate
                    };
                    rmsEntities.Swaps.Add(swaps);

                    var stockTransDate = RMSEntitiesHelper.GetCombinedDateTime(TranscationDate);

                    foreach (var item in SwapsDetailList)
                    {
                        swaps.SwapDetails.Add(new SwapDetail()
                        {
                            StockId = item.StockId,
                            Quantity = item.Quantity,
                            SellingPrice = item.SellingPrice,
                            CostPrice = item.CostPrice,
                            AddedOn = TranscationDate
                        });

                        var stock = rmsEntities.Stocks.FirstOrDefault(s => s.Id == item.StockId);
                        SetStockTransaction(rmsEntities, item, stock, stockTransDate);

                        if (stock != null)
                        {
                            stock.Quantity -= item.Quantity;
                        }
                    }
                    rmsEntities.SaveChanges();
                    Clear();
                }
                catch (Exception ex)
                {
                    log.Error("Error while saving", ex);
                    Utilities.Utility.ShowErrorBox(ex.Message);
                }
            }
        }

        #endregion 


        private static void SetStockTransaction(RMSEntities rmsEntities, SwapExtn swapDetail, Stock stockNewItem, DateTime stockTransDate)
        {
            var stockTrans = rmsEntities.StockTransactions.Where(s => s.StockId == stockNewItem.Id).OrderByDescending(s => s.AddedOn).FirstOrDefault();
            var stockAdjustCheck = RMSEntitiesHelper.CheckStockAdjustment(rmsEntities, stockNewItem.Id);

            //stock transaction not available for this product. Add them 
            if (stockTrans == null)
            {
                var firstStockTrans = new StockTransaction()
                {
                    OpeningBalance = stockNewItem.Quantity - swapDetail.Quantity, //Opening balance will be the one from stock table 
                    Outward = swapDetail.Quantity,
                    ClosingBalance = stockNewItem.Quantity,
                    StockId = stockNewItem.Id,
                    AddedOn = stockTransDate
                };
                stockNewItem.StockTransactions.Add(firstStockTrans);
            }
            //stock transaction available. Check if it is for the current date else get the latest date and mark the opening balance
            else
            {
                var systemDBDate = RMSEntitiesHelper.Instance.GetSystemDBDate();
                var dateDiff = DateTime.Compare(stockTrans.AddedOn.Value.Date, systemDBDate);
                if (dateDiff == 0 && stockAdjustCheck != null && !stockAdjustCheck.StockTransId.HasValue)
                {
                    stockTrans.Outward = swapDetail.Quantity + (stockTrans.Outward ?? 0);
                    stockTrans.ClosingBalance -= swapDetail.Quantity;
                }
                else
                {
                    var newStockTrans = new StockTransaction()
                    {
                        OpeningBalance = stockTrans.ClosingBalance,
                        Outward = swapDetail.Quantity,
                        ClosingBalance = stockTrans.ClosingBalance - swapDetail.Quantity,
                        StockId = stockNewItem.Id,
                        AddedOn = stockTransDate
                    };
                    rmsEntities.StockTransactions.Add(newStockTrans);
                }
            }
        }
    }
}
