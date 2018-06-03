﻿using RetailManagementSystem.Interfaces;
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
        RMSEntities _rmsEntities;
        static RMSEntitiesHelper _rMSEntitiesHelper;
        static object _syncRoot = new object();        
        List<INotifier> _salesNotifierList = new List<INotifier>();
        List<INotifier> _purchaseNotifierList = new List<INotifier>();
        static string productsPriceSQL;

        private RMSEntitiesHelper()
        {
            _rmsEntities = new RMSEntities();

            productsPriceSQL = "select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price', " +
                                 " pd.SellingPrice as 'SellingPrice',st.Quantity as 'Quantity', pd.PriceId as 'PriceId', " +
                                 " DATE_FORMAT(st.ExpiryDate,'%d/%m/%Y') as 'ExpiryDate'," +
                                 " p.SupportsMultiPrice AS 'SupportsMultiplePrice',p.barcodeno" +
                                 " from Products p, PriceDetails pd, Stocks st " +
                                 "where p.Id = pd.ProductId and pd.PriceId = st.PriceId " +
                                 " and st.Quantity != 0 and p.Isactive = true" +
                                 " union " +
                                   "select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price'," +
                                   "pd.SellingPrice as 'SellingPrice',st.Quantity as 'Quantity', pd.PriceId as 'PriceId', " +
                                   " DATE_FORMAT(st.ExpiryDate,'%d/%m/%Y') as 'ExpiryDate'," +
                                   " p.SupportsMultiPrice AS 'SupportsMultiplePrice',p.barcodeno" +
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

        public RMSEntities RMSEntities
        {
            get
            {                
                    return _rmsEntities;
            }
        }

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

        public void SelectRunningBillNo(int categoryId,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            string sqlRunningNo = "select max(rollingno) + 1 from category cat where  cat.id = @p0";
            var salesNo = _rmsEntities.Database.SqlQuery<int>(sqlRunningNo, categoryId).FirstOrDefault();

            //Purhcase or sales is done, refresh the purchase & sales screens product list
            _salesNotifierList.ForEach(s => s.NotifyPurchaseUpdate());
            _purchaseNotifierList.ForEach(p => p.NotifyPurchaseUpdate());

            if (sourceFilePath.Contains("PurchaseEntryViewModel"))
            {
                foreach (var purchaseNotifyList in _purchaseNotifierList)
                {
                    purchaseNotifyList.Notify(salesNo,categoryId);
                }
                return;
            }

            foreach (var notifyList in _salesNotifierList)
            {
                notifyList.Notify(salesNo, categoryId);
            }
        }

        public ObservableCollection<ProductPrice> GetProductPriceList()
        {
            return new ObservableCollection<ProductPrice>(Instance.RMSEntities.Database.SqlQuery<ProductPrice>(productsPriceSQL));
            //foreach (var item in productList)
            //{
            //    productPriceList.Add(item);
            //}
        }

        public decimal? GetLastSoldPrice(int productId,int customerId)
        {
            string lastSoldPriceSQL = "select sd.SellingPrice from sales s, saleDetails sd " +
                                  "  where s.CustomerId = " + customerId +
                                  "  and s.BillId = sd.billId " +
                                  "  and sd.ProductId =  " + productId +
                                  "  order by sd.ModifiedOn desc " +
                                  "  limit 0, 1";

            return Instance.RMSEntities.Database.SqlQuery<decimal?>(lastSoldPriceSQL).FirstOrDefault();
        }

        public static CustomerBill CheckIfBillExists(int billNo, int categoryId,Window window)
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
                if(window !=null)
                    Utility.ShowErrorBox(window,"Bill Number doesn't exist");
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
                Utility.ShowErrorBox(window,"Bill Number doesn't exist");
                return null;
            }

            return companyBill;
        }

        public static DateTime GetServerDate()
        {
            var sql = "select RMS.GetSysDate()";
            var serverDateTime = Instance.RMSEntities.Database.SqlQuery<DateTime>(sql);
            return serverDateTime.FirstOrDefault();
        }

    }

    public class  CustomerBill
    {
        public int CustomerId { get; set; }
    }

    public class CompanyBill
    {
        public int CompanyId { get; set; }
    }

}
