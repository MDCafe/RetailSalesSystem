using System;
using System.Linq;
using System.Windows.Input;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Extensions;

namespace RetailManagementSystem.ViewModel.Sales
{
    class SalesEntryViewModel : DocumentViewModel
    {
        #region Private Variables
        RMSEntities _rmsEntities;
        IEnumerable<PaymentMode> _paymentModes;       
        DateTime _saleDate;
        Customer _selectedCustomer;
        PaymentMode _selectedPaymentMode;
        Sale _billSales;
        int _runningBillNo;
        bool _showAllCustomers;
        int _categoryId;
        int _othersCategoryId;
        decimal? _totalAmount = 0;        
        decimal? _totalDiscountPercent;
        decimal? _totalDiscountAmount;
        decimal _totalAmountDisplay = 0.0M;
        decimal _amountPaid = 0.0M;
        char _selectedPaymentId;
        Category _category = null;
        string _selectedCustomerText;
        IExtensions _extensions;
        bool _isEditMode;

        ObservableCollection<SaleDetailExtn> _salesDetailsList;
        IEnumerable<ProductPrice> _productsPriceList;
        List<SaleDetailExtn> _deletedItems;
        #endregion
 
        #region Constructor
        public SalesEntryViewModel(SalesParams salesParams)
        {            
            //IsDirty = true;            
            _rmsEntities = new RMSEntities();
            var cnt = _rmsEntities.Customers.ToList();
            var cnt1 = _rmsEntities.Products.ToList();
            _saleDate = DateTime.Now;

            var othersCategory = _rmsEntities.Categories.FirstOrDefault(c => c.name == Constants.CUSTOMERS_OTHERS);
            _othersCategoryId = othersCategory.Id;

            _showAllCustomers = salesParams.ShowAllCustomers;

            if (_showAllCustomers)
                _categoryId = _othersCategoryId;
            else
            {
                _category = _rmsEntities.Categories.FirstOrDefault(c => c.name == Constants.CUSTOMERS_HOTEL);
                _categoryId = _category.Id;
            }
           
            _paymentModes = new List<PaymentMode>(2)
            {
                new PaymentMode {PaymentId = '0',PaymentName="Cash" },
                new PaymentMode {PaymentId = '1',PaymentName="Credit" }
            };

            _billSales = _rmsEntities.Sales.Create();

            _salesDetailsList = new ObservableCollection<SaleDetailExtn>();

            _salesDetailsList.CollectionChanged += _salesDetailsList_CollectionChanged;

            GetProductPriceList();
            SelectedPaymentId = '0';

            if (salesParams !=null &&  salesParams.Billno.HasValue)
            {                
                OnEditBill(salesParams.Billno.Value);                
                Title = "Sale Bill Amend :" + _runningBillNo;
            }
            else
            {
                Title = "Sales Entry";
                SetRunningBillNo();
            }

        }
       
        #endregion

        #region Getters and Setters

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                if (_showAllCustomers)
                    return _rmsEntities.Customers.Local;
                return _rmsEntities.Customers.Local.Where(c => c.CustomerTypeId != _othersCategoryId);
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
                    _extensions.SetValues(_billSales.TransportCharges.Value);
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

        private void SetRunningBillNo()
        {
            string sqlRunningNo = "select max(rollingno) + 1 from category cat where  cat.id = @p0";

            RunningBillNo = _rmsEntities.Database.SqlQuery<int>(sqlRunningNo, _categoryId).FirstOrDefault();
        }

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
            Workspace.This.Close(this);
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
                var saleDetail = _rmsEntities.SaleDetails.Create();
                saleDetail.Discount = saleDetailItem.Discount;
                saleDetail.PriceId = saleDetailItem.PriceId;
                saleDetail.ProductId = saleDetailItem.ProductId;
                saleDetail.Qty = saleDetailItem.Qty;
                saleDetail.SellingPrice = saleDetailItem.SellingPrice;
                saleDetail.BillId = _billSales.BillId;                
                _billSales.SaleDetails.Add(saleDetail);

                var stockToReduceColln = _rmsEntities.Stocks.Where(s => s.ProductId == saleDetailItem.ProductId && s.PriceId == saleDetailItem.PriceId);
                var stock = stockToReduceColln.FirstOrDefault();
                             
                if(stock != null)
                {                    
                        stock.Quantity -= saleDetailItem.Qty.Value;
                }
                //totalAmount += totalAmount + saleDetailItem.Amount;
            }

            _totalAmount = _extensions.Calculate(_totalAmount.Value);

            _billSales.TotalAmount = _totalAmount;
            _billSales.TransportCharges = _extensions.GetPropertyValue("TransportCharges");

           
            _rmsEntities.Sales.Add(_billSales);
            _category.RollingNo = _runningBillNo;
            //}
            //else
            //{
            //    var saleToEdit = _rmsEntities.Sales.Where(s => s.RunningBillNo == _billSales.RunningBillNo).FirstOrDefault();
            //    saleToEdit = _billSales;
            //}


