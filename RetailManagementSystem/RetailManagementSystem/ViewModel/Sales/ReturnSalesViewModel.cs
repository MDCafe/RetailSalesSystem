using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Sales
{
    class ReturnSalesViewModel : SalesViewModelbase
    {
        public delegate void MakeReadOnly(bool isReadOnly);
        public event MakeReadOnly MakeReadonlyEvent;

        RMSEntities _rmsEntities;
        ObservableCollection<ReturnSaleDetailExtn> _returnSalesDetailsList;
        IEnumerable<PriceDetail> _returnPriceList;        
        Sale _billSales;
        DateTime? _saleDate;        
        bool _isBillBasedReturn;
        new decimal? _totalAmount;
        string _customerName;
        string _modeOfPayment;

        public int? BillNo { get; set; }        

        public decimal? TotalAmount
        {
            get { return _totalAmount; }
            private set
            {
                _totalAmount = value;
                RaisePropertyChanged("TotalAmount");
            }
        }
        public string  CustomerName
        {
            get { return _customerName; }
            set
            {
                _customerName = value;
                RaisePropertyChanged("CustomerName");
            }
        }
        public string ModeOfPayment
        {
            get { return _modeOfPayment; }
            set
            {
                _modeOfPayment = value;
                //NotifyPropertyChanged(() => this._selectedCustomer);
                RaisePropertyChanged("ModeOfPayment");
            }
        }
        public DateTime? SaleDate
        {
            get { return _saleDate; }
            set
            {
                _saleDate = value;
                RaisePropertyChanged("SaleDate");
            }
        }        
        public bool IsBillBasedReturn
        {
            get { return _isBillBasedReturn; }
            set
            {
                _isBillBasedReturn = value;                
                MakeReadonlyEvent?.Invoke(_isBillBasedReturn);
            }
        }

        public ReturnSalesViewModel(bool showRestrictedCustomers) : base(showRestrictedCustomers)
        {            
            _returnSalesDetailsList = new ObservableCollection<ReturnSaleDetailExtn>();
            _returnSalesDetailsList.CollectionChanged += (s, e) =>
             {
                 if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                 {
                     TotalAmount = _returnSalesDetailsList.Sum(r => r.Amount);
                 }
             };

            _rmsEntities = RMSEntitiesHelper.Instance.GetNewInstanceOfRMSEntities();
            //_returnPriceList = _rmsEntities.PriceDetails.ToList();
            var cnt = _rmsEntities.Products.ToList();
            //var cnt1= _returnPriceList.Count();            
            this.Title = "Return Sales Entry";
        }

        public ObservableCollection<ReturnSaleDetailExtn> ReturnSaleDetailList
        {
            get { return _returnSalesDetailsList; }         
            private set
            {
                _returnSalesDetailsList = value;
                RaisePropertyChanged("ReturnSaleDetailList");
            }
        }

        public IEnumerable<PriceDetail> ReturnPriceList
        {
            get
            {
                return _returnPriceList;
            }
            private set
            {
                _returnPriceList = value;
                RaisePropertyChanged("ReturnPriceList");
            }
        }

        public IEnumerable<Product> ProductsList
        {
            get { return _rmsEntities.Products.Local; }
        }
      
        public void SetProductPriceDetails(int productId, int selectedIndex)
        {
            var product = _rmsEntities.Products.Where(p => p.Id == productId).FirstOrDefault();
            _returnSalesDetailsList[selectedIndex].ProductName = product.Name;
            //var selectedProduct = _returnSalesDetailsList.Where(s => s.ProductId == product.Id).FirstOrDefault();
            //selectedProduct.ProductName = product.Name;
        }

        public void SetPriceDetails(int priceId,int selectedIndex)
        {
            var returnPrice = _rmsEntities.PriceDetails.Where(pr => pr.PriceId == priceId).FirstOrDefault();
            var returnItem = _returnSalesDetailsList[selectedIndex];
            returnItem.ReturnPrice = returnPrice.Price;
            returnItem.Amount = returnItem.ReturnPrice * returnItem.ReturnQty;
            TotalAmount = _returnSalesDetailsList.Sum(p => p.Amount);
            returnItem.PropertyChanged += (s, e) =>
            {
                if(e.PropertyName == Constants.RETURN_QTY)
                {
                    returnItem.Amount = returnItem.ReturnPrice * returnItem.ReturnQty;
                    TotalAmount = _returnSalesDetailsList.Sum(p => p.Amount);
                }
            };
        }

        private void SetReturnSalePrice(Product productPrice)
        {            
            //if (SaleDetailExtn != null)
            //{            
            //    SaleDetailExtn.SellingPrice = productPrice.SellingPrice;
            //    SaleDetailExtn.CostPrice = productPrice.Price;
            //    SaleDetailExtn.PriceId = productPrice.PriceId;
            //    SaleDetailExtn.AvailableStock = productPrice.Quantity;
            //    SaleDetailExtn.SerialNo = selectedIndex;

            //    SaleDetailExtn.PropertyChanged += (sender, e) =>
            //    {
            //        var prop = e.PropertyName;
            //        if (prop == Constants.AMOUNT)
            //        {
            //            TotalAmount = SaleDetailList.Sum(a => a.Amount);
            //            return;
            //        }
            //        var amount = SaleDetailExtn.SellingPrice * SaleDetailExtn.Qty;
            //        var discountAmount = SaleDetailExtn.DiscountPercentage != 0 ?
            //                             amount - (amount * (SaleDetailExtn.DiscountPercentage / 100)) :
            //                             SaleDetailExtn.DiscountAmount != 0 ?
            //                             amount - SaleDetailExtn.DiscountAmount :
            //                             0;

            //        if (discountAmount != 0)
            //        {
            //            SaleDetailExtn.Amount = discountAmount;
            //            SaleDetailExtn.Discount = discountAmount;
            //            return;
            //        }

            //        SaleDetailExtn.Amount = amount;
            //        SaleDetailExtn.Discount = 0;
            //    };
            //}
        }
         

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
            return _returnSalesDetailsList.Count != 0 && _returnSalesDetailsList[0].ProductId != 0;
        }

        private void OnSave(object parameter)
        {
            decimal amount = 0.0M;
            if (IsBillBasedReturn)
            {
                foreach (var item in _returnSalesDetailsList)
                {
                    if (item.Selected)
                    {
                        amount += item.ReturnAmount;
                        SaveReturnItems(item,_billSales.BillId);
                    }
                }
                var customer = _rmsEntities.Customers.FirstOrDefault(c => c.Id == _billSales.CustomerId);
                if (customer != null)
                    customer.BalanceDue -= amount;

                _rmsEntities.SaveChanges();
                Clear();
                return;
            }
            
            foreach (var item in _returnSalesDetailsList)
            {
                SaveReturnItems(item,null);
            }
            _rmsEntities.SaveChanges();
            Clear();
        }

        private void SaveReturnItems(ReturnSaleDetailExtn item, int? billNo)
        {
            
            _rmsEntities.ReturnDamagedStocks.Add(new ReturnDamagedStock()
            {
                isReturn = true,
                PriceId = item.PriceId,
                ProductId = item.ProductId,
                Quantity = item.ReturnQty,
                BillId = billNo
            });

            var stock = _rmsEntities.Stocks.Where(s => s.PriceId == item.PriceId && s.ProductId == item.ProductId).FirstOrDefault();
            stock.Quantity += item.ReturnQty;
        }

        #endregion

        #region GetBillCommand
        RelayCommand<object> _getBillCommand = null;
        public ICommand GetBillCommand
        {
            get
            {
                if (_getBillCommand == null)
                {
                    _getBillCommand = new RelayCommand<object>((p) => OnGetBill(p), (p) => CanGetBillNo(p));
                }

                return _getBillCommand;
            }
        }
        
        public bool CanGetBillNo(object parameter)
        {            
            return BillNo!=null && BillNo.HasValue && BillNo.Value != 0;
        }

        private void OnGetBill(object parameter)
        {
            PanelLoading = true;
            var GetBillTask = System.Threading.Tasks.Task.Run(() =>
            {
               _returnSalesDetailsList = new ObservableCollection<ReturnSaleDetailExtn>();
               var customerBill = RMSEntitiesHelper.CheckIfBillExists(BillNo.Value, _categoryId,null);
               if (customerBill == null)
               {
                   PanelLoading = false;
                   return;

               }

               _billSales = _rmsEntities.Sales.Where(s => s.RunningBillNo == BillNo && customerBill.CustomerId == s.CustomerId).FirstOrDefault();
               
               CustomerName = _billSales.Customer.Name;
               SaleDate = _billSales.AddedOn.Value;              
               ModeOfPayment = PaymentMode.GetPaymentString(_billSales.PaymentMode);
               var saleDetailsForBill = _rmsEntities.SaleDetails.Where(b => b.BillId == _billSales.BillId);

               foreach (var saleDetailItem in saleDetailsForBill.ToList())
               {
                   //Get the item and check if there is any return done for the item.
                   var returnQty = _rmsEntities.ReturnDamagedStocks.FirstOrDefault(b => b.BillId == _billSales.BillId && saleDetailItem.ProductId == b.ProductId);
                   var remainingQty = 0.0M;
                   if (returnQty != null)
                   {
                       remainingQty = saleDetailItem.Qty.Value - returnQty.Quantity;
                       if (remainingQty == 0) continue;
                   }
                   else
                       remainingQty = saleDetailItem.Qty.Value;

                   var productPrice = _rmsEntities.PriceDetails.Where(p => p.PriceId == saleDetailItem.PriceId).FirstOrDefault();
                   var saleDetailExtn = new ReturnSaleDetailExtn()
                   {
                       Discount = saleDetailItem.Discount,
                       PriceId = saleDetailItem.PriceId,
                       ProductId = saleDetailItem.ProductId,
                       Qty = remainingQty,
                       OriginalQty = saleDetailItem.Qty,
                       SellingPrice = saleDetailItem.SellingPrice,
                       BillId = saleDetailItem.BillId,
                       CostPrice = productPrice.Price,
                       ProductName = _rmsEntities.Products.FirstOrDefault(p => p.Id == saleDetailItem.ProductId).Name,
                       //AvailableStock = productPrice.Quantity,
                       Amount = productPrice.SellingPrice * saleDetailItem.Qty
                   };

                   ReturnSaleDetailList.Add(saleDetailExtn);

                   saleDetailExtn.PropertyChanged += (s, e) =>
                   {
                       if (e.PropertyName == Constants.RETURN_QTY)
                       {

                           saleDetailExtn.ReturnAmount = saleDetailExtn.SellingPrice.Value * saleDetailExtn.ReturnQty;
                           TotalAmount = _returnSalesDetailsList.Sum(p => p.ReturnAmount);
                       }
                   };

                   //tempTotalAmount += productPrice.SellingPrice * remainingQty;
               }
               //TotalAmount = tempTotalAmount;               
                System.Threading.Thread.Sleep(1000);
           }).ContinueWith(
            (t) =>
            {
                PanelLoading = false;
                RaisePropertyChanged("ReturnSaleDetailList");
                IsBillBasedReturn = true;
            });            
        }

        #endregion

        #region Clear Command

        override internal void Clear()
        {
            _returnSalesDetailsList = new ObservableCollection<ReturnSaleDetailExtn>();
            RaisePropertyChanged("ReturnSaleDetailList");
            BillNo = null;
            IsBillBasedReturn = false;
            TotalAmount = null;
            CustomerName = "";
            ModeOfPayment = "";
            SaleDate = null;
        }

        #endregion
    }
}
