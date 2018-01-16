using System;
using System.Linq;
using System.Windows.Input;
using System.Threading;
using log4net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Extensions;
using RetailManagementSystem.UserControls;
using System.Configuration;
using System.Data.Entity;

namespace RetailManagementSystem.ViewModel.Sales
{
    class SalesEntryViewModel : SalesViewModelbase
    {
        #region Private Variables
        static readonly ILog log = LogManager.GetLogger(typeof(SalesEntryViewModel));                                       
        Sale _billSales;                          
        
        IExtensions _extensions;   
        //System.Timers.Timer _timer,_autoTimer;
        static object rootLock = new object();
        string _guid;
        SalesParams _salesParams;
        List<Customer> _customerList;
        //AutoResetEvent _autoResetEvent;

        ObservableCollection<SaleDetailExtn> _salesDetailsList;        
        List<SaleDetailExtn> _deletedItems;
        Customer _selectedCustomer;
        string _selectedCustomerText;

        SalesBillPrint _salesBillPrint;
        RMSEntities _rmsEntities;
        #endregion

        #region Constructor

        public SalesEntryViewModel(SalesParams salesParams) : base(salesParams !=null ? salesParams.ShowAllCustomers : false)
        {
            _salesParams = salesParams;
            _rmsEntities = new RMSEntities();// RMSEntitiesHelper.Instance.RMSEntities;
            //var cnt = _rmsEntities.Customers.ToList();
            var cnt1 = _rmsEntities.Products.ToList();

            _salesBillPrint = new SalesBillPrint();
            _billSales = _rmsEntities.Sales.Create();
            _salesDetailsList = new ObservableCollection<SaleDetailExtn>();
            _salesDetailsList.CollectionChanged += OnSalesDetailsListCollectionChanged;

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
                    RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
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
                    GetTempDataFromDB(salesParams.Guid);
                    return;
                }
                //return;
            }
            