            //check if complete amount is paid, else mark it in advancedetails table against the customer
            var outstandingBalance = _totalAmount.Value - AmountPaid;
            if (outstandingBalance > 0)
            {
                var msg = "Outstanding balance Rs " + outstandingBalance + ". Do you want to keep as pending balance amount?";
                var result = Utility.ShowMessageBoxWithOptions(msg);
                if(result == System.Windows.MessageBoxResult.Yes)
                {
                    _rmsEntities.PaymentDetails.Add
                        (
                            new PaymentDetail
                            {
                                BillId = _billSales.BillId,
                                AmountPaid = AmountPaid,
                                CustomerId = _selectedCustomer.Id                                
                            }
                        );
                }
                var customer = _rmsEntities.Customers.FirstOrDefault(c => c.Id == _selectedCustomer.Id);
                customer.BalanceDue += outstandingBalance;
            }
            
            _rmsEntities.SaveChanges();
            Clear();
        }

        private void SaveOnEdit()
        {
            //Check if there are any deletions

            RemoveDeletedItems();

            foreach (var saleDetailItemExtn in _salesDetailsList)
            {
                var saleDetail = _rmsEntities.SaleDetails.FirstOrDefault(b => b.BillId == saleDetailItemExtn.BillId
                                                                        && b.ProductId == saleDetailItemExtn.ProductId);

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
                    }
                    continue;
                }

                SetSaleDetailItem(saleDetailItemExtn, saleDetail);
                var stock = _rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetail.ProductId && s.PriceId == saleDetail.PriceId);

                if (stock != null)
                {
                    stock.Quantity = (stock.Quantity + saleDetailItemExtn.OriginalQty.Value) - saleDetail.Qty.Value;
                }
            }

            _totalAmount = _extensions.Calculate(_totalAmount.Value);

            _billSales.TotalAmount = _totalAmount;
            _billSales.TransportCharges = _extensions.GetPropertyValue("TransportCharges");

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
                    stockNewItem.Quantity += saleDetail.Qty.Value;
                }
                _rmsEntities.SaleDetails.Remove(saleDetail);
            }
        }

        private void _salesDetailsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && _isEditMode)
            {
                var SaleDetailExtn = e.OldItems[0] as SaleDetailExtn;
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

            _billSales = _rmsEntities.Sales.Where(b => b.RunningBillNo == runningBillNo).FirstOrDefault();            
            SelectedCustomer = _billSales.Customer;
            SelectedCustomerText = SelectedCustomer.Name;
            SaleDate = _billSales.AddedOn.Value;            
            SelectedPaymentId = Char.Parse(_billSales.PaymentMode);
            OrderNo = _billSales.CustomerOrderNo;
            var saleDetailsForBill = _rmsEntities.SaleDetails.Where(b => b.BillId == _billSales.BillId);            

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
                SetSaleDetailExtn(productPrice, saleDetailExtn);

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
            GetProductPriceList();
            SelectedCustomer = null;
            SelectedPaymentId = '0';
            OrderNo = "";
            SaleDetailList.Clear();
            _billSales.CustomerId = 0;
            _totalAmount = 0;
            TotalAmountDisplay =0.0M;
            AmountPaid = 0.0M;            
            _extensions.Clear();
            SetRunningBillNo();
            _isEditMode = false;
        }

        #endregion

        #region Public Methods
        public void SetProductDetails(ProductPrice productPrice)
        {
            if (productPrice == null) return;
            var selRowSaleDetailExtn = SaleDetailList.FirstOrDefault(s => s.ProductId == productPrice.ProductId);
            SetSaleDetailExtn(productPrice, selRowSaleDetailExtn);
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
        private void SetSaleDetailExtn(ProductPrice productPrice, SaleDetailExtn SaleDetailExtn)
        {
            if (SaleDetailExtn != null)
            {
                //selectedRowSaleDetail.Qty = productPrice.Quantity;
                SaleDetailExtn.SellingPrice = productPrice.SellingPrice;
                SaleDetailExtn.CostPrice = productPrice.Price;
                SaleDetailExtn.PriceId = productPrice.PriceId;
                SaleDetailExtn.AvailableStock = productPrice.Quantity;

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

        private void GetProductPriceList()
        {
            string productsSQL = "select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price', " +
                                  " pd.SellingPrice as 'SellingPrice',st.Quantity as 'Quantity', pd.PriceId as 'PriceId'" +
                                  " from Products p, PriceDetails pd, Stocks st " +
                                  "where p.Id = pd.ProductId and pd.PriceId = st.PriceId " +
                                  " and st.Quantity != 0 " +
                                  " union " +
                                    "select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price'," +
                                    "pd.SellingPrice as 'SellingPrice',st.Quantity as 'Quantity', pd.PriceId as 'PriceId'" +
                                    " from Products p, PriceDetails pd, Stocks st " +
                                    " where p.Id = pd.ProductId and pd.PriceId = st.PriceId " +
                                    " and st.Quantity = 0 " +
                                    " and St.ModifiedOn = " +
                                    " (select max(ModifiedOn) from Stocks s " +
                                     "   where s.ProductId = st.ProductId) " +
                                    " order by ProductName ";

            _productsPriceList = _rmsEntities.Database.SqlQuery<ProductPrice>(productsSQL).ToList();
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
