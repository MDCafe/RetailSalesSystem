using log4net;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.UserControls;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.ViewModel.Entitlements;
using RetailManagementSystem.ViewModel.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Sales
{
    class SalesEntryViewModel : SalesViewModelbase
    {
        #region Private Variables
        static readonly ILog _log = LogManager.GetLogger(typeof(SalesEntryViewModel));                                       
        Sale _billSales;                          
        
        IExtensions _extensions;

        //System.Timers.Timer _timer,_autoTimer;
        //static object rootLock = new object();
        //string _guid;
        readonly SalesParams _salesParams;
        List<Customer> _customerList;
        readonly AutoResetEvent _autoResetEvent;
        List<SaleDetailExtn> _deletedItems;
        Customer _selectedCustomer;
        string _selectedCustomerText;
        
        #endregion

        #region Constructor

        public SalesEntryViewModel(SalesParams salesParams) : base(salesParams !=null ? salesParams.ShowAllCustomers : false)
        {
            _salesParams = salesParams;
            //_rmsEntities = new RMSEntities();// RMSEntitiesHelper.Instance.RMSEntities;
            //var cnt = _rmsEntities.Customers.ToList();
            //var cnt1 = _rmsEntities.Products.ToList();

            _billSales = new Sale();
            SaleDetailList = new ObservableCollection<SaleDetailExtn>();
            SaleDetailList.CollectionChanged += OnSalesDetailsListCollectionChanged;
            _autoResetEvent = new AutoResetEvent(false);

            //SelectedCustomer = DefaultCustomer;

            Title = "Sales Entry";

            if (_salesParams.ShowAllCustomers)
            {
                Title = "Sales Entry*";
            }

            if (salesParams != null)
            {
                //Temp window to save 10 items                
                if(salesParams.IsTempDataWindow)
                {
                    //AutoSaveData();
                    
                    //Title = "Sales Entry";
                    RMSEntitiesHelper.Instance.AddNotifier(this);
                    RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId,true);
                    return;
                }

                if (salesParams.Billno.HasValue)
                {
                    //Amend Bill             
                    OnEditBill(salesParams.Billno.Value);
                    Title = "Sale Bill Amend :" + _runningBillNo;
                    IsVisible = System.Windows.Visibility.Visible;
                    return;
                }
                else if (salesParams.GetTemproaryData)
                {
                    //Get Temproary window from DB
                    //GetTempDataFromDB(salesParams.Guid);
                    return;
                }
                //return;
            }
            
            //Title = "Sales Entry";
            RMSEntitiesHelper.Instance.AddNotifier(this);
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId,true);
            //SaveDataTemp();            
        }
       
        #endregion

        #region Getters and Setters

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                using (var rmsEntities = new RMSEntities())
                {
                    var defaultCustomerConfigName = ConfigurationManager.AppSettings["DefaultCustomer"];
                    _customerList = new List<Customer>();
                    var cnt = rmsEntities.Customers.ToList().Count();

                    _customerList = new List<Customer>(cnt);

                    if (_salesParams.GetTemproaryData)
                    {
                        foreach (var item in rmsEntities.Customers)
                        {
                            if (item.CustomerTypeId == _categoryId)
                            {
                                _customerList.Add(item);
                            }

                        }
                        //_rmsEntities.Customers.Local.Where(c => c.CustomerTypeId == _categoryId);
                    }
                    if (_salesParams.ShowAllCustomers)
                    {
                        foreach (var item in rmsEntities.Customers)
                        {
                            if (item.CustomerTypeId == Constants.CUSTOMERS_OTHERS)
                            {
                                _customerList.Add(item);
                            }
                        }
                    }
                    //customerList =  _rmsEntities.Customers.Local.Where(c => c.CustomerTypeId == Constants.CUSTOMERS_OTHERS);
                    else
                    {
                        foreach (var item in rmsEntities.Customers)
                        {
                            if (item.CustomerTypeId == Constants.CUSTOMERS_HOTEL)
                            {
                                _customerList.Add(item);
                            }
                        }
                    }
                    //customerList  = _rmsEntities.Customers.Local.Where(c => c.CustomerTypeId != Constants.CUSTOMERS_OTHERS);

                    var defaultCustomerByConfig = _customerList.FirstOrDefault(c => c.Name.ToUpper() == defaultCustomerConfigName.ToUpper());
                    if (defaultCustomerByConfig != null)
                    {
                        DefaultCustomer = defaultCustomerByConfig;
                    }
                    return _customerList.OrderBy(a => a.Name);
                }
            }
        }

        public ObservableCollection<SaleDetailExtn> SaleDetailList { get; private set; }

        public string OrderNo { get; set; }       

        public decimal? TotalAmount
        {
            get { return _totalAmount; }
            set
            {
                //if discout is available apply it
                CalculateTotalAmount();                
            }
        }

        private void CalculateTotalAmount()
        {
            decimal? tempTotal = SaleDetailList.Sum(a => a.Amount);
            if (_totalDiscountAmount.HasValue)
            {
                tempTotal -= _totalDiscountAmount;
            }

            if (_totalDiscountPercent.HasValue)
            {
                var discountValue = tempTotal * (_totalDiscountPercent / 100);
                tempTotal -= discountValue;
            }

            if (_extensions != null)
            {
                var transportCharges = _extensions.GetPropertyValue("TransportCharges", out decimal? oldTransportValue);
                tempTotal += (transportCharges);
            }

            _totalAmount = tempTotal;
            //TotalAmountDisplay = _totalAmount.Value;
            RaisePropertyChanged("TotalAmount");
            RaisePropertyChanged("BalanceAmount");
        }

        public decimal? TotalDiscountAmount
        {
            get { return _totalDiscountAmount; }
            set
            {
                _totalDiscountAmount = value;
                CalculateTotalAmount();
                RaisePropertyChanged("TotalDiscountAmount");
            }
        }

        public decimal? TotalDiscountPercent
        {
            get { return _totalDiscountPercent; }
            set
            {
                _totalDiscountPercent = value;
                CalculateTotalAmount();
                RaisePropertyChanged("TotalDiscountPercent");
            }
        }

        public IExtensions Extensions
        {
            set
            {
                _extensions = value;
                if(_isEditMode)
                {
                    var transportCharges = _billSales.TransportCharges == null ? 0 : _billSales.TransportCharges.Value;
                    _extensions.SetValues(transportCharges);
                    TotalAmount = _extensions.Calculate(_totalAmount.Value);
                }

                _extensions.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "TransportCharges")
                    {
                        //TotalAmount = _extensions.Calculate(_totalAmount.Value);
                        CalculateTotalAmount();
                    }
                };
            }
            get { return _extensions; }
        }        

        public Customer SelectedCustomer
        {
            get {
                //if(_selectedCustomer == null)
                //{
                //    _selectedCustomer = _rmsEntities.Customers.FirstOrDefault();
                //    RaisePropertyChanged("SelectedCustomer");
                 //   return _selectedCustomer;
                //}
                return _selectedCustomer;
            }
            set
            {
                if (_selectedCustomer == value) return;

                _selectedCustomer = value;                
                //CheckIfWithinCreditLimimt();
                RaisePropertyChanged("SelectedCustomer");
            }
        }

        private void CheckIfWithinCreditLimimt()
        {
            if (_selectedCustomer !=null && _selectedCustomer.Name != "Cash Customer")
            {
                var fromSqlParam = new MySqlParameter("fromDate", MySqlDbType.Date);                
                var toSqlParam = new MySqlParameter("toDate", MySqlDbType.Date);                
                var categoryIdSqlParam = new MySqlParameter("customerId", MySqlDbType.Int32)
                {
                    Value = _selectedCustomer.Id
                };

                var paramList = new MySqlParameter[] { fromSqlParam, toSqlParam, categoryIdSqlParam };
                var acctBalanceAmount = MySQLDataAccess.GetData("GetCustomerBalance", paramList);
                var acctBalanceAmountValue = acctBalanceAmount != System.DBNull.Value ? Convert.ToDecimal(acctBalanceAmount) : 0m;
                if (acctBalanceAmountValue > _selectedCustomer.CreditLimit)
                {
                    var msgBuilder = new System.Text.StringBuilder();
                    msgBuilder.Append("Acct balance\t= ").Append(acctBalanceAmountValue.ToString("N2")).AppendLine();
                    msgBuilder.Append("Credit limit\t= ").Append(_selectedCustomer.CreditLimit.Value.ToString("N2")).AppendLine();
                    msgBuilder.Append("Balance     \t= ").Append((_selectedCustomer.CreditLimit - acctBalanceAmountValue).Value.ToString("N2")).AppendLine();
                    msgBuilder.Append("Do you want to override?");
                    var result = Utility.ShowMessageBoxWithOptions(msgBuilder.ToString(),System.Windows.MessageBoxButton.YesNoCancel);
                    if(result == System.Windows.MessageBoxResult.Yes)
                    {
                        var login = new View.Entitlements.Login();
                        var loginResult = login.ShowDialog();
                        if (RMSEntitiesHelper.Instance.IsAdmin(login.LoginVM.UserId) && loginResult.Value)
                            return;
                    }
                    CloseCommand.Execute(null);
                }
            }
        }

        public string SelectedCustomerText
        {
            get { return _selectedCustomerText; }
            set
            {
                _selectedCustomerText = value;
                //NotifyPropertyChanged(() => this._selectedCustomer);
                RaisePropertyChanged("SelectedCustomerText");
            }
        }
        
        #endregion

        #region TextContent

        private string _textContent = string.Empty;
        public string TextContent
        {
          get { return _textContent; }
          set
          {
            if (_textContent != value)
            {
              _textContent = value;
              RaisePropertyChanged("TextContent");
              
            }
          }
        }

        #endregion
       
        #region CloseCommand
      
        override protected bool OnClose()
        {
            if(SaleDetailList.Count() > 0 && !IsEditMode)
            {
                var options = Utility.ShowMessageBoxWithOptions(System.Windows.Application.Current.MainWindow,
                                                                "Unsaved items are available, do you want to save them?",
                                                                System.Windows.MessageBoxButton.YesNo);

                if (options == System.Windows.MessageBoxResult.Yes)
                {
                    if (!Validate()) return false;
                    SaveDataOnClosing();
                    _autoResetEvent.WaitOne();
                }
            }
            
            //Clear all the data if cancel is pressed
            //if (_salesParams.GetTemproaryData)
            //{
            //    var msgResult = Utility.ShowMessageBoxWithOptions("All data will not be saved. Do you want to cancel?");
            //    if (msgResult == System.Windows.MessageBoxResult.No) return false;
            //    RemoveTempSalesItemForGUID(_guid);
            //    _rmsEntities.SaveChanges();
            //}

            var returnValue = Workspace.This.Close(this);
            if (!returnValue) return false;

            RMSEntitiesHelper.Instance.RemoveNotifier(this);

            //_rmsEntities.Dispose();
            //if (_timer != null)
            //{
            //    _timer.Stop();
            //    _timer.Close();
            //    _timer.Dispose();
            //}

            //if (_autoTimer != null)
            //{
            //    _autoTimer.Stop();
            //    _autoTimer.Close();
            //    _autoTimer.Dispose();
            if (_autoResetEvent != null)
            {
                _autoResetEvent.Close();
                _autoResetEvent.Dispose();
            }

            return true;
            //}
        }

        private async void SaveDataOnClosing()
        {            
            await OnSave(SaveOperations.SaveOnWindowClosing);                        
        }
        #endregion

        #region CancelBillCommand
        RelayCommand<object> _cancelBillCommand = null;
        public ICommand CancelBillCommand
        {
            get
            {
                if (_cancelBillCommand == null)
                {
                    _cancelBillCommand = new RelayCommand<object>((p) => OnBillCancel(), (p) => CanBillCancel());
                }

                return _cancelBillCommand;
            }
        }

        private bool CanBillCancel()
        {
            return _salesParams.Billno != null;
        }

        private void OnBillCancel()
        {
            using (var rmsEntities = new RMSEntities())
            {
                var cancelBill = rmsEntities.Sales.FirstOrDefault(s => s.RunningBillNo == _salesParams.Billno && s.CustomerId == _salesParams.CustomerId);

                var msgResult = Utility.ShowMessageBoxWithOptions("Do you want to cancel the bill?");
                if (msgResult == System.Windows.MessageBoxResult.No || msgResult == System.Windows.MessageBoxResult.Cancel) return;

                var cancelBillItems = rmsEntities.SaleDetails.Where(s => s.BillId == cancelBill.BillId);
                foreach (var item in cancelBillItems.ToList())
                {
                    var stockItem = rmsEntities.Stocks.FirstOrDefault(st => st.ProductId == item.ProductId && st.PriceId == item.PriceId);
                    var stockTrans = GetStockTransation(rmsEntities,stockItem, cancelBill.AddedOn.Value);
                    //if (item.Product.SupportsMultiPrice.HasValue && item.Product.SupportsMultiPrice.Value)
                    //var stockTransList = rmsEntities.StockTransactions.Where(str => str.StockId == stockItem.Id);
                    //foreach (var stkTrnsItem in stockTransList)
                    //{
                    //    if(stkTrnsItem.AddedOn.Value.Subtract(cancelBill.AddedOn.Value).Days == 0)
                    //    {
                    //        stockTrans = stkTrnsItem;
                    //        break;
                    //    }
                    //}
                    //else
                    //  stockTrans = rmsEntities.StockTransactions.FirstOrDefault(str => str.StockId == stockItem.Id);


                     //&& str.AddedOn.Value.Date.Month == cancelBill.AddedOn.Value.Date.Month
                     //                                                          && str.AddedOn.Value.Date.Day == cancelBill.AddedOn.Value.Date.Day
                     //                                                          && str.AddedOn.Value.Date.Year == cancelBill.AddedOn.Value.Date.Year
                    var saleQty = item.Qty.Value;
                    if (stockTrans != null)
                    {
                        var actualStockTrans = rmsEntities.StockTransactions.FirstOrDefault(st => st.Id == stockTrans.Id);
                        if (actualStockTrans != null)
                        {
                            actualStockTrans.Outward -= saleQty;
                            actualStockTrans.ClosingBalance += saleQty;
                            actualStockTrans.SalesPurchaseCancelQty = saleQty;
                        }
                    }
                    stockItem.Quantity += saleQty;
                    stockItem.UpdatedBy = EntitlementInformation.UserInternalId;
                }

                //Remove the amount from customer balance due and from pending amounts
                var pendingPayments = rmsEntities.PaymentDetails.FirstOrDefault(p => p.BillId == cancelBill.BillId && p.CustomerId == _salesParams.CustomerId);
                if (pendingPayments != null)
                {
                    rmsEntities.PaymentDetails.Remove(pendingPayments);
                    var customer = rmsEntities.Customers.FirstOrDefault(c => c.Id == _salesParams.CustomerId);
                    customer.BalanceDue -= cancelBill.TotalAmount;
                }
                cancelBill.IsCancelled = true;
                cancelBill.UpdatedBy = EntitlementInformation.UserInternalId;
                rmsEntities.SaveChanges();
            }
            OnClose();
        }

        private StockTransaction GetStockTransation(RMSEntities rmsEntities, Stock stock,DateTime transDateTime)
        {
            var query = "Select * from stockTransaction where StockId =  " + stock.Id + " and Date(addedon) ='" + transDateTime.ToString("yyyy-MM-dd") + "'";
            return rmsEntities.Database.SqlQuery<StockTransaction>(query).FirstOrDefault();
        }

        #endregion

        #region SaveCommand
        RelayCommand<object> _saveCommand = null;
         public ICommand SaveCommand
        {
          get
          {
            if (_saveCommand == null)
            {
              _saveCommand = new RelayCommand<object>(async (p) => await OnSave(p), (p) =>
              {
                  return _selectedCustomer != null && _selectedCustomer.Id != 0 && SaleDetailList.Count != 0 &&
                    SaleDetailList[0].ProductId != 0 && _selectedCustomerText == _selectedCustomer.Name;
              });
            }

            return _saveCommand;
          }
        }
       
        
        private async Task OnSave(object parameter)
        {
            short paramValue = Convert.ToInt16(parameter);

            //Validate for Errors
            if (!_isEditMode && !Validate()) return;
            if (_isEditMode)
            {                                    
                SaveOnEdit(paramValue);
                return;
            }

            PanelLoading = true;

            try
            {
                var salesSaveTask = Task.Factory.StartNew(() =>
                    {
                        using (var rmsEntitiesSaveCtx = new RMSEntities())
                        {
                            using (DbContextTransaction dbTransaction = rmsEntitiesSaveCtx.Database.BeginTransaction())
                            {
                                try
                                {
                                    //Get the latest runningBill number with exclusive lock                       
                                    string sqlRunningNo = "select max(rollingno) + 1 from category cat where  cat.id = @p0  for Update";
                                    var nextRunningNo = rmsEntitiesSaveCtx.Database.SqlQuery<int>(sqlRunningNo, _categoryId).FirstOrDefault();

                                    _runningBillNo = nextRunningNo;
                                    _billSales.RunningBillNo = nextRunningNo;

                                    var _category = rmsEntitiesSaveCtx.Categories.FirstOrDefault(c => c.Id == _categoryId);
                                    _category.RollingNo = nextRunningNo;

                                    //rmsEntitiesSaveCtx.SaveChanges();

                                    _log.DebugFormat("Enter save :{0}", _runningBillNo);
                                    var combinedDateTime = GetCombinedDateTime();

                                    //Create new sales to avoid DB context Error
                                    var lclBillSales = new Sale
                                    {
                                        CustomerId = _selectedCustomer.Id,
                                        CustomerOrderNo = OrderNo,
                                        PaymentMode = SelectedPaymentId.ToString(),
                                        AddedOn = combinedDateTime,
                                        ModifiedOn = combinedDateTime,
                                        RunningBillNo = _runningBillNo,
                                        UpdatedBy = EntitlementInformation.UserInternalId
                                    };

                                    //_billSales.UpdatedBy = LoginViewModel.UserId;                                                

                                    foreach (var saleDetailItem in SaleDetailList)
                                    {
                                        if (saleDetailItem.ProductId == 0) continue;
                                        var saleDetail = new SaleDetail
                                        {
                                            Discount = saleDetailItem.Discount,
                                            PriceId = saleDetailItem.PriceId,
                                            ProductId = saleDetailItem.ProductId,
                                            Qty = saleDetailItem.Qty,
                                            SellingPrice = saleDetailItem.SellingPrice,
                                            BillId = lclBillSales.BillId,
                                            AddedOn = combinedDateTime,
                                            ModifiedOn = combinedDateTime,
                                            UpdatedBy = EntitlementInformation.UserInternalId
                                        };
                                        lclBillSales.SaleDetails.Add(saleDetail);

                                        var expiryDate = saleDetailItem.ExpiryDate;
                                        //var stock = rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetailItem.ProductId && s.PriceId == saleDetailItem.PriceId
                                        //                                               && s.ExpiryDate.Year == expiryDate.Value.Year
                                        //                                               && s.ExpiryDate.Month == expiryDate.Value.Month
                                        //                                               && s.ExpiryDate.Day == expiryDate.Value.Day
                                        //                                               );

                                        var stock = GetStockDetails(rmsEntitiesSaveCtx, saleDetailItem.ProductId, saleDetailItem.PriceId, expiryDate.Value);
                                        if (stock != null)
                                        {
                                            var actualStockEntity = rmsEntitiesSaveCtx.Stocks.First(s => s.Id == stock.Id);
                                            actualStockEntity.Quantity -= saleDetailItem.Qty.Value;
                                            actualStockEntity.UpdatedBy = EntitlementInformation.UserInternalId;
                                            SetStockTransaction(rmsEntitiesSaveCtx, saleDetail, actualStockEntity);
                                        }
                                    }

                                    //_totalAmount = _extensions.Calculate(_totalAmount.Value);

                                    CalculateTotalAmount();

                                    lclBillSales.TotalAmount = _totalAmount;
                                    lclBillSales.Discount = GetDiscountValue();
                                    lclBillSales.TransportCharges = _extensions.GetPropertyValue("TransportCharges", out decimal? oldValue);

                                    //rmsEntities.Entry(_billSales).State = EntityState.Added;

                                    //rmsEntitiesSaveCtx.Sales.Add(_billSales);

                                    //RemoveTempSalesItemForGUID(_guid);
                                    //this is done to get the latest bill no
                                    //RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId,false);
                                    //_billSales.RunningBillNo = _runningBillNo;


                                    var outstandingBalance = _totalAmount.Value - AmountPaid;
                                    if (outstandingBalance > 0 && SelectedPaymentId != 0)
                                    {
                                        lclBillSales.AmountPaid = _amountPaid;
                                        var custPaymentDetail = new PaymentDetail
                                        {
                                            AmountPaid = AmountPaid,
                                            CustomerId = SelectedCustomer.Id,
                                            Sale = lclBillSales,
                                            UpdatedBy = EntitlementInformation.UserInternalId,
                                            PaymentMode = 0,
                                            PaymentDate = GetCombinedDateTime()
                                        };

                                        rmsEntitiesSaveCtx.PaymentDetails.Add
                                        (
                                            custPaymentDetail
                                        );

                                        //_billSales.PaymentDetails.Add(custPaymentDetail);
                                        var customer = rmsEntitiesSaveCtx.Customers.FirstOrDefault(c => c.Id == _selectedCustomer.Id);
                                        customer.BalanceDue = customer.BalanceDue.HasValue ? customer.BalanceDue.Value + outstandingBalance : outstandingBalance;
                                    }

                                    rmsEntitiesSaveCtx.Sales.Add(lclBillSales);
                                    rmsEntitiesSaveCtx.SaveChanges();

                                    dbTransaction.Commit();

                                    _log.DebugFormat("Exit save :{0}", _runningBillNo);

                                    if (paramValue == SaveOperations.SavePrint)
                                    {
                                        var salesBillPrint = new SalesBillPrint(rmsEntitiesSaveCtx);
                                        salesBillPrint.Print(SelectedCustomer.Name, SaleDetailList.ToList(), lclBillSales, TotalAmount.Value,
                                                             AmountPaid, BalanceAmount, _showRestrictedCustomer);
                                    }
                                    //if (_salesParams.GetTemproaryData)
                                    //    CloseCommand.Execute(null);
                                    if (paramValue != SaveOperations.SaveOnWindowClosing)
                                    {
                                        GetProductsToOrder();
                                        Clear();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    dbTransaction.Rollback();
                                    _log.Error("Error while saving..!!", ex);
                                    throw ex;
                                }
                            }
                        }
                    }).ContinueWith(
                    (t) =>
                    {
                        PanelLoading = false;
                        if (!_autoResetEvent.SafeWaitHandle.IsClosed)
                        {
                            _autoResetEvent.Set();
                        }
                    });
                    await salesSaveTask;
            }
            catch (Exception ex)
            {
                if (paramValue == SaveOperations.SaveOnWindowClosing) return;
                Utility.ShowErrorBox("Error while saving..!!" + ex.Message);                
            }
            finally
            {
                PanelLoading = false;
                if (!_autoResetEvent.SafeWaitHandle.IsClosed)
                {
                    _autoResetEvent.Set();
                }
            }            
        }

       

        private DateTime GetCombinedDateTime()
        {
            DateTime combinedDateTime;
            //Get the current time since it takes the window open time
            DateTime date = _transcationDate.Date;
            TimeSpan time  = RMSEntitiesHelper.GetServerTime();
            //TimeSpan time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            combinedDateTime = date.Add(time);
            return combinedDateTime;
        }

        private Stock GetStockDetails(RMSEntities rmsEntities, int productId,int priceId, DateTime dateToCompare)
        {
            var query = "select * from stocks where ProductId = " + productId +
                        " and PriceId =" + priceId +
                        " and date(ExpiryDate) = '" + dateToCompare.ToString("yyyy-MM-dd") + "'";

            return rmsEntities.Database.SqlQuery<Stock>(query).FirstOrDefault();
        }

        private void GetProductsToOrder()
        {
            var query = "GetProductsToOrderForProductIds";

            using (var conn = MySQLDataAccess.GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    
                    string str = string.Join(",", SaleDetailList.Select(s => s.ProductId.ToString()));
                    var companySqlParam = new MySqlParameter("productsIn", MySql.Data.MySqlClient.MySqlDbType.VarString)
                    {
                        Value = str
                    };

                    cmd.Parameters.Add(companySqlParam);

                    List<ProductOrder> lstProductOrder = new List<ProductOrder>();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (!rdr.HasRows) return;
                        while (rdr.Read())
                        {
                            lstProductOrder.Add(new ProductOrder()
                            {
                                ProductName = rdr.GetString(0),
                                StockQuantity = rdr.GetDecimal(1),
                                CategoryName = rdr.GetString(2),
                                ReorderPoint = rdr.GetDecimal(3)
                            });
                        }
                    }

                    string[,] properties = new string[3, 2] {{"Name","ProductName"},
                                                              {"Stock","StockQuantity"},
                                                              {"Reorder","ReorderPoint"}};

                    var nvm = new Notification.NotificationViewModel<ProductOrder>(lstProductOrder, properties);                    
                    nvm.ExecuteShowWindow();                    
                }
            }
        }

        private bool Validate()
        {
            var duplicateResult = SaleDetailList.GroupBy(x => x.ProductId)
              .Where(g => g.Count() > 1)
              .ToList();

            if (duplicateResult != null && duplicateResult.Count() > 0)
            {
                var duplicateSaleItem = duplicateResult.First().First();
                var duplicateItemIndex = SaleDetailList.IndexOf(duplicateSaleItem);
                var duplicateProduct = ProductsPriceList.FirstOrDefault(p => p.ProductId == duplicateSaleItem.ProductId);

                Utility.ShowErrorBox("'" + duplicateProduct.ProductName +  "' Product appears more than Once at location " + duplicateItemIndex + 1 + ".Please check");
                return false;
            }

            foreach (var saleDetailItem in SaleDetailList)
            {
                if (saleDetailItem.ProductId == 0) continue;

                if (saleDetailItem.Qty < 0 || saleDetailItem.SellingPrice == 0) continue;

                if(!saleDetailItem.Qty.HasValue || !saleDetailItem.Amount.HasValue )
                {
                    Utility.ShowErrorBox("Product Quantity or Amount can't be null");
                    return false;
                }

                if(saleDetailItem.SellingPrice < ((saleDetailItem.CostPrice * 3/100) + saleDetailItem.CostPrice))
                {
                    Utility.ShowErrorBox("Selling Price can't be lower than 3% of cost price");
                    return false;
                }

                if (saleDetailItem.SellingPrice > ((saleDetailItem.CostPrice * 500/100) + saleDetailItem.CostPrice))
                {
                    Utility.ShowErrorBox("Selling Price can't be more than 500% of cost price");
                    return false;
                }

                return true;

                //var saleQty = saleDetailItem.Qty;
                //if ((saleQty == null || saleQty > saleDetailItem.AvailableStock || saleDetailItem.AvailableStock == 0)
                //    && (saleQty > 0))
                //{
                //    Utility.ShowErrorBox("Selling quantity can't be more than available quantity");
                //    return false;
                //}

                //using (var rmsEntities = new RMSEntities())
                //{
                //    var stock = rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetailItem.ProductId && s.PriceId == saleDetailItem.PriceId
                //                                                   && s.Quantity == saleDetailItem.AvailableStock);

                //    if (stock != null)
                //    {
                //        var stkQty = stock.Quantity;
                //        var saleQtyValue = saleDetailItem.Qty.Value;
                //        if (saleQtyValue > 0 && (stkQty - saleDetailItem.Qty.Value) < 0)
                //        {
                //            var product = rmsEntities.Products.Find(saleDetailItem.ProductId);
                //            var productName = "";
                //            if (product != null)
                //            {
                //                productName = product.Name;
                //            }
                //            Utility.ShowErrorBox("Stock available is less than sale quantity \nProduct Name: " + productName +
                //                                 "\nAvailable stock : " + stkQty + "\nSale Quantity :" + saleDetailItem.Qty.Value);
                //            return false;
                //        }
                //    }
                //}
            }
            return true;
        }

        //private void SaveInterim()
        //{                                   
        //    if (SelectedCustomer == null)
        //    {
        //        Utility.ShowErrorBox("Select a customer to save the items else items will not be saved");
        //        return;
        //    }

        //    var itemsToSave = _salesDetailsList.Take(3).ToList();
        //    if (itemsToSave.Count() < 3)
        //    {
        //        //_autoResetEvent.Set();
        //        return;
        //    }

        //    Monitor.Enter(rootLock);
        //    _log.Debug("Enter save :SaveInterim");

        //    var totalAmount = 0M;

        //    foreach (var saleDetailItem in itemsToSave)
        //    {
        //        if (saleDetailItem.ProductId == 0 || saleDetailItem.Qty == null ) continue;
        //        var saleDetail = new SaleDetail();
        //        saleDetail.Discount = saleDetailItem.Discount;
        //        saleDetail.PriceId = saleDetailItem.PriceId;
        //        saleDetail.ProductId = saleDetailItem.ProductId;
        //        saleDetail.Qty = saleDetailItem.Qty;
        //        saleDetail.SellingPrice = saleDetailItem.SellingPrice;
        //        saleDetail.BillId = _billSales.BillId;
        //        _billSales.SaleDetails.Add(saleDetail);

        //        var stockToReduceColln = _rmsEntities.Stocks.Where(s => s.ProductId == saleDetailItem.ProductId 
        //                                                                                    && s.PriceId == saleDetailItem.PriceId);
        //        var stock = stockToReduceColln.FirstOrDefault();
        //        if (stock != null)
        //        {
        //            stock.Quantity -= saleDetailItem.Qty.Value;
        //        }
        //        totalAmount += saleDetail.Qty.Value * saleDetail.SellingPrice.Value;
        //    }

        //    //_totalAmount = _extensions.Calculate(_totalAmount.Value);

        //    _billSales.TotalAmount = _billSales.TotalAmount == null ? totalAmount : _billSales.TotalAmount + totalAmount;
        //    //_billSales.TransportCharges = _extensions.GetPropertyValue("TransportCharges");

        //    if (_billSales.BillId == 0)
        //    {
        //        _billSales.CustomerId = _selectedCustomer.Id;
        //        _billSales.CustomerOrderNo = OrderNo;
        //        _billSales.RunningBillNo = _runningBillNo;
        //        _billSales.PaymentMode = SelectedPaymentId.ToString();

        //        _rmsEntities.Sales.Add(_billSales);
        //        var _category = _rmsEntities.Categories.FirstOrDefault(c => c.Id == _categoryId);
        //        _category.RollingNo = _runningBillNo;
        //        RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
        //        _billSales.RunningBillNo = _runningBillNo;
        //    }

        //    _rmsEntities.SaveChanges();

        //    itemsToSave.ForEach
        //        (
        //            (s) =>
        //            {
        //                App.Current.Dispatcher.Invoke(delegate 
        //                {
        //                    //var stateBefore = _rmsEntities.Entry<SaleDetail>(s.).State;
        //                    _salesDetailsList.Remove(s);
        //                    //var stateAfter = _rmsEntities.Entry<SaleDetail>(s).State;
        //                });
                       
        //            }
        //        );

        //    if (_salesDetailsList.Count() == 0)
        //    {
        //        App.Current.Dispatcher.Invoke(delegate
        //        {
        //            _salesDetailsList.Add(new SaleDetailExtn());
        //        });
        //    }

        //    Monitor.Exit(rootLock);
        //    _log.Debug("Exit SaveIternim");
        //    //_autoResetEvent.Set();                        
        //}


        private void SaveOnEdit(short parameter)
        {         
            PanelLoading = true;
            var salesSaveOnEditTask = System.Threading.Tasks.Task.Run(() =>
            {
                using (var rmsEntities = new RMSEntities())
                {
                    try
                    {
                        var combinedDateTime = GetCombinedDateTime();
                        _billSales.ModifiedOn = RMSEntitiesHelper.GetServerDate();
                        _billSales.CustomerId = _selectedCustomer.Id;
                        _billSales.CustomerOrderNo = OrderNo;
                        _billSales.PaymentMode = SelectedPaymentId.ToString();
                        _billSales.AddedOn = combinedDateTime;
                        _billSales.ModifiedOn = combinedDateTime;
                        _billSales.UpdatedBy = EntitlementInformation.UserInternalId;

                        SaleDetail saleDetail;

                        //Check if there are any deletions
                        RemoveDeletedItems(rmsEntities);
                        foreach (var saleDetailItemExtn in SaleDetailList)
                        {
                            saleDetail = rmsEntities.SaleDetails.FirstOrDefault(b => b.BillId == saleDetailItemExtn.BillId
                                                                                 && b.ProductId == saleDetailItemExtn.ProductId
                                                                                 && b.PriceId == saleDetailItemExtn.PriceId);
                            var serverDate = RMSEntitiesHelper.GetServerDate();
                            //New item added
                            if (saleDetail == null)
                            {
                                saleDetail = rmsEntities.SaleDetails.Create();
                                saleDetail.AddedOn = serverDate;
                                saleDetail.ModifiedOn = serverDate;
                                saleDetailItemExtn.OriginalQty = saleDetailItemExtn.Qty;
                                saleDetail.UpdatedBy = EntitlementInformation.UserInternalId;
                                rmsEntities.SaleDetails.Add(saleDetail);

                                SetSaleDetailItem(saleDetailItemExtn, saleDetail);

                                var stockNewItem = rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetail.ProductId && s.PriceId == saleDetail.PriceId);
                                if (stockNewItem != null)
                                {
                                    var stkQty = stockNewItem.Quantity;
                                    //if ((stkQty - saleDetail.Qty.Value) < 0)
                                    //{
                                    //    Utility.ShowErrorBox(" Stock available is less than sale quantity\nAvailable stock :" + stkQty + "\nSale Quantity :" + saleDetail.Qty.Value);
                                    //    return;
                                    //}
                                    stockNewItem.Quantity -= saleDetail.Qty.Value;
                                    stockNewItem.UpdatedBy = EntitlementInformation.UserInternalId;
                                    SetStockTransaction(rmsEntities, saleDetail, stockNewItem);
                                }
                                continue;
                            }

                            //Update existing item
                            SetSaleDetailItem(saleDetailItemExtn, saleDetail);
                            saleDetail.ModifiedOn = serverDate;

                            var stock = rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetail.ProductId && s.PriceId == saleDetail.PriceId);

                            if (stock != null)
                            {
                                var prd = rmsEntities.Products.Find(new object[1] { saleDetailItemExtn.ProductId });
                                var multiPriceSupport = prd.SupportsMultiPrice;
                                StockTransaction stockTransExisting = null;
                                if (multiPriceSupport.HasValue && multiPriceSupport.Value)
                                {
                                    var saleDate = saleDetail.AddedOn.Value.Date;
                                    //stockTransExisting = rmsEntities.StockTransactions.FirstOrDefault(st => st.StockId == stock.Id
                                    //                       && st.AddedOn.Value.Date.Day == saleDate.Day
                                    //                       && st.AddedOn.Value.Date.Month == saleDate.Month
                                    //                       && st.AddedOn.Value.Date.Year == saleDate.Year);

                                    //stockTransExisting = rmsEntities.StockTransactions.FirstOrDefault(st => st.StockId == stock.Id
                                    //&& DbFunctions.TruncateTime(st.AddedOn) == saleDate);

                                    foreach (var item in rmsEntities.StockTransactions)
                                    {
                                        if (item.StockId == stock.Id)
                                        {
                                            if (item.AddedOn == saleDate)
                                            {
                                                stockTransExisting = item;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                    stockTransExisting = rmsEntities.StockTransactions.FirstOrDefault(st => st.StockId == stock.Id);

                                if (saleDetailItemExtn.OriginalQty.Value > saleDetail.Qty.Value)
                                {
                                    var qty = saleDetailItemExtn.OriginalQty.Value - saleDetail.Qty.Value;
                                    //if (stock.Quantity - qty < 0)
                                    //{
                                    //    Utility.ShowErrorBox(" Stock available is less than sale quantity\nAvailable stock :" + stock.Quantity + "\nSale Quantity :" + qty);
                                    //    return;
                                    //}
                                    stock.Quantity -= qty;
                                    stockTransExisting.Outward -= qty;
                                    stockTransExisting.ClosingBalance -= qty;
                                }
                                else if (saleDetailItemExtn.OriginalQty.Value < saleDetail.Qty.Value)
                                {
                                    var qtyValue = saleDetail.Qty.Value - saleDetailItemExtn.OriginalQty.Value;
                                    //if (stock.Quantity - qtyValue < 0)
                                    //{
                                    //    Utility.ShowErrorBox(" Stock available is less than sale quantity\nAvailable stock :" + stock.Quantity + "\nSale Quantity :" + qtyValue);
                                    //    return;
                                    //}

                                    stock.Quantity += qtyValue;
                                    stockTransExisting.Outward += qtyValue;
                                    stockTransExisting.ClosingBalance += qtyValue;
                                }
                            }
                        }

                        //foreach (var slLogitem in _rmsEntities.SaleDetails)
                        //{
                        //    _log.Info("Bill Info :"  + slLogitem.BillId + "," + slLogitem.ProductId + "," + slLogitem.Qty + "," + slLogitem.PriceId + "," + slLogitem.SellingPrice);
                        //}

                        //_totalAmount = _extensions.Calculate(_totalAmount.Value);
                        //Find the sale item again as the context is different
                        var sale = rmsEntities.Sales.Find(_billSales.BillId);
                        //sale = _billSales;

                        sale.TotalAmount = _totalAmount;
                        sale.ModifiedOn = _billSales.ModifiedOn;
                        sale.PaymentMode = _billSales.PaymentMode;
                        sale.TransportCharges = _extensions.GetPropertyValue("TransportCharges", out decimal? oldvalue);
                        sale.CustomerOrderNo = _billSales.CustomerOrderNo;
                        sale.Discount = GetDiscountValue();
                        sale.UpdatedBy = EntitlementInformation.UserInternalId;
                        //This is for printing purpose
                        _billSales.TransportCharges = sale.TransportCharges;

                        rmsEntities.Entry(sale).State = EntityState.Modified;                        
                        rmsEntities.SaveChanges();
                        
                        if (parameter == SaveOperations.SavePrint)
                        {
                            var salesBillPrint = new SalesBillPrint(rmsEntities);
                            salesBillPrint.Print(_billSales.Customer.Name, SaleDetailList.ToList(), _billSales,
                                                 TotalAmount.Value, AmountPaid, BalanceAmount, _showRestrictedCustomer);
                        }
                        Clear();
                    }
                    catch (Exception ex)
                    {                        
                        Utility.ShowErrorBox("Error while saving on edit..!!" + ex.Message);
                        _log.Error("Error while saving on edit..!!", ex);
                        CloseCommand.Execute(null);
                    }
                }
            }).ContinueWith(
            (t) =>
            {
                PanelLoading = false;
                CloseCommand.Execute(null);
            });
        }

        private void RemoveDeletedItems(RMSEntities rmsEntities)
        {
            foreach (var saleDetailExtn in _deletedItems)
            {
                //New item added and removed
                if (saleDetailExtn.BillId == 0) continue;
                var saleDetail = rmsEntities.SaleDetails.FirstOrDefault(s => s.BillId == saleDetailExtn.BillId && s.ProductId == saleDetailExtn.ProductId);

                var stockNewItem = rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetail.ProductId && s.PriceId == saleDetail.PriceId);
                if (stockNewItem != null)
                {
                    var qty = saleDetail.Qty.Value;
                    var stockTrans = rmsEntities.StockTransactions.Where(s => s.StockId == stockNewItem.Id).OrderByDescending(s => s.AddedOn).FirstOrDefault();

                    stockTrans.Outward -= qty;
                    stockTrans.ClosingBalance += qty;

                    stockNewItem.Quantity += qty;
                }
                rmsEntities.SaleDetails.Remove(saleDetail);
            }
        }

        private void RemoveProductWithNullValues()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in SaleDetailList.Reverse())
                {
                    if (item.ProductId == 0)
                        SaleDetailList.Remove(item);
                }
            });
        }

        private void OnSalesDetailsListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                if (!(e.OldItems[0] is SaleDetailExtn SaleDetailExtn) || SaleDetailExtn.ProductId <= 0) return;
                if (_isEditMode)
                    _deletedItems.Add(SaleDetailExtn);
                _totalAmount -= SaleDetailExtn.Amount;
                RaisePropertyChanged("TotalAmount");

                var i = 0;
                foreach (var item in SaleDetailList)
                {
                    item.SerialNo = ++i;
                }
            }            
        }

        private void SetSaleDetailItem(SaleDetailExtn saleDetailItemExtn, SaleDetail saleDetail)
        {
            saleDetail.Discount = saleDetailItemExtn.Discount;
            saleDetail.PriceId = saleDetailItemExtn.PriceId;
            saleDetail.ProductId = saleDetailItemExtn.ProductId;
            saleDetail.Qty = saleDetailItemExtn.Qty; 
            saleDetail.SellingPrice = saleDetailItemExtn.SellingPrice;
            saleDetail.BillId = _billSales.BillId;
        }

        #endregion

        #region GetBill Command       

        private void OnEditBill(int? billNo)
        {
            if (billNo == null) throw new ArgumentNullException("Please enter a bill no");
            var runningBillNo = billNo.Value;

            _isEditMode = true;
            using (var rmsEntities = new RMSEntities())
            {
                _billSales = rmsEntities.Sales.Where(b => b.RunningBillNo == runningBillNo && b.CustomerId == _salesParams.CustomerId).FirstOrDefault();
                SelectedCustomer = _billSales.Customer;
                SelectedCustomerText = SelectedCustomer.Name;
                TranscationDate = _billSales.AddedOn.Value;
                SelectedPaymentId = char.Parse(_billSales.PaymentMode);
                OrderNo = _billSales.CustomerOrderNo;
                TotalDiscountAmount = _billSales.Discount;
                //var transport = _billSales.TransportCharges;
                //_extensions.SetValues

                var saleDetailsForBill = rmsEntities.SaleDetails.Where(b => b.BillId == _billSales.BillId);

                var tempTotalAmount = 0.0M;
                //var priceIdParam = new MySql.Data.MySqlClient.MySqlParameter("priceId", MySql.Data.MySqlClient.MySqlDbType.Int32);
                foreach (var saleDetailItem in saleDetailsForBill.ToList())
                {
                    //  priceIdParam.Value = saleDetailItem.PriceId;

                    var mySQLparam = new MySqlParameter("@priceId", MySql.Data.MySqlClient.MySqlDbType.Int32)
                    {
                        Value = saleDetailItem.PriceId
                    };

                    var productPrice = rmsEntities.Database.SqlQuery<ProductPrice>
                                           ("CALL GetProductPriceForPriceId(@priceId)", mySQLparam).FirstOrDefault();

                    //var productPrice = _productsPriceList.FirstOrDefault(p => p.PriceId == saleDetailItem.PriceId);
                    var discount = saleDetailItem.Discount ?? 0.00M;

                    var saleDetailExtn = new SaleDetailExtn()
                    {
                        DiscountAmount = discount,
                        PriceId = saleDetailItem.PriceId,
                        ProductId = saleDetailItem.ProductId,
                        Qty = saleDetailItem.Qty % 1 == 0 ? Math.Truncate(saleDetailItem.Qty.Value) : saleDetailItem.Qty,
                        OriginalQty = saleDetailItem.Qty,
                        //SellingPrice = saleDetailItem.SellingPrice,
                        BillId = saleDetailItem.BillId,
                        CostPrice = productPrice.Price,
                        AvailableStock = productPrice.Quantity,
                        Amount = (saleDetailItem.SellingPrice * saleDetailItem.Qty) - discount,
                        PropertyReadOnly = true
                    };

                    SaleDetailList.Add(saleDetailExtn);
                    SetSaleDetailExtn(productPrice, saleDetailExtn, 0);
                    saleDetailExtn.SellingPrice = saleDetailItem.SellingPrice;

                    tempTotalAmount += productPrice.SellingPrice * saleDetailItem.Qty.Value;
                }
                TotalAmount = tempTotalAmount;

                RunningBillNo = runningBillNo;

                if (_deletedItems == null)
                    _deletedItems = new List<SaleDetailExtn>();
                else
                    _deletedItems.Clear();
            }
        }
        #endregion

        #region Clear Command

        override internal void Clear()
        {
            //RefreshProductList();
            var defaultCustomerConfigName = ConfigurationManager.AppSettings["DefaultCustomer"];
            SelectedCustomer = _customerList.Where(c => c.Name == defaultCustomerConfigName).FirstOrDefault();
            SelectedCustomerText = SelectedCustomer.Name;
            SelectedPaymentId = '0';
            OrderNo = "";
            App.Current.Dispatcher.Invoke(() =>
            {
                SaleDetailList.Clear();
            });
            
            _billSales = new Sale();
            _totalAmount = 0;
            TotalAmount = null;
            AmountPaid = 0.0M;            
            _extensions.Clear();
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId,false);
            _isEditMode = false;
            TotalDiscountAmount = null;
            TotalDiscountPercent = null;
            IsVisible = System.Windows.Visibility.Collapsed;
            RefreshProductList();
        }

        #endregion

        #region Add Customer Command
        RelayCommand<object> _addCustomerCommand = null;

        public ICommand AddCustomerCommand
        {
            get
            {
                if (_addCustomerCommand == null)
                {
                    _addCustomerCommand = new RelayCommand<object>((p) => AddCustomer());
                }

                return _addCustomerCommand;
            }
        }

        private void AddCustomer()
        {
            Workspace.This.OpenCustomerCommand.Execute(null);
            RaisePropertyChanged("CustomersList");
        }

        #endregion

        RelayCommand<object> _CustomerChangedCommand = null;

        public ICommand CustomerChangedCommand
        {
            get
            {
                if (_CustomerChangedCommand == null)
                {
                    _CustomerChangedCommand = new RelayCommand<object>((p) => OnCustomerChanged());
                }

                return _CustomerChangedCommand;
            }
        }

        private void OnCustomerChanged()
        {
            Workspace.This.OpenCustomerCommand.Execute(null);
            RaisePropertyChanged("CustomersList");
        }

        

        #region Public Methods

        public void SetProductDetails(string barCodeNo, ProductPrice productPrice, int selectedIndex)
        {
            var productPriceForBarCode = _productsPriceList.FirstOrDefault(p => p.BarCodeNo == barCodeNo);

            if (productPriceForBarCode != null)
            {
                productPrice = productPriceForBarCode;
                
            }
            if (productPrice == null)
            {
                _log.Debug("SetProductDetails(); Product Price is null");
                return;
            }
            //var saleItem = SaleDetailList.FirstOrDefault(s => s.ProductId == productPrice.ProductId && s.PriceId == productPrice.PriceId);
            try
            {
                _log.Info("SetProductDetails:SelIndex:" + selectedIndex);
                if (selectedIndex == -1 || selectedIndex > SaleDetailList.Count - 1)
                {
                    _log.Info("Inside Return : selectedIndex" + selectedIndex);
                    _log.Info("Inside Return : SaleDetailList.Count" + SaleDetailList.Count);
                    SaleDetailList.Add(new SaleDetailExtn());
                    selectedIndex -= 1;
                    //SaleDetailList[selectedIndex] = ;
                    //return;
                }
                var selRowSaleDetailExtn = SaleDetailList[selectedIndex];
                selRowSaleDetailExtn.Clear();
                //selRowSaleDetailExtn

                //if (saleItem !=null)
                //{
                //    Utility.ShowWarningBox("Item is already added");
                //    selRowSaleDetailExtn.ProductId = 0;
                //    return;
                //}            
                SetSaleDetailExtn(productPrice, selRowSaleDetailExtn, selectedIndex);
            }
            catch (Exception ex)
            {
                _log.Error("Error on SetProductDetails", ex);
                _log.ErrorFormat("SelectedIndex: {0}. ProductId Id:{1} Price Id:{2}",selectedIndex,productPrice.ProductId,productPrice.PriceId);
            }
        }

        public void SetProductId()
        {
            foreach (var item in SaleDetailList)
            {
                item.OnPropertyChanged("ProductId");
            }
        }

        //public override Uri IconSource
        //{
        //    get
        //    {
        //        return null;
        //        // This icon is visible in AvalonDock's Document Navigator window
        //        //return new Uri("pack://application:,,,/Edi;component/Images/document.png", UriKind.RelativeOrAbsolute);
        //    }
        //}

        #endregion

        #region Private Methods

        private void SetStockTransaction(RMSEntities rmsEntities, SaleDetail saleDetail, Stock stockNewItem)
        {
            var stockTrans = rmsEntities.StockTransactions.Where(s => s.StockId == stockNewItem.Id).OrderByDescending(s => s.AddedOn).FirstOrDefault();

            //stock transaction not available for this product. Add them 
            if (stockTrans == null)
            {
                var firstStockTrans = new StockTransaction()
                {
                    OpeningBalance = stockNewItem.Quantity - saleDetail.Qty, //Opening balance will be the one from stock table 
                    Outward = saleDetail.Qty,
                    ClosingBalance = stockNewItem.Quantity,
                    StockId = stockNewItem.Id,
                    AddedOn = _transcationDate
                };
                //_rmsEntities.StockTransactions.Add(firstStockTrans);
                stockNewItem.StockTransactions.Add(firstStockTrans);
            }
            //stock transaction available. Check if it is for the current date else get the latest date and mark the opening balance
            else
            {
                var systemDBDate = RMSEntitiesHelper.Instance.GetSystemDBDate();
                var dateDiff = DateTime.Compare(stockTrans.AddedOn.Value.Date, systemDBDate);
                if (dateDiff == 0)
                {
                    stockTrans.Outward = saleDetail.Qty.Value + (stockTrans.Outward ?? 0);
                    stockTrans.ClosingBalance -= saleDetail.Qty;
                }
                else
                {
                    var newStockTrans = new StockTransaction()
                    {
                        OpeningBalance = stockTrans.ClosingBalance,
                        Outward = saleDetail.Qty,
                        ClosingBalance = stockTrans.ClosingBalance - saleDetail.Qty,
                        StockId = stockNewItem.Id,
                        AddedOn = _transcationDate                        
                    };
                    rmsEntities.StockTransactions.Add(newStockTrans);
                    //stockNewItem.StockTransactions.Add(newStockTrans);
                }
            }
        }

        private void SetSaleDetailExtn(ProductPrice productPrice, SaleDetailExtn SaleDetailExtn,int selectedIndex)
        {
            if (SaleDetailExtn != null)
            {
                SaleDetailExtn.SellingPrice = productPrice.SellingPrice;
                SaleDetailExtn.CostPrice = productPrice.Price;
                SaleDetailExtn.PriceId = productPrice.PriceId;
                SaleDetailExtn.AvailableStock = productPrice.Quantity;
                SaleDetailExtn.SerialNo = ++selectedIndex;
                SaleDetailExtn.ExpiryDate = DateTime.ParseExact(productPrice.ExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None);
                SaleDetailExtn.ProductId = productPrice.ProductId;
                //SaleDetailExtn.PriceToCalculate = SaleDetailExtn.SellingPrice;
            }
            //SaleDetailExtn.ProductName = productPrice.ProductName;

            //var customerSales = _rmsEntities.Sales.Where(s => s.CustomerId == _selectedCustomer.Id);//.OrderByDescending(d => d.ModifiedOn);
            var lastSoldPrice = RMSEntitiesHelper.Instance.GetLastSoldPrice(productPrice.ProductId, _selectedCustomer.Id);
            if (lastSoldPrice != null)
            {
                //var lastSaleDetail = customerSales. SaleDetails.Where(p => p.ProductId == productPrice.ProductId).OrderByDescending(d => d.ModifiedOn);
                //var lastSaleDetail1 = customerSales.SaleDetails.Where(p => p.ProductId == productPrice.ProductId);
                ////var lastSoldPrice = lastSaleDetail != null ? lastSaleDetail.SellingPrice : 0;

                SaleDetailExtn.LastSoldPrice = lastSoldPrice;
            }

            SaleDetailExtn.SubscribeToAmountChange(() =>
            {
                TotalAmount = SaleDetailList.Sum(a => a.Amount);
                //purchaseDetailExtn.CalculateCost(purchaseDetailExtn.FreeIssue);
            });
            //if (lastSoldDateByCustomer.Any())
            //{
            //    var lastSaleDetail = lastSoldDateByCustomer.FirstOrDefault().SaleDetails.Where(sd => sd.ProductId == productPrice.ProductId).FirstOrDefault();

            //    var lastSoldPrice = lastSaleDetail != null ? lastSaleDetail.SellingPrice : 0;

            //    SaleDetailExtn.LastSoldPrice = lastSoldPrice;
            //}


            //SaleDetailExtn.PropertyChanged += (sender, e) =>
            //{
            //    var prop = e.PropertyName;
            //    if (prop == Constants.AMOUNT)
            //    {
            //        TotalAmount = SaleDetailList.Sum(a => a.Amount);
            //        return;
            //    }
            //    var amount = SaleDetailExtn.SellingPrice * SaleDetailExtn.Qty;
            //    var discountAmount = SaleDetailExtn.DiscountPercentage != 0 ?
            //                         amount - (amount * (SaleDetailExtn.DiscountPercentage / 100)) :
            //                         SaleDetailExtn.DiscountAmount != 0 ?
            //                         amount - SaleDetailExtn.DiscountAmount :
            //                         0;

            //    if (discountAmount != 0)
            //    {
            //        SaleDetailExtn.Amount = discountAmount;
            //        SaleDetailExtn.Discount = amount - discountAmount;
            //        return;
            //    }

            //    SaleDetailExtn.Amount = amount;
            //    SaleDetailExtn.Discount = 0;
            //};
            //&& SaleDetailExtn.AvailableStock >
            //SaleDetailExtn.Qty.Value
            SaleDetailExtn.Qty = SaleDetailExtn.Qty ?? 1;
        }

        private void SaveDataTemp()
        {
            //_guid = Guid.NewGuid().ToString();          
            //_timer = new System.Timers.Timer();
            //_timer.Interval = 60000;
            //_timer.Start();            
            //_timer.Elapsed += (s, e) =>
            //{
            //    Monitor.Enter(rootLock);
            //    {
            //        log.DebugFormat("Entering timer loop :{0}", _guid);
            //        if (_salesDetailsList.Count() < 0 || SelectedCustomer == null)
            //        {
            //            Monitor.Exit(rootLock);
            //            return;
            //        }
            //        //Save to temp table  
            //        //IsDirty = true;
            //        //var deletedItems =_rmsEntities.SaleTemps.Where(i => _salesDetailsList.Contains(j => j.ProductId != i.ProductId) == true);                    
            //        var saleTempItems = _rmsEntities.SaleTemps.Where(g => g.Guid == _guid);

            //        foreach (var delItem in saleTempItems.ToList())
            //        {
            //            if (!_salesDetailsList.Any(p => p.ProductId == delItem.ProductId && delItem.Guid == _guid))
            //            {
            //                _rmsEntities.SaleTemps.Remove(delItem);
            //            }
            //        }

            //        foreach (var item in _salesDetailsList.ToList())
            //        {
            //            var tempItem = _rmsEntities.SaleTemps.FirstOrDefault(st => st.ProductId == item.ProductId && st.Guid == _guid);
            //            if (tempItem != null)
            //            {
            //                tempItem.SaleDate = _transcationDate;
            //                tempItem.CustomerId = _selectedCustomer == null ? -1 : _selectedCustomer.Id;
            //                tempItem.PaymentMode = SelectedPaymentId.ToString();
            //                tempItem.OrderNo = OrderNo;
            //                tempItem.ProductId = item.ProductId;
            //                tempItem.Quantity = item.Qty.HasValue == false? 0 : item.Qty;
            //                tempItem.SellingPrice = item.SellingPrice;
            //                tempItem.DiscountPercentage = item.DiscountPercentage;
            //                tempItem.DiscountAmount = item.DiscountAmount;
            //                tempItem.Amount = item.Amount;
            //                tempItem.PriceId = item.PriceId;
            //                continue;
            //            }

            //            var newSaletemp =
            //            new SaleTemp()
            //            {
            //                Guid = _guid,
            //                SaleDate = _transcationDate,
            //                CustomerId = _selectedCustomer == null ? -1 : _selectedCustomer.Id,
            //                PaymentMode = SelectedPaymentId.ToString(),
            //                OrderNo = OrderNo,
            //                ProductId = item.ProductId,
            //                Quantity = item.Qty,
            //                SellingPrice = item.SellingPrice,
            //                DiscountPercentage = item.DiscountPercentage,
            //                DiscountAmount = item.DiscountAmount,
            //                Amount = item.Amount,
            //                PriceId = item.PriceId
            //            };

            //            //    _rmsEntities.Entry(newSaletemp).State = System.Data.EntityState.Added;
            //            _rmsEntities.SaleTemps.Add(newSaletemp);

            //        }

            //        _rmsEntities.SaveChanges();
            //    }
            //    Monitor.Exit(rootLock);
            //    log.DebugFormat("Exit timer loop :{0}", _guid);
            //};
        }

        //private void RemoveTempSalesItemForGUID(string guid)
        //{
        //    var saleTempItems = _rmsEntities.SaleTemps.Where(g => g.Guid == _guid);

        //    foreach (var delItem in saleTempItems)
        //    {
        //        _rmsEntities.SaleTemps.Remove(delItem);
        //    }
        //}

        //private void GetTempDataFromDB(string guid)
        //{            
        //    var tempData = _rmsEntities.SaleTemps.Where(st => st.Guid == guid);
        //    var tempDataFirst = _rmsEntities.SaleTemps.Where(st => st.Guid == guid).FirstOrDefault();
        //    //var tempDataFirst 

        //    SelectedCustomer = _rmsEntities.Customers.FirstOrDefault(c => c.Id == tempDataFirst.CustomerId.Value);
        //    _categoryId = _rmsEntities.Categories.FirstOrDefault(c => c.Id == SelectedCustomer.CustomerTypeId).Id;
        //    SelectedCustomerText = SelectedCustomer.Name;
        //    _selectedPaymentMode = new PaymentMode();
        //    SelectedPaymentId = char.Parse(tempDataFirst.PaymentMode);
        //    RaisePropertyChanged("SelectedCustomer");
        //    //RaisePropertyChanged("SelectedPaymentMode");
        //    OrderNo = tempDataFirst.OrderNo;
        //    TranscationDate = tempDataFirst.SaleDate.Value;
        //    _guid = guid;

        //    var tempTotalAmount = 0.0M;
        //    foreach (var saleDetailItem in tempData)
        //    {
        //        var productPrice = _productsPriceList.Where(p => p.PriceId == saleDetailItem.PriceId).FirstOrDefault();
        //        var saleDetailExtn = new SaleDetailExtn()
        //        {
        //            Discount = saleDetailItem.DiscountAmount,
        //            DiscountPercentage = saleDetailItem.DiscountPercentage.Value,
        //            PriceId = saleDetailItem.PriceId.Value,
        //            ProductId = saleDetailItem.ProductId.Value,
        //            Qty = saleDetailItem.Quantity,
        //            OriginalQty = saleDetailItem.Quantity,
        //            SellingPrice = saleDetailItem.SellingPrice,
        //            CostPrice = productPrice.Price,
        //            AvailableStock = productPrice.Quantity,
        //            Amount = productPrice.SellingPrice * saleDetailItem.Quantity.Value
        //        };

        //        SaleDetailList.Add(saleDetailExtn);
        //        SetSaleDetailExtn(productPrice, saleDetailExtn,0);

        //        tempTotalAmount += productPrice.SellingPrice * saleDetailItem.Quantity.Value;
        //    }
        //    TotalAmount = tempTotalAmount;

        //    Title = "Unsaved  data ";
        //    RMSEntitiesHelper.Instance.AddNotifier(this);
        //    RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
        //}

        private void AutoSaveData()
        {
            //_autoTimer = new System.Timers.Timer();
            //_autoTimer.Interval = 30000;
            //_autoTimer.Start();
            //_autoTimer.Elapsed += (o, s) =>
            //    {                
            //        if (_salesDetailsList.Count() >= 3 && _salesDetailsList[2].Amount !=null)
            //        {
            //            if (_autoResetEvent == null)
            //            {
            //                _autoResetEvent = new AutoResetEvent(false);
            //            }
            //            else
            //                _autoResetEvent.WaitOne();

            //            SaveInterim();                    
            //        }
            //};
        }

        #endregion
    }
}
