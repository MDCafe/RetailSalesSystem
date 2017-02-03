using System;
using System.Windows.Input;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;

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
        Category category = null;


        ObservableCollection<SaleDetailExtn> _salesDetailsList;
        IEnumerable<ProductPrice> _productsPriceList;

        public SalesEntryViewModel()
        {
            IsDirty = true;
            Title = "Sales Entry";
            _rmsEntities = new RMSEntities();
            var cnt = _rmsEntities.Customers.ToList();
            var cnt1 = _rmsEntities.Products.ToList();
            _saleDate = DateTime.Now;


            if (_showAllCustomers)
                category = _rmsEntities.Categories.FirstOrDefault(c => c.name == Constants.CUSTOMERS_OTHERS);
            else
                category = _rmsEntities.Categories.FirstOrDefault(c => c.name == Constants.CUSTOMERS_HOTEL);

            if (category != null) _categoryId = category.Id;


            string sqlRunningNo = "select max(rollingno) + 1 from category cat where  cat.id = @p0";

            _runningBillNo = _rmsEntities.Database.SqlQuery<int>(sqlRunningNo, _categoryId).FirstOrDefault();

            _paymentModes = new List<PaymentMode>(2)
            {
                new PaymentMode {PaymentId = 0,PaymentName="Cash" },
                new PaymentMode {PaymentId = 1,PaymentName="Credit" }
            };

            _billSales = _rmsEntities.Sales.Create();

            _salesDetailsList = new ObservableCollection<SaleDetailExtn>();

            GetProductPriceList();

        }
       
        public IEnumerable<Customer> CustomersList
        {
            get
            {
                if (_showAllCustomers)
                    return _rmsEntities.Customers.Local;
                return _rmsEntities.Customers.Local.Where(c => c.CustomerTypeId == _categoryId);
            }
        }
         
        public ObservableCollection<SaleDetailExtn> SaleDetailList
        {
            get { return _salesDetailsList; }
        }        

        public IEnumerable<ProductPrice> ProductsPriceList
        {
            get { return _productsPriceList; }
            private set
            {
                _productsPriceList = value;
                NotifyPropertyChanged(() => this.ProductsPriceList);
            }
        }

        public IEnumerable<PaymentMode> PaymentModes
        {
            get { return _paymentModes; }
            private set
            {
                _paymentModes = value;
                NotifyPropertyChanged(() => this._paymentModes);
            }
        }

        public DateTime SaleDate { get { return _saleDate; } set { _saleDate = value; NotifyPropertyChanged(() => this._selectedCustomer); } }

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {                
                _selectedCustomer = value;
                NotifyPropertyChanged(() => this._selectedCustomer);
            }
        }

        public PaymentMode SelectedPaymentMode
        {
            get { return _selectedPaymentMode; }
            set
            {
                _selectedPaymentMode = value;
                NotifyPropertyChanged(() => this._selectedPaymentMode);
            }
        }
        
        public string OrderNo { get; set; }

        public int RunningBillNo
        {
            get { return _runningBillNo; }
            set
            {
                _runningBillNo = value;
                NotifyPropertyChanged(() => this._runningBillNo);
            }
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

        public bool CanSave(object parameter)
        {
          return IsDirty;
        }

        private void OnSave(object parameter)
        {            
            _billSales.CustomerId = SelectedCustomer.Id;
            _billSales.CustomerOrderNo = OrderNo;            
            _billSales.RunningBillNo = _runningBillNo;
            
            decimal? totalAmount = 0;
                           
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
                totalAmount += totalAmount + saleDetailItem.Amount;
            }

            _billSales.TotalAmount = totalAmount;
            _rmsEntities.Sales.Add(_billSales);
            category.RollingNo = _runningBillNo;

            _rmsEntities.SaveChanges();
            Clear();

        }

        #endregion

        #region GetBill Command
        RelayCommand<object> _getBillCommand = null;
        public ICommand GetBillCommand
        {
            get
            {
                if (_getBillCommand == null)
                {
                    _getBillCommand = new RelayCommand<object>((p) => OnGetBill(p));
                }

                return _getBillCommand;
            }
        }

        private void OnGetBill(object p)
        {
            throw new NotImplementedException();
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

        private void Clear()
        {
            GetProductPriceList();
            _selectedCustomer.Id = -1;
            _selectedPaymentMode.PaymentId = 0;
            OrderNo = "";
            _billSales.SaleDetails.Clear();
            _billSales = null;
        }

        public override Uri IconSource
        {
          get
          {
            // This icon is visible in AvalonDock's Document Navigator window
            return new Uri("pack://application:,,,/Edi;component/Images/document.png", UriKind.RelativeOrAbsolute);
          }
        }       
    }

    public struct PaymentMode
    {
        public int PaymentId { get; set; }
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
