using System;
using System.Windows.Input;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Extensions;

namespace RetailManagementSystem.ViewModel
{
    class SalesEntryViewModel : DocumentViewModel
    {
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
        string _totalAmountText = "0.0";
        char _selectedPaymentId;
        Category _category = null;
        string _selectedCustomerText;
        IExtensions _extensions;


        ObservableCollection<SaleDetailExtn> _salesDetailsList;
        IEnumerable<ProductPrice> _productsPriceList;

        public SalesEntryViewModel(bool showAllCustomers)
        {
            IsDirty = true;
            Title = "Sales Entry";
            _rmsEntities = new RMSEntities();
            var cnt = _rmsEntities.Customers.ToList();
            var cnt1 = _rmsEntities.Products.ToList();
            _saleDate = DateTime.Now;

            var othersCategory = _rmsEntities.Categories.FirstOrDefault(c => c.name == Constants.CUSTOMERS_OTHERS);
            _othersCategoryId = othersCategory.Id;
            _showAllCustomers = showAllCustomers;

            if (_showAllCustomers)
                _categoryId = _othersCategoryId;
            else
            {
                _category = _rmsEntities.Categories.FirstOrDefault(c => c.name == Constants.CUSTOMERS_HOTEL);
                _categoryId = _category.Id;
            }

            SetRunningBillNo();

            _paymentModes = new List<PaymentMode>(2)
            {
                new PaymentMode {PaymentId = '0',PaymentName="Cash" },
                new PaymentMode {PaymentId = '1',PaymentName="Credit" }
            };

            _billSales = _rmsEntities.Sales.Create();

            _salesDetailsList = new ObservableCollection<SaleDetailExtn>();

            GetProductPriceList();
            SelectedPaymentId = '0';

        }

        private void SetRunningBillNo()
        {
            string sqlRunningNo = "select max(rollingno) + 1 from category cat where  cat.id = @p0";

            RunningBillNo = _rmsEntities.Database.SqlQuery<int>(sqlRunningNo, _categoryId).FirstOrDefault();
        }

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
                //NotifyPropertyChanged(() => this._selectedCustomer);
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
            TotalAmountText = _totalAmount.ToString();
            RaisePropertyChanged("TotalAmount");
        }

        public string TotalAmountText
        {
            get { return _totalAmountText; }
            set
            {
                _totalAmountText = value;
                RaisePropertyChanged("TotalAmountText");
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
                _extensions.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "TransportCharges")
                    {
                        TotalAmountText = _extensions.Calculate(_totalAmount.Value).ToString();
                    }
                };
            }
            get { return _extensions; }
        }


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

        #region SaveCommand
         RelayCommand _saveCommand = null;
         override public ICommand SaveCommand
        {
          get
          {
            if (_saveCommand == null)
            {
              _saveCommand = new RelayCommand((p) => OnSave(p), (p) => CanSave(p));
            }

            return _saveCommand;
          }
        }

        #region CloseCommand
        RelayCommand _closeCommand = null;
        override public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand((p) => OnClose(), (p) => CanClose());
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

            _rmsEntities.SaveChanges();
            Clear();

        }

        #endregion

        #region GetBill Command
        RelayCommand _getBillCommand = null;        

        public ICommand GetBillCommand
        {
            get
            {
                if (_getBillCommand == null)
                {
                    _getBillCommand = new RelayCommand((p) => OnGetBill(_runningBillNo));
                }

                return _getBillCommand;
            }
        }

        private void OnGetBill(object billNo)
        {
            Clear();
            if (billNo == null) throw new ArgumentNullException("Please enter a bill no");
            var runningBillNo = Convert.ToInt32(billNo.ToString());

            _billSales = _rmsEntities.Sales.Where(b => b.RunningBillNo == runningBillNo).FirstOrDefault();
            SelectedCustomer = _billSales.Customer;
            SaleDate = _billSales.AddedOn.Value;
            //SelectedPaymentMode.PaymentId = Char.Parse(_billSales.PaymentMode);
            SelectedPaymentId = Char.Parse(_billSales.PaymentMode);
            OrderNo = _billSales.CustomerOrderNo;
            var saleDetailsForBill = _rmsEntities.SaleDetails.Where(b => b.BillId == _billSales.BillId);
            foreach (var saleDetailItem in saleDetailsForBill)
            {
                var productPrice = _productsPriceList.Where(p => p.PriceId == saleDetailItem.PriceId).FirstOrDefault();
                SaleDetailList.Add(
                    new SaleDetailExtn()
                    {
                        Discount = saleDetailItem.Discount,
                        PriceId = saleDetailItem.PriceId,
                        ProductId = saleDetailItem.ProductId,
                        Qty = saleDetailItem.Qty,
                        SellingPrice = saleDetailItem.SellingPrice,
                        BillId = saleDetailItem.BillId,
                        CostPrice = productPrice.Price,
                        AvailableStock = productPrice.Quantity,
                        Amount = productPrice.SellingPrice * saleDetailItem.Qty
                    });
            }
            _extensions.SetValues(_billSales.TransportCharges.Value);
            TotalAmount =  _extensions.Calculate(_billSales.TransportCharges.Value);
        }
        #endregion


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

        #region Clear Command
        RelayCommand _clearCommand = null;

        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand((p) => Clear());
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
            TotalAmountText = "";
            _extensions.Clear();
            SetRunningBillNo();            
        }

        #endregion

        public override Uri IconSource
        {
          get
          {
            // This icon is visible in AvalonDock's Document Navigator window
            return new Uri("pack://application:,,,/Edi;component/Images/document.png", UriKind.RelativeOrAbsolute);
          }
        }       
    }

    public class PaymentMode
    {
        public char PaymentId { get; set; }
        public string PaymentName { get; set; }
    }

    public class ProductPrice
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Quantity { get; set; }
        public int PriceId { get; set; }
    }
}
