﻿using System;
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
using RetailManagementSystem.Interfaces;


namespace RetailManagementSystem.ViewModel.Sales
{
    class SalesEntryViewModel : DocumentViewModel,INotifier
    {
        #region Private Variables
        static readonly ILog log = LogManager.GetLogger(typeof(SalesEntryViewModel));        
        IEnumerable<PaymentMode> _paymentModes;       
        DateTime _saleDate;
        Customer _selectedCustomer;
        PaymentMode _selectedPaymentMode;
        Sale _billSales;
        int _runningBillNo;
        bool _showAllCustomers;
        int _categoryId;        
        decimal? _totalAmount = 0;        
        decimal? _totalDiscountPercent;
        decimal? _totalDiscountAmount;
        decimal _totalAmountDisplay = 0.0M;
        decimal _amountPaid = 0.0M;
        char _selectedPaymentId;        
        string _selectedCustomerText;
        IExtensions _extensions;
        bool _isEditMode;        
        System.Timers.Timer _timer,_autoTimer;
        static object rootLock = new object();
        string _guid;
        SalesParams _salesParams;
        AutoResetEvent _autoResetEvent;
        
        ObservableCollection<SaleDetailExtn> _salesDetailsList;
        IEnumerable<ProductPrice> _productsPriceList;
        List<SaleDetailExtn> _deletedItems;
        #endregion
 
        #region Constructor

        public SalesEntryViewModel(SalesParams salesParams)
        {
            _salesParams = salesParams;            
            var cnt = RMSEntitiesHelper.Instance.RMSEntities.Customers.ToList();
            var cnt1 = RMSEntitiesHelper.Instance.RMSEntities.Products.ToList();
            _saleDate = DateTime.Now;
            
            //var hotelRetailCategory = RMSEntitiesHelper.Instance.RMSEntities.Categories.FirstOrDefault(c => c.Id != 4);
            //_hotelCategoryId = hotelRetailCategory.Id;

            _showAllCustomers = salesParams.ShowAllCustomers;

            if (_showAllCustomers)
                _categoryId = Constants.CUSTOMERS_OTHERS;
            else
            {
                _categoryId = Constants.CUSTOMERS_HOTEL;
            }

            _paymentModes = new List<PaymentMode>(2)
            {
                new PaymentMode {PaymentId = '0',PaymentName="Cash" },
                new PaymentMode {PaymentId = '1',PaymentName="Credit" }
            };

            _billSales = RMSEntitiesHelper.Instance.RMSEntities.Sales.Create();

            _salesDetailsList = new ObservableCollection<SaleDetailExtn>();

            _salesDetailsList.CollectionChanged += OnSalesDetailsListCollectionChanged;

            _productsPriceList = RMSEntitiesHelper.Instance.GetProductPriceList();
            SelectedPaymentId = '0';

            if (salesParams != null)
            {
                //Temp window to save 10 items                
                if(salesParams.IsTempDataWindow)
                {
                    AutoSaveData();
                    Title = "Sales Entry";
                    RMSEntitiesHelper.Instance.AddNotifier(this);
                    RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
                    return;
                }

                if (salesParams.Billno.HasValue)
                {
                    //Amend Bill             
                    OnEditBill(salesParams.Billno.Value);
                    Title = "Sale Bill Amend :" + _runningBillNo;
                    return;
                }
                else if (salesParams.GetTemproaryData)
                {
                    //Get Temproary window from DB
                    GetTempDataFromDB();
                    return;
                }
                //return;
            }
            
            Title = "Sales Entry";
            RMSEntitiesHelper.Instance.AddNotifier(this);
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
            SaveDataTemp();            
        }
       
        #endregion

        #region Getters and Setters

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                if (_salesParams.GetTemproaryData)
                    return RMSEntitiesHelper.Instance.RMSEntities.Customers.Local.Where(c => c.CustomerTypeId ==  _categoryId);
                if(_salesParams.ShowAllCustomers)
                    return RMSEntitiesHelper.Instance.RMSEntities.Customers.Local.Where(c => c.CustomerTypeId == Constants.CUSTOMERS_OTHERS);

                return RMSEntitiesHelper.Instance.RMSEntities.Customers.Local.Where(c => c.CustomerTypeId != Constants.CUSTOMERS_OTHERS);
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