            //Title = "Sales Entry";
            RMSEntitiesHelper.Instance.AddNotifier(this);
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
            //SaveDataTemp();            
        }
       
        #endregion

        #region Getters and Setters

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                var defaultCustomerConfigName = ConfigurationManager.AppSettings["DefaultCustomer"];
                _customerList = new List<Customer>();
                var cnt = _rmsEntities.Customers.ToList().Count();

                _customerList = new List<Customer>(cnt);

                if (_salesParams.GetTemproaryData)
                {
                    foreach (var item in _rmsEntities.Customers)
                    {
                        if(item.CustomerTypeId == _categoryId)
                        {
                            _customerList.Add(item);
                        }

                    }
                    //_rmsEntities.Customers.Local.Where(c => c.CustomerTypeId == _categoryId);
                }
                if(_salesParams.ShowAllCustomers)
                {
                    foreach (var item in _rmsEntities.Customers)
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
                    foreach (var item in _rmsEntities.Customers)
                    {
                        if (item.CustomerTypeId == Constants.CUSTOMERS_HOTEL)
                        {
                            _customerList.Add(item);
                        }
                    }
                }
                    //customerList  = _rmsEntities.Customers.Local.Where(c => c.CustomerTypeId != Constants.CUSTOMERS_OTHERS);

                var defaultCustomerByConfig = _customerList.FirstOrDefault(c => c.Name.ToUpper() == defaultCustomerConfigName.ToUpper());
                if(defaultCustomerByConfig != null)
                {
                    DefaultCustomer = defaultCustomerByConfig;
                }
                return _customerList.OrderBy(a=> a.Name);
            }
        }
         
        public ObservableCollection<SaleDetailExtn> SaleDetailList
        {
            get { return _salesDetailsList; }
            private set
            {
                _salesDetailsList = value;
            }
        }                                      

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
            decimal? tempTotal = _salesDetailsList.Sum(a => a.Amount);
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
                decimal? oldTransportValue = 0.0M;
                var transportCharges = _extensions.GetPropertyValue("TransportCharges", out oldTransportValue);
                tempTotal = tempTotal + (transportCharges);
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
                _selectedCustomer = value;
                RaisePropertyChanged("SelectedCustomer");
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
        RelayCommand<object> _closeCommand = null;
        override public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand<object>((p) => OnClose(), (p) => CanClose());
                }

                return _closeCommand;
            }
        }

        private bool CanClose()
        {
            return true;
        }

        private new void  OnClose()
        {
            //Clear all the data if cancel is pressed
            if (_salesParams.GetTemproaryData)
            {
                var msgResult = Utility.ShowMessageBoxWithOptions("All data will not be saved. Do you want to cancel?");
                if (msgResult == System.Windows.MessageBoxResult.No) return;
                RemoveTempSalesItemForGUID(_guid);
                _rmsEntities.SaveChanges();
            }

            var returnValue = Workspace.This.Close(this);
            if (!returnValue) return;

            RMSEntitiesHelper.Instance.RemoveNotifier(this);
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
            //    if (_autoResetEvent != null)
            //    {
            //        _autoResetEvent.Close();
            //        _autoResetEvent.Dispose();
            //    }
            //}
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
            var cancelBill = _rmsEntities.Sales.FirstOrDefault(s => s.RunningBillNo == _salesParams.Billno);            

            var msgResult = Utility.ShowMessageBoxWithOptions("Do you want to cancel the bill?");
            if (msgResult == System.Windows.MessageBoxResult.No || msgResult == System.Windows.MessageBoxResult.Cancel) return;
            
            var cancelBillItems = _rmsEntities.SaleDetails.Where(s => s.BillId == cancelBill.BillId);            
            foreach (var item in cancelBillItems.ToList())
            {                
                var stockItem = _rmsEntities.Stocks.FirstOrDefault(st => st.ProductId == item.ProductId && st.PriceId == item.PriceId);
                var stockTrans = _rmsEntities.StockTransactions.AsEnumerable().FirstOrDefault(str => str.StockId == stockItem.Id
                                                                            && str.AddedOn.Value.Date == cancelBill.AddedOn.Value.Date);
                var saleQty = item.Qty.Value;
                stockTrans.Outward -= saleQty;
                stockTrans.ClosingBalance += saleQty;

                stockItem.Quantity += saleQty;
            }

            cancelBill.IsCancelled = true;
            _rmsEntities.SaveChanges();
            OnClose();
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
              _saveCommand = new RelayCommand<object>((p) => OnSave(p), (p) => CanSave(p));
            }

            return _saveCommand;
          }
        }
       
        public bool CanSave(object parameter)
        {
            return _selectedCustomer != null && _selectedCustomer.Id != 0 && _salesDetailsList.Count != 0 &&
                    _salesDetailsList[0].ProductId != 0 && _selectedCustomerText == _selectedCustomer.Name;
        }

        //private bool IsValid(DependencyObject obj)
        //{
        //    The dependency object is valid if it has no errors and all
        //    of its children (that are dependency objects) are error-free.
        //    return !Validation.QuantityValidationRule.GetHasError(obj) &&
        //    LogicalTreeHelper.GetChildren(obj)
        //    .OfType<DependencyObject>()
        //    .All(IsValid);
        //}

        private void OnSave(object parameter)
        {
            PanelLoading = true;
            var purchaseSaveTask = System.Threading.Tasks.Task.Run(() =>
            {
                //using (var dbTrans = _rmsEntities.Database.BeginTransaction())
                //{
                //    try
                //    {
                        log.DebugFormat("Enter save :{0}", _guid);
                        _billSales.CustomerId = _selectedCustomer.Id;
                        _billSales.CustomerOrderNo = OrderNo;
                        _billSales.RunningBillNo = _runningBillNo;
                        _billSales.PaymentMode = SelectedPaymentId.ToString();
                        _billSales.AddedOn = _transcationDate;
                        _billSales.ModifiedOn = _transcationDate;

                        if (_isEditMode)
                        {
                            SaveOnEdit();
                            return;
                        }

                        foreach (var saleDetailItem in _salesDetailsList)
                        {
                            if (saleDetailItem.ProductId == 0) continue;
                            if (saleDetailItem.Qty == null || saleDetailItem.Qty > saleDetailItem.AvailableStock || saleDetailItem.AvailableStock == 0)
                            {
                                Utility.ShowErrorBox("Selling quantity can't be more than available quantity");
                                return;
                            }
                            var saleDetail = _rmsEntities.SaleDetails.Create();
                            saleDetail.Discount = saleDetailItem.Discount;
                            saleDetail.PriceId = saleDetailItem.PriceId;
                            saleDetail.ProductId = saleDetailItem.ProductId;
                            saleDetail.Qty = saleDetailItem.Qty;
                            saleDetail.SellingPrice = saleDetailItem.SellingPrice;
                            saleDetail.BillId = _billSales.BillId;
                            saleDetail.AddedOn = _transcationDate;
                            saleDetail.ModifiedOn = _transcationDate;
                            _billSales.SaleDetails.Add(saleDetail);

                            var stockToReduceColln = _rmsEntities.Stocks.Where(s => s.ProductId == saleDetailItem.ProductId && s.PriceId == saleDetailItem.PriceId);
                            var stock = stockToReduceColln.FirstOrDefault();

                            if (stock != null)
                            {
                                stock.Quantity -= saleDetailItem.Qty.Value;
                                SetStockTransaction(saleDetail, stock);
                            }
                        }

                        //_totalAmount = _extensions.Calculate(_totalAmount.Value);

                        _billSales.TotalAmount = _totalAmount;
                        _billSales.Discount = GetDiscountValue();
                        decimal? oldValue;
                        _billSales.TransportCharges = _extensions.GetPropertyValue("TransportCharges", out oldValue);

                        _rmsEntities.Entry(_billSales).State = EntityState.Added;

                        //_rmsEntities.Sales.Add(_billSales);

                        RemoveTempSalesItemForGUID(_guid);
                        //this is done to get the latest bill no
                        RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
                        _billSales.RunningBillNo = _runningBillNo;

                        var _category = _rmsEntities.Categories.FirstOrDefault(c => c.Id == _categoryId);
                        _category.RollingNo = _runningBillNo;

                        //_rmsEntities.SaveChanges();

                        var outstandingBalance = _totalAmount.Value - AmountPaid;
                        if (outstandingBalance > 0)
                        {
                            //var msg = "Outstanding balance Rs " + outstandingBalance.ToString("N2") + ". Do you want to keep as pending balance amount?";
                            //var result = Utility.ShowMessageBoxWithOptions(msg);

                            //if (result == System.Windows.MessageBoxResult.Cancel) return;

                            //if (result == System.Windows.MessageBoxResult.Yes)
                            //{
                            //Customer cust = new Customer();
                            //cust = SelectedCustomer;
                            //Sale saleNew = new Sale();
                            //saleNew = _billSales;
                            var custPaymentDetail = new PaymentDetail
                            {
                                AmountPaid = AmountPaid,
                                CustomerId = SelectedCustomer.Id,
                                //BillId = _billSales.BillId,
                                //Customer = SelectedCustomer,
                                Sale = _billSales,
                                PaymentMode = "Cash"
                            };

                            _rmsEntities.PaymentDetails.Add
                                (
                                    custPaymentDetail
                                );


                            //_billSales.PaymentDetails.Add(custPaymentDetail);
                            var customer = _rmsEntities.Customers.FirstOrDefault(c => c.Id == _selectedCustomer.Id);
                            customer.BalanceDue = customer.BalanceDue.HasValue ? customer.BalanceDue.Value + outstandingBalance : outstandingBalance;
                        }

                    _rmsEntities.SaveChanges();
                    //dbTrans.Commit();
                    //Monitor.Exit(rootLock);
                    log.DebugFormat("Exit save :{0}", _guid);

                    if (parameter == null)
                        _salesBillPrint.Print(_billSales.Customer.Name, _salesDetailsList.ToList(), _billSales, AmountPaid, BalanceAmount, _showRestrictedCustomer);

                    if (_salesParams.GetTemproaryData)
                        _closeCommand.Execute(null);
                    Clear();

            //    //}
            //    //catch (Exception ex)
            //    //{
            //    //    if (dbTrans != null) dbTrans.Rollback();

            //    //    Utility.ShowErrorBox("Error while saving..!!" + ex.Message);
            //    //        log.Error(ex.Message, ex);
            //    //}
            //}
            }).ContinueWith(
            (t) =>
            {
                PanelLoading = false;
            });
        }       

        private void SaveInterim()
        {                                   
            if (SelectedCustomer == null)
            {
                Utility.ShowErrorBox("Select a customer to save the items else items will not be saved");
                return;
            }

            var itemsToSave = _salesDetailsList.Take(3).ToList();
            if (itemsToSave.Count() < 3)
            {
                //_autoResetEvent.Set();
                return;
            }

            Monitor.Enter(rootLock);
            log.Debug("Enter save :SaveInterim");

            var totalAmount = 0M;

            foreach (var saleDetailItem in itemsToSave)
            {
                if (saleDetailItem.ProductId == 0 || saleDetailItem.Qty == null ) continue;
                var saleDetail = new SaleDetail();
                saleDetail.Discount = saleDetailItem.Discount;
                saleDetail.PriceId = saleDetailItem.PriceId;
                saleDetail.ProductId = saleDetailItem.ProductId;
                saleDetail.Qty = saleDetailItem.Qty;
                saleDetail.SellingPrice = saleDetailItem.SellingPrice;
                saleDetail.BillId = _billSales.BillId;
                _billSales.SaleDetails.Add(saleDetail);

                var stockToReduceColln = _rmsEntities.Stocks.Where(s => s.ProductId == saleDetailItem.ProductId 
                                                                                            && s.PriceId == saleDetailItem.PriceId);
                var stock = stockToReduceColln.FirstOrDefault();
                if (stock != null)
                {
                    stock.Quantity -= saleDetailItem.Qty.Value;
                }
                totalAmount += saleDetail.Qty.Value * saleDetail.SellingPrice.Value;
            }

            //_totalAmount = _extensions.Calculate(_totalAmount.Value);

            _billSales.TotalAmount = _billSales.TotalAmount == null ? totalAmount : _billSales.TotalAmount + totalAmount;
            //_billSales.TransportCharges = _extensions.GetPropertyValue("TransportCharges");

            if (_billSales.BillId == 0)
            {
                _billSales.CustomerId = _selectedCustomer.Id;
                _billSales.CustomerOrderNo = OrderNo;
                _billSales.RunningBillNo = _runningBillNo;
                _billSales.PaymentMode = SelectedPaymentId.ToString();

                _rmsEntities.Sales.Add(_billSales);
                var _category = _rmsEntities.Categories.FirstOrDefault(c => c.Id == _categoryId);
                _category.RollingNo = _runningBillNo;
                RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
                _billSales.RunningBillNo = _runningBillNo;
            }

            _rmsEntities.SaveChanges();

            itemsToSave.ForEach
                (
                    (s) =>
                    {
                        App.Current.Dispatcher.Invoke(delegate 
                        {
                            //var stateBefore = _rmsEntities.Entry<SaleDetail>(s.).State;
                            _salesDetailsList.Remove(s);
                            //var stateAfter = _rmsEntities.Entry<SaleDetail>(s).State;
                        });
                       
                    }
                );

            if (_salesDetailsList.Count() == 0)
            {
                App.Current.Dispatcher.Invoke(delegate
                {
                    _salesDetailsList.Add(new SaleDetailExtn());
                });
            }

            Monitor.Exit(rootLock);
            log.Debug("Exit SaveIternim");
            //_autoResetEvent.Set();                        
        }


        private void SaveOnEdit()
        {
            //Check if there are any deletions
            RemoveDeletedItems();

            foreach (var saleDetailItemExtn in _salesDetailsList)
            {
                var saleDetail = _rmsEntities.SaleDetails.FirstOrDefault(b => b.BillId == saleDetailItemExtn.BillId
                                                                                                    && b.ProductId == saleDetailItemExtn.ProductId
                                                                                                    );
                if (saleDetail == null)
                {
                    saleDetail = _rmsEntities.SaleDetails.Create();
                    saleDetailItemExtn.OriginalQty = saleDetailItemExtn.Qty;
                    _rmsEntities.SaleDetails.Add(saleDetail);

                    SetSaleDetailItem(saleDetailItemExtn, saleDetail);

                    var stockNewItem = _rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetail.ProductId && s.PriceId == saleDetail.PriceId);
                    if (stockNewItem != null)
                    {
                        stockNewItem.Quantity -= saleDetail.Qty.Value;
                        SetStockTransaction(saleDetail, stockNewItem);
                    }
                    continue;
                }

                SetSaleDetailItem(saleDetailItemExtn, saleDetail);
                var stock = _rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetail.ProductId && s.PriceId == saleDetail.PriceId);

                if (stock != null)
                {
                    var stockTransExisting = _rmsEntities.StockTransactions.AsEnumerable().FirstOrDefault(st => st.StockId == stock.Id
                                               && st.AddedOn.Value.Date == saleDetail.AddedOn.Value.Date);

                    if (saleDetailItemExtn.OriginalQty.Value > saleDetail.Qty.Value)
                    {
                        var qty = saleDetailItemExtn.OriginalQty.Value - saleDetail.Qty.Value;
                        stock.Quantity -= qty;
                        stockTransExisting.Outward -= qty;
                        stockTransExisting.ClosingBalance -= qty;
                    }
                    else if(saleDetailItemExtn.OriginalQty.Value < saleDetail.Qty.Value)
                    {
                        var qtyValue = saleDetail.Qty.Value - saleDetailItemExtn.OriginalQty.Value;
                        stock.Quantity += qtyValue;
                        stockTransExisting.Outward += qtyValue;
                        stockTransExisting.ClosingBalance += qtyValue;
                    }
                }
            }

            _totalAmount = _extensions.Calculate(_totalAmount.Value);

            _billSales.TotalAmount = _totalAmount;
            decimal? oldvalue;
            _billSales.TransportCharges = _extensions.GetPropertyValue("TransportCharges", out oldvalue);

            _rmsEntities.SaveChanges();
            Clear();
            _closeCommand.Execute(null);
        }

        private void RemoveDeletedItems()
        {
            foreach (var saleDetailExtn in _deletedItems)
            {
                var saleDetail = _rmsEntities.SaleDetails.FirstOrDefault(s => s.BillId == saleDetailExtn.BillId && s.ProductId == saleDetailExtn.ProductId);

                var stockNewItem = _rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetail.ProductId && s.PriceId == saleDetail.PriceId);
                if (stockNewItem != null)
                {
                    var qty = saleDetail.Qty.Value;
                    var stockTrans = _rmsEntities.StockTransactions.Where(s => s.StockId == stockNewItem.Id).OrderByDescending(s => s.AddedOn).FirstOrDefault();
                
                    stockTrans.Outward -= qty;
                    stockTrans.ClosingBalance += qty;

                    stockNewItem.Quantity += qty;
                }
                _rmsEntities.SaleDetails.Remove(saleDetail);
            }
        }

        private void OnSalesDetailsListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var SaleDetailExtn = e.OldItems[0] as SaleDetailExtn;
                if(_isEditMode)
                    _deletedItems.Add(SaleDetailExtn);
                TotalAmount -= SaleDetailExtn.Amount;
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

            _billSales = _rmsEntities.Sales.Where(b => b.RunningBillNo == runningBillNo && b.CustomerId == _salesParams.CustomerId).FirstOrDefault();            
            SelectedCustomer = _billSales.Customer;
            SelectedCustomerText = SelectedCustomer.Name;
            TranscationDate = _billSales.AddedOn.Value;            
            SelectedPaymentId = char.Parse(_billSales.PaymentMode);
            OrderNo = _billSales.CustomerOrderNo;
            TotalDiscountAmount = _billSales.Discount;
            //var transport = _billSales.TransportCharges;
            //_extensions.SetValues
            
            var saleDetailsForBill = _rmsEntities.SaleDetails.Where(b => b.BillId == _billSales.BillId);            

            var tempTotalAmount = 0.0M;
            var priceIdParam = new MySql.Data.MySqlClient.MySqlParameter("priceId", MySql.Data.MySqlClient.MySqlDbType.Int32);
            foreach (var saleDetailItem in saleDetailsForBill.ToList())
            {
                priceIdParam.Value = saleDetailItem.PriceId;
                //var productPrice = _rmsEntities.Database.SqlQuery<ProductPrice>
                //                       ("CALL GetProductPriceForPriceId(@priceId)", priceIdParam).FirstOrDefault();

                var productPrice = _productsPriceList.FirstOrDefault(p => p.PriceId == saleDetailItem.PriceId);
                var discount = saleDetailItem.Discount.HasValue ? saleDetailItem.Discount.Value : 0.00M;

                var saleDetailExtn = new SaleDetailExtn()
                {
                    DiscountAmount = discount,
                    PriceId = saleDetailItem.PriceId,
                    ProductId = saleDetailItem.ProductId,
                    Qty = saleDetailItem.Qty,
                    OriginalQty = saleDetailItem.Qty,
                    SellingPrice = saleDetailItem.SellingPrice,
                    BillId = saleDetailItem.BillId,
                    CostPrice = productPrice.Price,
                    AvailableStock = productPrice.Quantity,
                    Amount = (productPrice.SellingPrice * saleDetailItem.Qty) - discount
                };

                SaleDetailList.Add(saleDetailExtn);
                SetSaleDetailExtn(productPrice, saleDetailExtn,0);

                tempTotalAmount += productPrice.SellingPrice * saleDetailItem.Qty.Value;
            }
            TotalAmount = tempTotalAmount;

            RunningBillNo = runningBillNo;
            _isEditMode = true;
            if (_deletedItems == null)
                _deletedItems = new List<SaleDetailExtn>();
            else
                _deletedItems.Clear();
        }
        #endregion

        #region Clear Command
        RelayCommand<object> _clearCommand = null;

        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand<object>((p) => Clear());
                }

                return _clearCommand;
            }
        }

        private void Clear()
        {
            _rmsEntities = new RMSEntities(); 
            RefreshProductList();
            var defaultCustomerConfigName = ConfigurationManager.AppSettings["DefaultCustomer"];
            SelectedCustomer = _customerList.Where(c => c.Name == defaultCustomerConfigName).FirstOrDefault();
            SelectedCustomerText = SelectedCustomer.Name;
            SelectedPaymentId = '0';
            OrderNo = "";
            SaleDetailList = new ObservableCollection<SaleDetailExtn>();
            _billSales = _rmsEntities.Sales.Create();     
            _totalAmount = 0;
            TotalAmount = null;
            AmountPaid = 0.0M;            
            _extensions.Clear();
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
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

        #region Public Methods
        public void SetProductDetails(ProductPrice productPrice, int selectedIndex)
        {
            if (productPrice == null) return;
            var saleItem = SaleDetailList.FirstOrDefault(s => s.ProductId == productPrice.ProductId && s.PriceId == productPrice.PriceId);
            var selRowSaleDetailExtn = SaleDetailList[selectedIndex];
            //if (saleItem !=null)
            //{
            //    Utility.ShowWarningBox("Item is already added");
            //    selRowSaleDetailExtn.ProductId = 0;
            //    return;
            //}            
            SetSaleDetailExtn(productPrice, selRowSaleDetailExtn, selectedIndex);
        }

        public void SetProductName()
        {
            foreach (var item in _salesDetailsList)
            {
                item.ProductName = item.ProductName;
            }
        }

        public override Uri IconSource
        {
            get
            {
                // This icon is visible in AvalonDock's Document Navigator window
                return new Uri("pack://application:,,,/Edi;component/Images/document.png", UriKind.RelativeOrAbsolute);
            }
        }
        #endregion

        #region Private Methods

        private void SetStockTransaction(SaleDetail saleDetail, Stock stockNewItem)
        {
            var rmsEntities = _rmsEntities;

            var stockTrans = rmsEntities.StockTransactions.Where(s => s.StockId == stockNewItem.Id).OrderByDescending(s => s.AddedOn).FirstOrDefault();

            //stock transaction not available for this product. Add them 
            if (stockTrans == null)
            {
                var firstStockTrans = new StockTransaction()
                {
                    OpeningBalance = stockNewItem.Quantity - saleDetail.Qty, //Opening balance will be the one from stock table 
                    Outward = saleDetail.Qty,
                    ClosingBalance = stockNewItem.Quantity,
                    StockId = stockNewItem.Id
                };
                //_rmsEntities.StockTransactions.Add(firstStockTrans);
                stockNewItem.StockTransactions.Add(firstStockTrans);
            }
            //stock transaction available. Check if it is for the current date else get the latest date and mark the opening balance
            else
            {
                var dateDiff = DateTime.Compare(stockTrans.AddedOn.Value.Date, DateTime.Now.Date);
                if (dateDiff == 0)
                {
                    stockTrans.Outward = saleDetail.Qty.Value + (stockTrans.Outward.HasValue ? stockTrans.Outward.Value : 0);
                    stockTrans.ClosingBalance -= saleDetail.Qty;
                }
                else
                {
                    var newStockTrans = new StockTransaction()
                    {
                        OpeningBalance = stockTrans.ClosingBalance,
                        Outward = saleDetail.Qty,
                        ClosingBalance = stockTrans.ClosingBalance - saleDetail.Qty,
                        StockId = stockNewItem.Id
                    };
                    //rmsEntities.StockTransactions.Add(newStockTrans);
                    stockNewItem.StockTransactions.Add(newStockTrans);
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
                SaleDetailExtn.SerialNo = selectedIndex;
                SaleDetailExtn.ProductName = productPrice.ProductName;

                //var customerSales = _rmsEntities.Sales.Where(s => s.CustomerId == _selectedCustomer.Id);//.OrderByDescending(d => d.ModifiedOn);
                var lastSoldPrice = RMSEntitiesHelper.Instance.GetLastSoldPrice(productPrice.ProductId, _selectedCustomer.Id);
                if (lastSoldPrice != null)
                {
                    //var lastSaleDetail = customerSales. SaleDetails.Where(p => p.ProductId == productPrice.ProductId).OrderByDescending(d => d.ModifiedOn);
                    //var lastSaleDetail1 = customerSales.SaleDetails.Where(p => p.ProductId == productPrice.ProductId);
                    ////var lastSoldPrice = lastSaleDetail != null ? lastSaleDetail.SellingPrice : 0;

                    SaleDetailExtn.LastSoldPrice = lastSoldPrice;
                }

                //if (lastSoldDateByCustomer.Any())
                //{
                //    var lastSaleDetail = lastSoldDateByCustomer.FirstOrDefault().SaleDetails.Where(sd => sd.ProductId == productPrice.ProductId).FirstOrDefault();

                //    var lastSoldPrice = lastSaleDetail != null ? lastSaleDetail.SellingPrice : 0;

                //    SaleDetailExtn.LastSoldPrice = lastSoldPrice;
                //}


                SaleDetailExtn.PropertyChanged += (sender, e) =>
                {
                    var prop = e.PropertyName;
                    if (prop == Constants.AMOUNT)
                    {
                        TotalAmount = SaleDetailList.Sum(a => a.Amount);
                        return;
                    }
                    var amount = SaleDetailExtn.SellingPrice * SaleDetailExtn.Qty;
                    var discountAmount = SaleDetailExtn.DiscountPercentage != 0 ?
                                         amount - (amount * (SaleDetailExtn.DiscountPercentage / 100)) :
                                         SaleDetailExtn.DiscountAmount != 0 ?
                                         amount - SaleDetailExtn.DiscountAmount :
                                         0;

                    if (discountAmount != 0)
                    {
                        SaleDetailExtn.Amount = discountAmount;
                        SaleDetailExtn.Discount = amount - discountAmount;
                        return;
                    }

                    SaleDetailExtn.Amount = amount;
                    SaleDetailExtn.Discount = 0;
                };
                //&& SaleDetailExtn.AvailableStock >
                //SaleDetailExtn.Qty.Value
                SaleDetailExtn.Qty = SaleDetailExtn.Qty.HasValue ? SaleDetailExtn.Qty.Value : 1;
            }
        }

        private bool CanSaveAs(object parameter)
        {
            return true;
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

        private void RemoveTempSalesItemForGUID(string guid)
        {
            var saleTempItems = _rmsEntities.SaleTemps.Where(g => g.Guid == _guid);

            foreach (var delItem in saleTempItems)
            {
                _rmsEntities.SaleTemps.Remove(delItem);
            }
        }

        private void GetTempDataFromDB(string guid)
        {            
            var tempData = _rmsEntities.SaleTemps.Where(st => st.Guid == guid);
            var tempDataFirst = _rmsEntities.SaleTemps.Where(st => st.Guid == guid).FirstOrDefault();
            //var tempDataFirst 

            SelectedCustomer = _rmsEntities.Customers.FirstOrDefault(c => c.Id == tempDataFirst.CustomerId.Value);
            _categoryId = _rmsEntities.Categories.FirstOrDefault(c => c.Id == SelectedCustomer.CustomerTypeId).Id;
            SelectedCustomerText = SelectedCustomer.Name;
            _selectedPaymentMode = new PaymentMode();
            SelectedPaymentId = char.Parse(tempDataFirst.PaymentMode);
            RaisePropertyChanged("SelectedCustomer");
            //RaisePropertyChanged("SelectedPaymentMode");
            OrderNo = tempDataFirst.OrderNo;
            TranscationDate = tempDataFirst.SaleDate.Value;
            _guid = guid;

            var tempTotalAmount = 0.0M;
            foreach (var saleDetailItem in tempData)
            {
                var productPrice = _productsPriceList.Where(p => p.PriceId == saleDetailItem.PriceId).FirstOrDefault();
                var saleDetailExtn = new SaleDetailExtn()
                {
                    Discount = saleDetailItem.DiscountAmount,
                    DiscountPercentage = saleDetailItem.DiscountPercentage.Value,
                    PriceId = saleDetailItem.PriceId.Value,
                    ProductId = saleDetailItem.ProductId.Value,
                    Qty = saleDetailItem.Quantity,
                    OriginalQty = saleDetailItem.Quantity,
                    SellingPrice = saleDetailItem.SellingPrice,
                    CostPrice = productPrice.Price,
                    AvailableStock = productPrice.Quantity,
                    Amount = productPrice.SellingPrice * saleDetailItem.Quantity.Value
                };

                SaleDetailList.Add(saleDetailExtn);
                SetSaleDetailExtn(productPrice, saleDetailExtn,0);

                tempTotalAmount += productPrice.SellingPrice * saleDetailItem.Quantity.Value;
            }
            TotalAmount = tempTotalAmount;

            Title = "Unsaved  data ";
            RMSEntitiesHelper.Instance.AddNotifier(this);
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
        }

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
