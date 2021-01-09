using RetailManagementSystem.Interfaces;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace RetailManagementSystem
{
    internal class RMSEntitiesHelper
    {
        static RMSEntitiesHelper _rMSEntitiesHelper;
        static readonly object _syncRoot = new object();
        readonly List<INotifier> _salesNotifierList = new List<INotifier>();
        readonly List<INotifier> _purchaseNotifierList = new List<INotifier>();
        static string productsPriceSQL;

        private RMSEntitiesHelper()
        {
            RMSEntities = new RMSEntities();

            productsPriceSQL = "select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price', " +
                                 " pd.SellingPrice as 'SellingPrice',st.Quantity as 'Quantity', pd.PriceId as 'PriceId', " +
                                 " DATE_FORMAT(st.ExpiryDate,'%d/%m/%Y') as 'ExpiryDate'," +
                                 " p.SupportsMultiPrice AS 'SupportsMultiplePrice',p.barcodeno,p.UnitOfMeasure" +
                                 " from Products p, PriceDetails pd, Stocks st " +
                                 "where p.Id = pd.ProductId and pd.PriceId = st.PriceId " +
                                 " and st.Quantity != 0 and p.Isactive = true" +
                                 " union " +
                                   "select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price'," +
                                   "pd.SellingPrice as 'SellingPrice',st.Quantity as 'Quantity', pd.PriceId as 'PriceId', " +
                                   " DATE_FORMAT(st.ExpiryDate,'%d/%m/%Y') as 'ExpiryDate'," +
                                   " p.SupportsMultiPrice AS 'SupportsMultiplePrice',p.barcodeno,p.UnitOfMeasure" +
                                   " from Products p, PriceDetails pd, Stocks st " +
                                   " where p.Id = pd.ProductId and pd.PriceId = st.PriceId " +
                                   " and st.Quantity = 0 and p.Isactive = true " +
                                   " and St.ModifiedOn = " +
                                   " (select max(ModifiedOn) from Stocks s " +
                                    "   where s.ProductId = st.ProductId) " +
                                   " order by ProductName ";
        }

        public static RMSEntitiesHelper Instance
        {
            get
            {
                Monitor.Enter(_syncRoot);
                if (_rMSEntitiesHelper == null)
                {
                    Monitor.Exit(_syncRoot);
                    return _rMSEntitiesHelper = new RMSEntitiesHelper();
                }
                Monitor.Exit(_syncRoot);
                return _rMSEntitiesHelper;
            }
        }

        public RMSEntities RMSEntities { get; }

        public RMSEntities GetNewInstanceOfRMSEntities()
        {
            return new RMSEntities();
        }

        public void AddNotifier(INotifier notifier)
        {
            Monitor.Enter(_salesNotifierList);
            _salesNotifierList.Add(notifier);
            Monitor.Exit(_salesNotifierList);
        }

        public void RemoveNotifier(INotifier notifier)
        {
            Monitor.Enter(_salesNotifierList);
            _salesNotifierList.Remove(notifier);
            Monitor.Exit(_salesNotifierList);
        }

        public void AddPurchaseNotifier(INotifier notifier)
        {
            Monitor.Enter(_salesNotifierList);
            _purchaseNotifierList.Add(notifier);
            Monitor.Exit(_salesNotifierList);
        }

        public void RemovePurchaseNotifier(INotifier notifier)
        {
            Monitor.Enter(_salesNotifierList);
            _purchaseNotifierList.Remove(notifier);
            Monitor.Exit(_salesNotifierList);
        }

        public void SelectRunningBillNo(int categoryId, bool onload,
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            string sqlRunningNo = "select max(rollingno) + 1 from category cat where  cat.id = @p0";
            var salesNo = RMSEntities.Database.SqlQuery<int>(sqlRunningNo, categoryId).FirstOrDefault();

            if (!onload)
            {
                //Purhcase or sales is done, refresh the purchase & sales screens product list
                _salesNotifierList.ForEach(s => s.NotifyPurchaseUpdate());
                _purchaseNotifierList.ForEach(p => p.NotifyPurchaseUpdate());
            }
            if (sourceFilePath.Contains("PurchaseEntryViewModel"))
            {
                foreach (var purchaseNotifyList in _purchaseNotifierList)
                {
                    purchaseNotifyList.Notify(salesNo, categoryId);
                }
                return;
            }

            foreach (var notifyList in _salesNotifierList)
            {
                notifyList.Notify(salesNo, categoryId);
            }
        }

        public static ObservableCollection<ProductPrice> GetProductPriceList()
        {
            return new ObservableCollection<ProductPrice>(Instance.RMSEntities.Database.SqlQuery<ProductPrice>(productsPriceSQL));
            //foreach (var item in productList)
            //{
            //    productPriceList.Add(item);
            //}
        }

        public static ObservableCollection<ProductPrice> GetProductPriceList(int companyId)
        {
            var productsPriceSQLCompany = "select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price', " +
                                          " pd.SellingPrice as 'SellingPrice',st.Quantity as 'Quantity', pd.PriceId as 'PriceId', " +
                                          " DATE_FORMAT(st.ExpiryDate,'%d/%m/%Y') as 'ExpiryDate'," +
                                          " p.SupportsMultiPrice AS 'SupportsMultiplePrice',p.barcodeno,p.UnitOfMeasure" +
                                          " from Products p, PriceDetails pd, Stocks st " +
                                          "where p.Id = pd.ProductId and pd.PriceId = st.PriceId " +
                                          " and st.Quantity != 0 and p.Isactive = true and p.CompanyId =" + companyId +
                                          " union " +
                                            "select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price'," +
                                            "pd.SellingPrice as 'SellingPrice',st.Quantity as 'Quantity', pd.PriceId as 'PriceId', " +
                                            " DATE_FORMAT(st.ExpiryDate,'%d/%m/%Y') as 'ExpiryDate'," +
                                            " p.SupportsMultiPrice AS 'SupportsMultiplePrice',p.barcodeno,p.UnitOfMeasure" +
                                            " from Products p, PriceDetails pd, Stocks st " +
                                            " where p.Id = pd.ProductId and pd.PriceId = st.PriceId " +
                                            " and st.Quantity = 0 and p.Isactive = true and p.CompanyId =" + companyId +
                                            " and St.ModifiedOn = " +
                                            " (select max(ModifiedOn) from Stocks s " +
                                             "   where s.ProductId = st.ProductId) " +
                                            " order by ProductName ";
            return new ObservableCollection<ProductPrice>(Instance.RMSEntities.Database.SqlQuery<ProductPrice>(productsPriceSQLCompany));
        }

        public static decimal? GetLastSoldPrice(int productId, int customerId)
        {
            string lastSoldPriceSQL = "select sd.SellingPrice from sales s, saleDetails sd " +
                                  "  where s.CustomerId = " + customerId +
                                  "  and s.BillId = sd.billId " +
                                  "  and sd.ProductId =  " + productId +
                                  "  order by sd.ModifiedOn desc " +
                                  "  limit 0, 1";

            return Instance.RMSEntities.Database.SqlQuery<decimal?>(lastSoldPriceSQL).FirstOrDefault();
        }

        public static CustomerBill CheckIfBillExists(int billNo, int categoryId, Window window)
        {
            var checkBill = from s in RMSEntitiesHelper.Instance.RMSEntities.Sales
                            join c in RMSEntitiesHelper.Instance.RMSEntities.Customers
                            on s.CustomerId equals c.Id
                            where s.RunningBillNo == billNo && c.CustomerTypeId.Value == categoryId
                            select new CustomerBill
                            {
                                CustomerId = s.CustomerId,
                            };

            var customerBill = checkBill.FirstOrDefault();

            if (checkBill.FirstOrDefault() == null)
            {
                if (window != null)
                    Utility.ShowErrorBox(window, "Bill Number doesn't exist");
                else
                    Utility.ShowErrorBox("Bill Number doesn't exist");

                return null;
            }

            return customerBill;
        }

        public static CompanyBill CheckIfPurchaseBillExists(int billNo, int categoryId, Window window)
        {
            var checkBill = from s in RMSEntitiesHelper.Instance.RMSEntities.Purchases
                            join c in RMSEntitiesHelper.Instance.RMSEntities.Companies
                            on s.CompanyId equals c.Id
                            where s.RunningBillNo == billNo && c.CategoryTypeId.Value == categoryId
                            select new CompanyBill
                            {
                                CompanyId = s.CompanyId,
                            };

            var companyBill = checkBill.FirstOrDefault();

            if (checkBill.FirstOrDefault() == null)
            {
                Utility.ShowErrorBox(window, "Bill Number doesn't exist");
                return null;
            }

            return companyBill;
        }

        public static DateTime GetServerDate()
        {
            var sql = "select GetSysDate()";
            var serverDateTime = Instance.RMSEntities.Database.SqlQuery<DateTime>(sql);
            return serverDateTime.FirstOrDefault();
        }

        public static TimeSpan GetServerTime()
        {
            var sql = "select time(now())";
            var serverTime = Instance.RMSEntities.Database.SqlQuery<TimeSpan>(sql);
            return serverTime.FirstOrDefault();
        }

        public bool IsAdmin(string userId)
        {
            return RMSEntities.Users.Any(u => u.username == userId && u.RoleId == Constants.ADMIN);
        }

        public DateTime GetSystemDBDate()
        {
            return GetServerDate();
            //using (RMSEntities rmsEntities = new RMSEntities())
            //{
            //    //return rmsEntities.SystemDatas.First().SysDate.Value;
            //    return rmsEntities.Database.SqlQuery<DateTime>();
            //}
        }

        public void UpdateSystemDBDate()
        {
            using (RMSEntities rmsEntities = new RMSEntities())
            {
                var systemDBDate = rmsEntities.SystemDatas.FirstOrDefault();
                if (systemDBDate == null) return;

                var newDate = systemDBDate.SysDate.Value.AddDays(1);
                systemDBDate.SysDate = newDate;
                rmsEntities.Entry<SystemData>(systemDBDate).State = System.Data.Entity.EntityState.Modified;
                rmsEntities.SaveChanges();

            }
        }

        public static void MarkEndOfDay()
        {
            using(var rmsEntities = new RMSEntities())
            {
                
                
                var sql = "MarkEndOfDay()";

                var resultInt = rmsEntities.Database.SqlQuery<int>(sql).FirstOrDefault();
                rmsEntities.SaveChanges();

                //var result = MySQLDataAccess.GetData(sql);
                //var resultInt = Convert.ToInt32(result);
                if (resultInt == 0)
                {
                    Utility.ShowMessageBox("Mark End of Day successfully completed");
                }
                else
                {
                    Utility.ShowErrorBox("Mark End of Day has been completed already");
                }                
            }
        }

        public bool CheckSystemDBDate()
        {
            using (RMSEntities rmsEntities = new RMSEntities())
            {
                var systemDBDate = rmsEntities.SystemDatas.FirstOrDefault();
                if (systemDBDate == null) return false;

                var serverDate = GetServerDate().AddDays(1);
                if (serverDate.Date.Ticks == systemDBDate.SysDate.Value.Ticks)
                    return true;

                return false;
            }
        }

        public IEnumerable<User> GetUsers()
        {
            using (RMSEntities rmsEntities = new RMSEntities())
            {
                return rmsEntities.Users.ToList();
            }
        }

        public static DateTime GetCombinedDateTime(DateTime transactionDate)
        {
            DateTime combinedDateTime;
            //Get the current time since it takes the window open time
            DateTime date = transactionDate.Date;
            TimeSpan time = GetServerTime();
            //TimeSpan time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            combinedDateTime = date.Add(time);
            return combinedDateTime;
        }

        public static DateTime GetCombinedDateTime()
        {
            return GetServerDate();
            //DateTime combinedDateTime;
            //Get the current time since it takes the window open time
            //DateTime date = Instance.GetSystemDBDate();
            //TimeSpan time = GetServerTime();
            //TimeSpan time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            //combinedDateTime = date.Add(time);
            //return combinedDateTime;
        }

        public static StockTransactionExt CheckStockAdjustment(RMSEntities rmsEntities, int stockId)
        {
            var query = "select st.*,sa.StockTransId from StockTransaction st  " +
                                    " left join StockAdjustments sa on(st.id = sa.stockTransId) " +
                                    " where st.StockId = " + stockId +
                                    " order by st.Addedon desc ";
            var stockTransCheck = rmsEntities.Database.SqlQuery<StockTransactionExt>(query).FirstOrDefault();
            return stockTransCheck;
        }


        public void NotifyAllPurchaseAndSalesOnStockUpdate()
        {
            _salesNotifierList.ForEach(n => n.NotifyStockUpdate());
            _purchaseNotifierList.ForEach(n => n.NotifyStockUpdate());
        }
    }

    public class CustomerBill
    {
        public int CustomerId { get; set; }
    }

    public class CompanyBill
    {
        public int CompanyId { get; set; }
    }

}