        public IEnumerable<ProductPrice> ProductsPriceList
        {
            get { return _productsPriceList; }
            private set
            {
                _productsPriceList = value;
                //NotifyPropertyChanged(() => this.ProductsPriceList);
                RaisePropertyChanged("ProductsPriceList");
            }
        }

        public IEnumerable<PaymentMode> PaymentModes
        {
            get { return _paymentModes; }
            private set
            {
                _paymentModes = value;
                RaisePropertyChanged("PaymentModes");
            }
        }

        public DateTime SaleDate
        {
            get { return _saleDate; }
            set
            {
                _saleDate = value;
                RaisePropertyChanged("SaleDate");
            }
        }

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
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

        public PaymentMode SelectedPaymentMode
        {
            get { return _selectedPaymentMode; }
            set
            {
                _selectedPaymentMode = value;
                RaisePropertyChanged("SelectedPaymentMode");
            }
        }

        public char SelectedPaymentId
        {
            get { return _selectedPaymentId ; }
            set
            {
                _selectedPaymentId = value;
                RaisePropertyChanged("SelectedPaymentId");
            }
        }

        public string OrderNo { get; set; }

        public int RunningBillNo
        {
            get { return _runningBillNo; }
            set
            {
                _runningBillNo = value;
                RaisePropertyChanged("RunningBillNo");
            }
        }

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
            decimal? tempTotal = _salesDetailsList.Sum(a => a.Amount); ;
            if (_totalDiscountAmount.HasValue)
            {
                tempTotal -= _totalDiscountAmount;
            }

            if (_totalDiscountPercent.HasValue)
            {
                var discountValue = tempTotal * (_totalDiscountPercent / 100);
                tempTotal -= discountValue;
            }

            _totalAmount = tempTotal;
            TotalAmountDisplay = _totalAmount.Value;
            RaisePropertyChanged("TotalAmount");
            RaisePropertyChanged("BalanceAmount");
        }

