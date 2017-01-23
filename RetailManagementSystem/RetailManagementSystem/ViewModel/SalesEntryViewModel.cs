using System;
using System.Windows.Input;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RetailManagementSystem.Model;

namespace RetailManagementSystem.ViewModel
{
    class SalesEntryViewModel : DocumentViewModel
    {
        RMSEntities _rmsEntities;
        List<PaymentMode> _paymentModes;       
        DateTime _saleDate;
        Customer _selectedCustomer;
        PaymentMode _selectedPaymentMode;
        Sale _billSales;
        SaleDetail _saleDetails;

        ObservableCollection<SaleDetailExtn> _salesDetailsList;
        IEnumerable<ProductPrice> _productsPriceList;

        public SalesEntryViewModel()
        {
            IsDirty = true;
            Title = "Sales Entry";
            _rmsEntities =  new RMSEntities();
            var cnt = _rmsEntities.Customers.ToList();
            var cnt1 = _rmsEntities.Products.ToList();
            _saleDate = DateTime.Now;

            _paymentModes = new List<PaymentMode>(2)
            {
                new PaymentMode {PaymentId = 0,PaymentName="Cash" },
                new PaymentMode {PaymentId = 1,PaymentName="Credit" }
            };

            _billSales = new Sale();
            _saleDetails = new SaleDetail();
            _salesDetailsList = new ObservableCollection<SaleDetailExtn>();
            _salesDetailsList.CollectionChanged += SalesDetailsListCollectionChanged;            

            string productsSQL = "select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price', " +
                                  " pd.SellingPrice as 'SellingPrice',st.Quantity as 'Quantity', pd.PriceId as 'PriceId'" +
                                  " from Products p, PriceDetails pd, Stocks st " +
                                  "where p.Id = pd.ProductId and pd.PriceId = st.PriceId " +
                                  " and st.Quantity != 0 order by p.Name";

            _productsPriceList =  _rmsEntities.Database.SqlQuery<ProductPrice>(productsSQL).ToList();

            

        }

        private void SalesDetailsListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
               e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset) return;
            var test = e.NewItems[0] as SaleDetail;            
        }

        public IEnumerable<Customer> CustomersList
        {
            get { return _rmsEntities.Customers.Local; }
        }
         
        public ObservableCollection<SaleDetailExtn> SaleDetailList
        {
            get { return _salesDetailsList; }
        }
        

        public IEnumerable<ProductPrice> ProductsPriceList
        {
            get { return _productsPriceList; }
        }

        public IEnumerable<PaymentMode> PaymentModes
        {
            get { return _paymentModes; }
        }

        public DateTime SaleDate { get { return _saleDate; } set { _saleDate = value; } }

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {                
                _selectedCustomer = value;                
            }
        }

        public PaymentMode SelectedPaymentMode
        {
            get { return _selectedPaymentMode; }
            set { _selectedPaymentMode = value; }
        }
        
        public string OrderNo { get; set; }

        public SaleDetail SaleDetail
        {
            get {return _saleDetails; }
            set { _saleDetails = value ; }
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
          return IsDirty;
        }

        private void OnSave(object parameter)
        {
            _billSales.CustomerId = SelectedCustomer.Id;
            _billSales.CustomerOrderNo = OrderNo;
            
        }

        #endregion

   
        private bool CanSaveAs(object parameter)
        {
          return IsDirty;
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
