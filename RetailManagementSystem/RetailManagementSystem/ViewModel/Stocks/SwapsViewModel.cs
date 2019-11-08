using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RetailManagementSystem.ViewModel.Stocks
{
    class SwapsViewModel : DocumentViewModel
    {                
        public ObservableCollection<StockAdjustProductPrice> ProductsPriceList { get; private set; }
        public DateTime TranscationDate { get; set; }
        public int SelectedSwapModeId { get;set; }        
        public IEnumerable<CodeMaster> SwapsCodeList { get; set; }
        public static IEnumerable<Customer> SwapsCustomersList{ get; set; }
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
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                TotalAmount = SwapsDetailList.Sum(sw => sw.Amount);
            }
        }

        internal override void Clear()
        {                        
            SwapsDetailList = new ObservableCollection<SwapExtn>();
            TotalAmount = 0;
        }

        internal void SetProductDetails(StockAdjustProductPrice productPrice, int selectedIndex)
        {
            if (productPrice == null) return;
            //var saleItem = SwapsDetailList.FirstOrDefault(s => s.ProductId == productPrice.ProductId && s.PriceId == productPrice.PriceId);
            var selRowSwapExtn = SwapsDetailList[selectedIndex];
            if(selRowSwapExtn != null)
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
            }
        }        
    }
}