        public decimal TotalAmountDisplay
        {
            get { return _totalAmountDisplay; }
            set
            {
                _totalAmountDisplay = value;
                RaisePropertyChanged("TotalAmountDisplay");
            }

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
                        TotalAmountDisplay = _extensions.Calculate(_totalAmount.Value);
                    }
                };
            }
            get { return _extensions; }
        }

        //private void SetRunningBillNo()
        //{
        //    string sqlRunningNo = "select max(rollingno) + 1 from category cat where  cat.id = @p0";

        //    RunningBillNo = RMSEntitiesHelper.Instance.RMSEntities.Database.SqlQuery<int>(sqlRunningNo, _categoryId).FirstOrDefault();
        //}

        public decimal AmountPaid
        {
            get { return _amountPaid; }
            set
            {
                _amountPaid = value;
                RaisePropertyChanged("AmountPaid");
                RaisePropertyChanged("BalanceAmount");
            }
        }

        public decimal BalanceAmount
        {
            get { return Math.Abs(_amountPaid != 0 ? _totalAmount.Value - _amountPaid : 0.00M); }            
        }

        public bool IsEditMode
        {
            get { return !_isEditMode; }
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
              IsDirty = true;
            }
          }
        }

        #endregion

        #region IsDirty

        private bool _isDirty = false;
        override public bool IsDirty
        {
          get { return _isDirty; }
          set
          {
            if (_isDirty != value)
            {
              _isDirty = value;
              RaisePropertyChanged("IsDirty");
              RaisePropertyChanged("FileName");
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

        private void OnClose()
        {            
            var returnValue = Workspace.This.Close(this);
            if (!returnValue) return;

            RMSEntitiesHelper.Instance.RemoveNotifier(this);
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Close();
                _timer.Dispose();
            }

            if (_autoTimer != null)
            {
                _autoTimer.Stop();
                _autoTimer.Close();
                _autoTimer.Dispose();
                if (_autoResetEvent != null)
                {
                    _autoResetEvent.Close();
                    _autoResetEvent.Dispose();
                }
            }
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
            var cancelBill = RMSEntitiesHelper.Instance.RMSEntities.Sales.FirstOrDefault(s => s.RunningBillNo == _salesParams.Billno);            

            var msgResult = Utility.ShowMessageBoxWithOptions("Do you want to cancel the bill?");
            if (msgResult == System.Windows.MessageBoxResult.No) return;
            
            var cancelBillItems = RMSEntitiesHelper.Instance.RMSEntities.SaleDetails.Where(s => s.BillId == cancelBill.BillId);            
            foreach (var item in cancelBillItems.ToList())
            {                
                var stockItem = RMSEntitiesHelper.Instance.RMSEntities.Stocks.FirstOrDefault(st => st.ProductId == item.ProductId && st.PriceId == item.PriceId);
                stockItem.Quantity += item.Qty.Value;
            }

            cancelBill.IsCancelled = true;
            RMSEntitiesHelper.Instance.RMSEntities.SaveChanges();
            OnClose();
        }
        #endregion

        #region SaveCommand
        RelayCommand<object> _saveCommand = null;
         override public ICommand SaveCommand
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
            //return IsDirty;
        }

        private void OnSave(object parameter)
        {
            Monitor.Enter(rootLock); 
            log.DebugFormat("Enter save :{0}", _guid);
            _billSales.CustomerId = _selectedCustomer.Id;
            _billSales.CustomerOrderNo = OrderNo;            
            _billSales.RunningBillNo = _runningBillNo;
            _billSales.PaymentMode = SelectedPaymentId.ToString();

            if (_isEditMode)
            {
                SaveOnEdit();
                return;
            }

            foreach (var saleDetailItem in _salesDetailsList)
            {
                if (saleDetailItem.ProductId == 0) continue;
                var saleDetail = RMSEntitiesHelper.Instance.RMSEntities.SaleDetails.Create();
                saleDetail.Discount = saleDetailItem.Discount;
                saleDetail.PriceId = saleDetailItem.PriceId;
                saleDetail.ProductId = saleDetailItem.ProductId;
                saleDetail.Qty = saleDetailItem.Qty;
                saleDetail.SellingPrice = saleDetailItem.SellingPrice;
                saleDetail.BillId = _billSales.BillId;                
                _billSales.SaleDetails.Add(saleDetail);

                var stockToReduceColln = RMSEntitiesHelper.Instance.RMSEntities.Stocks.Where(s => s.ProductId == saleDetailItem.ProductId && s.PriceId == saleDetailItem.PriceId);
                var stock = stockToReduceColln.FirstOrDefault();
                             
                if(stock != null)
                {                    
                        stock.Quantity -= saleDetailItem.Qty.Value;
                }                
            }

            _totalAmount = _extensions.Calculate(_totalAmount.Value);

            _billSales.TotalAmount = _totalAmount;
            _billSales.TransportCharges = _extensions.GetPropertyValue("TransportCharges");

           
            RMSEntitiesHelper.Instance.RMSEntities.Sales.Add(_billSales);

            var _category = RMSEntitiesHelper.Instance.RMSEntities.Categories.FirstOrDefault(c => c.Id == _categoryId);
            _category.RollingNo = _runningBillNo;
            

            //check if complete amount is paid, else mark it in PaymentDetails table against the customer
            var outstandingBalance = _totalAmount.Value - AmountPaid;
            if (outstandingBalance > 0)
            {
                var msg = "Outstanding balance Rs " + outstandingBalance + ". Do you want to keep as pending balance amount?";
                var result = Utility.ShowMessageBoxWithOptions(msg);
                if(result == System.Windows.MessageBoxResult.Yes)
                {
                    RMSEntitiesHelper.Instance.RMSEntities.PaymentDetails.Add
                        (
                            new PaymentDetail
                            {
                                BillId = _billSales.BillId,
                                AmountPaid = AmountPaid,
                                CustomerId = _selectedCustomer.Id                                
                            }
                        );
                }
                var customer = RMSEntitiesHelper.Instance.RMSEntities.Customers.FirstOrDefault(c => c.Id == _selectedCustomer.Id);
                customer.BalanceDue += outstandingBalance;
            }
                        
            
            RemoveTempSalesItemForGUID(_guid);
            //this is done to get the latest bill no
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
            _billSales.RunningBillNo = _runningBillNo;
            RMSEntitiesHelper.Instance.RMSEntities.SaveChanges();
            Monitor.Exit(rootLock);
            log.DebugFormat("Exit save :{0}", _guid);
            if (_salesParams.GetTemproaryData)
                _closeCommand.Execute(null);
            Clear();
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
                _autoResetEvent.Set();
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

                var stockToReduceColln = RMSEntitiesHelper.Instance.RMSEntities.Stocks.Where(s => s.ProductId == saleDetailItem.ProductId 
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

                RMSEntitiesHelper.Instance.RMSEntities.Sales.Add(_billSales);
                var _category = RMSEntitiesHelper.Instance.RMSEntities.Categories.FirstOrDefault(c => c.Id == _categoryId);
                _category.RollingNo = _runningBillNo;
                RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
                _billSales.RunningBillNo = _runningBillNo;
            }

            RMSEntitiesHelper.Instance.RMSEntities.SaveChanges();

            itemsToSave.ForEach
                (
                    (s) =>
                    {
                        App.Current.Dispatcher.Invoke(delegate 
                        {
                            //var stateBefore = RMSEntitiesHelper.Instance.RMSEntities.Entry<SaleDetail>(s.).State;
                            _salesDetailsList.Remove(s);
                            //var stateAfter = RMSEntitiesHelper.Instance.RMSEntities.Entry<SaleDetail>(s).State;
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
            _autoResetEvent.Set();                        
        }


        private void SaveOnEdit()
        {
            //Check if there are any deletions
            RemoveDeletedItems();

            foreach (var saleDetailItemExtn in _salesDetailsList)
            {
                var saleDetail = RMSEntitiesHelper.Instance.RMSEntities.SaleDetails.FirstOrDefault(b => b.BillId == saleDetailItemExtn.BillId
                                                                        && b.ProductId == saleDetailItemExtn.ProductId);

                if (saleDetail == null)
                {
                    saleDetail = RMSEntitiesHelper.Instance.RMSEntities.SaleDetails.Create();
                    saleDetailItemExtn.OriginalQty = saleDetailItemExtn.Qty;
                    RMSEntitiesHelper.Instance.RMSEntities.SaleDetails.Add(saleDetail);

                    SetSaleDetailItem(saleDetailItemExtn, saleDetail);

                    var stockNewItem = RMSEntitiesHelper.Instance.RMSEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetail.ProductId && s.PriceId == saleDetail.PriceId);
                    if (stockNewItem != null)
                    {
                        stockNewItem.Quantity -= saleDetail.Qty.Value;
                    }
                    continue;
                }

                SetSaleDetailItem(saleDetailItemExtn, saleDetail);
                var stock = RMSEntitiesHelper.Instance.RMSEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetail.ProductId && s.PriceId == saleDetail.PriceId);

                if (stock != null)
                {
                    stock.Quantity = (stock.Quantity + saleDetailItemExtn.OriginalQty.Value) - saleDetail.Qty.Value;
                }
            }

            _totalAmount = _extensions.Calculate(_totalAmount.Value);

            _billSales.TotalAmount = _totalAmount;
            _billSales.TransportCharges = _extensions.GetPropertyValue("TransportCharges");

            RMSEntitiesHelper.Instance.RMSEntities.SaveChanges();
            Clear();
            _closeCommand.Execute(null);
        }

        private void RemoveDeletedItems()
        {
            foreach (var saleDetailExtn in _deletedItems)
            {
                var saleDetail = RMSEntitiesHelper.Instance.RMSEntities.SaleDetails.FirstOrDefault(s => s.BillId == saleDetailExtn.BillId && s.ProductId == saleDetailExtn.ProductId);

                var stockNewItem = RMSEntitiesHelper.Instance.RMSEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetail.ProductId && s.PriceId == saleDetail.PriceId);
                if (stockNewItem != null)
                {
                    stockNewItem.Quantity += saleDetail.Qty.Value;
                }
                RMSEntitiesHelper.Instance.RMSEntities.SaleDetails.Remove(saleDetail);
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

        private void OnEditBill(object billNo)
        {
            //Clear();
            if (billNo == null) throw new ArgumentNullException("Please enter a bill no");
            var runningBillNo = Convert.ToInt32(billNo.ToString());

            _billSales = RMSEntitiesHelper.Instance.RMSEntities.Sales.Where(b => b.RunningBillNo == runningBillNo).FirstOrDefault();            
            SelectedCustomer = _billSales.Customer;
            SelectedCustomerText = SelectedCustomer.Name;
            SaleDate = _billSales.AddedOn.Value;            
            SelectedPaymentId = Char.Parse(_billSales.PaymentMode);
            OrderNo = _billSales.CustomerOrderNo;
            var saleDetailsForBill = RMSEntitiesHelper.Instance.RMSEntities.SaleDetails.Where(b => b.BillId == _billSales.BillId);            

            var tempTotalAmount = 0.0M;
            foreach (var saleDetailItem in saleDetailsForBill)
            {
                var productPrice = _productsPriceList.Where(p => p.PriceId == saleDetailItem.PriceId).FirstOrDefault();                
                var saleDetailExtn = new SaleDetailExtn()
                {
                    Discount = saleDetailItem.Discount,
                    PriceId = saleDetailItem.PriceId,
                    ProductId = saleDetailItem.ProductId,
                    Qty = saleDetailItem.Qty,
                    OriginalQty = saleDetailItem.Qty,
                    SellingPrice = saleDetailItem.SellingPrice,
                    BillId = saleDetailItem.BillId,
                    CostPrice = productPrice.Price,
                    AvailableStock = productPrice.Quantity,
                    Amount = productPrice.SellingPrice * saleDetailItem.Qty
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
            _productsPriceList = RMSEntitiesHelper.Instance.GetProductPriceList();
            SelectedCustomer = null;
            SelectedPaymentId = '0';
            OrderNo = "";
            SaleDetailList.Clear();
            _billSales = RMSEntitiesHelper.Instance.RMSEntities.Sales.Create();     
            _totalAmount = 0;
            TotalAmountDisplay =0.0M;
            AmountPaid = 0.0M;            
            _extensions.Clear();
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
            _isEditMode = false;
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
            if (saleItem !=null)
            {
                Utility.ShowWarningBox("Item is already added");
                selRowSaleDetailExtn.ProductId = 0;
                return;
            }            
            SetSaleDetailExtn(productPrice, selRowSaleDetailExtn, selectedIndex);
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
        private void SetSaleDetailExtn(ProductPrice productPrice, SaleDetailExtn SaleDetailExtn,int selectedIndex)
        {
            if (SaleDetailExtn != null)
            {
                //selectedRowSaleDetail.Qty = productPrice.Quantity;
                SaleDetailExtn.SellingPrice = productPrice.SellingPrice;
                SaleDetailExtn.CostPrice = productPrice.Price;
                SaleDetailExtn.PriceId = productPrice.PriceId;
                SaleDetailExtn.AvailableStock = productPrice.Quantity;
                SaleDetailExtn.SerialNo = selectedIndex;

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
                        SaleDetailExtn.Discount = discountAmount;
                        return;
                    }

                    SaleDetailExtn.Amount = amount;
                    SaleDetailExtn.Discount = 0;
                };
            }
        }

        private bool CanSaveAs(object parameter)
        {
            return IsDirty;
        }      

        void INotifier.Notify(int runningNo)
        {
            RunningBillNo = runningNo;   
        }

        private void SaveDataTemp()
        {
            _guid = Guid.NewGuid().ToString();          
            _timer = new System.Timers.Timer();
            _timer.Interval = 60000;
            _timer.Start();
            //var rmsEntityTemp = RMSEntitiesHelper.GetNewInstance();
            //RMSEntitiesHelper.Instance.RMSEntities.SaleTemps.ToList();
            _timer.Elapsed += (s, e) =>
            {
                Monitor.Enter(rootLock);
                {
                    log.DebugFormat("Entering timer loop :{0}", _guid);
                    if (_salesDetailsList.Count() < 0)
                    {
                        Monitor.Exit(rootLock);
                        return;
                    }
                    //Save to temp table  
                    IsDirty = true;
                    //var deletedItems =RMSEntitiesHelper.Instance.RMSEntities.SaleTemps.Where(i => _salesDetailsList.Contains(j => j.ProductId != i.ProductId) == true);                    
                    var saleTempItems = RMSEntitiesHelper.Instance.RMSEntities.SaleTemps.Where(g => g.Guid == _guid);

                    foreach (var delItem in saleTempItems.ToList())
                    {
                        if (!_salesDetailsList.Any(p => p.ProductId == delItem.ProductId && delItem.Guid == _guid))
                        {
                            RMSEntitiesHelper.Instance.RMSEntities.SaleTemps.Remove(delItem);
                        }
                    }

                    foreach (var item in _salesDetailsList.ToList())
                    {
                        var tempItem = RMSEntitiesHelper.Instance.RMSEntities.SaleTemps.FirstOrDefault(st => st.ProductId == item.ProductId && st.Guid == _guid);
                        if (tempItem != null)
                        {
                            tempItem.SaleDate = _saleDate;
                            tempItem.CustomerId = _selectedCustomer == null ? -1 : _selectedCustomer.Id;
                            tempItem.PaymentMode = SelectedPaymentId.ToString();
                            tempItem.OrderNo = OrderNo;
                            tempItem.ProductId = item.ProductId;
                            tempItem.Quantity = item.Qty;
                            tempItem.SellingPrice = item.SellingPrice;
                            tempItem.DiscountPercentage = item.DiscountPercentage;
                            tempItem.DiscountAmount = item.DiscountAmount;
                            tempItem.Amount = item.Amount;
                            tempItem.PriceId = item.PriceId;
                            continue;
                        }

                        var newSaletemp =
                        new SaleTemp()
                        {
                            Guid = _guid,
                            SaleDate = _saleDate,
                            CustomerId = _selectedCustomer == null ? -1 : _selectedCustomer.Id,
                            PaymentMode = SelectedPaymentId.ToString(),
                            OrderNo = OrderNo,
                            ProductId = item.ProductId,
                            Quantity = item.Qty,
                            SellingPrice = item.SellingPrice,
                            DiscountPercentage = item.DiscountPercentage,
                            DiscountAmount = item.DiscountAmount,
                            Amount = item.Amount,
                            PriceId = item.PriceId
                        };

                        //    RMSEntitiesHelper.Instance.RMSEntities.Entry(newSaletemp).State = System.Data.EntityState.Added;
                        RMSEntitiesHelper.Instance.RMSEntities.SaleTemps.Add(newSaletemp);

                    }

                    RMSEntitiesHelper.Instance.RMSEntities.SaveChanges();
                }
                Monitor.Exit(rootLock);
                log.DebugFormat("Exit timer loop :{0}", _guid);
            };
        }

        private void RemoveTempSalesItemForGUID(string guid)
        {
            var saleTempItems = RMSEntitiesHelper.Instance.RMSEntities.SaleTemps.Where(g => g.Guid == _guid);

            foreach (var delItem in saleTempItems)
            {
                RMSEntitiesHelper.Instance.RMSEntities.SaleTemps.Remove(delItem);
            }
        }

        private void GetTempDataFromDB()
        {
            var tempData = RMSEntitiesHelper.Instance.RMSEntities.SaleTemps.ToList();
            var tempDataFirst = RMSEntitiesHelper.Instance.RMSEntities.SaleTemps.ToList().FirstOrDefault();

            SelectedCustomer = RMSEntitiesHelper.Instance.RMSEntities.Customers.FirstOrDefault(c => c.Id == tempDataFirst.CustomerId.Value);
            _categoryId = RMSEntitiesHelper.Instance.RMSEntities.Categories.FirstOrDefault(c => c.Id == SelectedCustomer.CustomerTypeId).Id;
            SelectedCustomerText = SelectedCustomer.Name;
            _selectedPaymentMode = new PaymentMode();
            SelectedPaymentId = char.Parse(tempDataFirst.PaymentMode);
            RaisePropertyChanged("SelectedCustomer");
            //RaisePropertyChanged("SelectedPaymentMode");
            OrderNo = tempDataFirst.OrderNo;
            SaleDate = tempDataFirst.SaleDate.Value;
            _guid = tempDataFirst.Guid;

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
            _autoTimer = new System.Timers.Timer();
            _autoTimer.Interval = 30000;
            _autoTimer.Start();
            _autoTimer.Elapsed += (o, s) =>
                {                
                    if (_salesDetailsList.Count() >= 3 && _salesDetailsList[2].Amount !=null)
                    {
                        if (_autoResetEvent == null)
                        {
                            _autoResetEvent = new AutoResetEvent(false);
                        }
                        else
                            _autoResetEvent.WaitOne();

                        SaveInterim();                    
                    }
            };
        }

        #endregion
    }

    #region PaymentMode Class
    public class PaymentMode
    {
        public char PaymentId { get; set; }
        public string PaymentName { get; set; }
    }
    #endregion

    #region ProductPrice Class

    public class ProductPrice
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Quantity { get; set; }
        public int PriceId { get; set; }
    }
    #endregion

}
