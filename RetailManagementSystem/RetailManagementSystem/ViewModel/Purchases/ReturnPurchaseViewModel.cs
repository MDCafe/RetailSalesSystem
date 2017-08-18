using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.Utilities;
using System.ComponentModel;

namespace RetailManagementSystem.ViewModel.Purchases
{
    class ReturnPurchaseViewModel : PurchaseViewModelbase
    {
        public delegate void MakeReadOnly(bool isReadOnly);
        public event MakeReadOnly MakeReadonlyEvent;

        RMSEntities _rmsEntities;
        ObservableCollection<ReturnPurchaseDetailExtn> _returnPurchaseDetailsList;
        IEnumerable<PriceDetail> _returnPriceList;        
        Purchase _billPurchase;
        DateTime? _purchaseDate;        
        bool _isBillBasedReturn, _panelLoading;
        new decimal? _totalAmount;
        Company _selectedCompany;
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
        public IEnumerable<Company> Companies
        {
            get { return _rmsEntities.Companies.Local.Where(c => c.CategoryTypeId == _categoryId);  }
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
        public DateTime? PurchaseDate
        {
            get { return _purchaseDate; }
            set
            {
                _purchaseDate = value;
                RaisePropertyChanged("PurchaseDate");
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


        public Company SelectedCompany
        {
            get
            {
                return _selectedCompany;
            }

            set
            {
                _selectedCompany = value;
                RaisePropertyChanged("SelectedCompany");
            }
        }
        public IEnumerable<CodeMaster> ReturnReasons
        { get { return _rmsEntities.CodeMasters.Local.Where(r => r.Code == "RTN"); } }

        public bool PanelLoading
        {
            get
            {
                return _panelLoading;
            }
            set
            {
                _panelLoading = value;
                RaisePropertyChanged("PanelLoading");
            }
        }

        public ReturnPurchaseViewModel(bool showRestrictedCustomers) : base(showRestrictedCustomers)
        {            
            _returnPurchaseDetailsList = new ObservableCollection<ReturnPurchaseDetailExtn>();
            _returnPurchaseDetailsList.CollectionChanged += (s, e) =>
             {
                 if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                 {
                     TotalAmount = _returnPurchaseDetailsList.Sum(r => r.ReturnAmount);
                 }
             };

            _rmsEntities = RMSEntitiesHelper.Instance.GetNewInstanceOfRMSEntities();
            //_returnPriceList = _rmsEntities.PriceDetails.ToList();
            var cnt = _rmsEntities.Products.ToList();
            var cnt1 = _rmsEntities.CodeMasters.ToList();
            var cnt2 = _rmsEntities.Companies.ToList();            
            this.Title = "Return Sales Entry";
        }

        public ObservableCollection<ReturnPurchaseDetailExtn> ReturnPurchaseDetailList
        {
            get { return _returnPurchaseDetailsList; }         
            private set
            {
                _returnPurchaseDetailsList = value;
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
            get
            {
                if (SelectedCompany.Id == 0)
                {
                    Utilities.Utility.ShowErrorBox("Select a supplier to select products");
                    return null;
                }
                return _rmsEntities.Products.Local.Where(p => p.CompanyId == SelectedCompany.Id);
            }
        }
      
        public void SetProductPriceDetails(int productId, int selectedIndex)
        {
            var product = _rmsEntities.Products.Where(p => p.Id == productId).FirstOrDefault();
            _returnPurchaseDetailsList[selectedIndex].ProductName = product.Name;
            //var selectedProduct = _returnSalesDetailsList.Where(s => s.ProductId == product.Id).FirstOrDefault();
            //selectedProduct.ProductName = product.Name;
        }

        public void SetPriceDetails(int priceId,int selectedIndex)
        {
            var returnPrice = _rmsEntities.PriceDetails.Where(pr => pr.PriceId == priceId).FirstOrDefault();
            var returnItem = _returnPurchaseDetailsList[selectedIndex];
            returnItem.ReturnPrice = returnPrice.Price;
            returnItem.Amount = returnItem.ReturnPrice * returnItem.ReturnQty;
            TotalAmount = _returnPurchaseDetailsList.Sum(p => p.Amount);
            returnItem.PropertyChanged += (s, e) =>
            {
                if(e.PropertyName == Constants.RETURN_QTY)
                {
                    returnItem.Amount = returnItem.ReturnPrice * returnItem.ReturnQty;
                    TotalAmount = _returnPurchaseDetailsList.Sum(p => p.Amount);
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
            return _returnPurchaseDetailsList.Count != 0 && _returnPurchaseDetailsList[0].ProductId != 0;
        }

        private void OnSave(object parameter)
        {
            decimal amount = 0.0M;
            if (IsBillBasedReturn)
            {
                foreach (var item in _returnPurchaseDetailsList)
                {
                    if (item.Selected)
                    {
                        amount += item.ReturnAmount;
                        SaveReturnItems(item,_billPurchase.BillId);
                    }
                }
                var customer = _rmsEntities.Customers.FirstOrDefault(c => c.Id == _billPurchase.CompanyId);
                if (customer != null)
                    customer.BalanceDue -= amount;

                _rmsEntities.SaveChanges();
                Clear();
                return;
            }
            
            foreach (var item in _returnPurchaseDetailsList)
            {
                SaveReturnItems(item,null);
            }
            _rmsEntities.SaveChanges();
            Clear();
        }

        private void SaveReturnItems(ReturnPurchaseDetailExtn item, int? billNo)
        {
            
            _rmsEntities.ReturnDamagedStocks.Add(new ReturnDamagedStock()
            {
                isReturn = true,
                PriceId = item.PriceId,
                ProductId = item.ProductId,
                Quantity = item.ReturnQty,
                BillId = billNo,
                comments = item.SelectedReturnReason.ToString()
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
            var GetBillTask = System.Threading.Tasks.Task.Run((Action)(() =>
            {
                _returnPurchaseDetailsList = new ObservableCollection<ReturnPurchaseDetailExtn>();
               var companyBill = RMSEntitiesHelper.CheckIfPurchaseBillExists(BillNo.Value, _categoryId);
               if (companyBill == null)
               {
                    PanelLoading = false;
                   return;
               }

                _billPurchase = _rmsEntities.Purchases.Where(s => s.RunningBillNo == BillNo && companyBill.CompanyId == s.CompanyId).FirstOrDefault();

                //CompanyName = _billPurchase.Company.Name;
                PurchaseDate = _billPurchase.AddedOn.Value;
                PaymentMode pm = new PaymentMode();
                ModeOfPayment = pm.GetPaymentString(_billPurchase.PaymentMode);
               var purhcaseDetailsForBill = _rmsEntities.PurchaseDetails.Where(b => b.BillId == _billPurchase.BillId);

               foreach (var purchaseDetailItem in purhcaseDetailsForBill.ToList())
               {
                   //Get the item and check if there is any return done for the item.
                   var returnQty = _rmsEntities.ReturnDamagedStocks.FirstOrDefault(b => b.BillId == _billPurchase.BillId && purchaseDetailItem.ProductId == b.ProductId);
                   var remainingQty = 0.0M;
                   if (returnQty != null)
                   {
                       remainingQty = purchaseDetailItem.PurchasedQty.Value - returnQty.Quantity;
                       if (remainingQty == 0) continue;
                   }
                   else
                       remainingQty = purchaseDetailItem.PurchasedQty.Value;

                   var productPrice = _rmsEntities.PriceDetails.Where(p => p.PriceId == purchaseDetailItem.PriceId).FirstOrDefault();
                   var purchaseDetailExtn = new ReturnPurchaseDetailExtn()
                   {
                       Discount = purchaseDetailItem.Discount,
                       PriceId = purchaseDetailItem.PriceId.Value,
                       ProductId = purchaseDetailItem.ProductId.Value,
                       Qty = remainingQty,
                       OriginalQty = purchaseDetailItem.PurchasedQty,
                       //SellingPrice = purchaseDetailItem.SellingPrice,
                       BillId = purchaseDetailItem.BillId,
                       CostPrice = productPrice.Price,
                       ProductName = _rmsEntities.Products.FirstOrDefault(p => p.Id == purchaseDetailItem.ProductId).Name,
                       //AvailableStock = productPrice.Quantity,
                       Amount = productPrice.SellingPrice * purchaseDetailItem.PurchasedQty
                   };

                    ReturnPurchaseDetailList.Add(purchaseDetailExtn);

                   purchaseDetailExtn.PropertyChanged += (object s, PropertyChangedEventArgs e) =>
                   {
                       if (e.PropertyName == Constants.RETURN_QTY)
                       {
                           purchaseDetailExtn.ReturnAmount = purchaseDetailExtn.SellingPrice.Value * purchaseDetailExtn.ReturnQty;
                           this.TotalAmount = this._returnPurchaseDetailsList.Sum( p => p.ReturnAmount);
                       }
                   };

                   //tempTotalAmount += productPrice.SellingPrice * remainingQty;
               }
                //TotalAmount = tempTotalAmount;               
                System.Threading.Thread.Sleep(1000);
           })).ContinueWith(
            (t) =>
            {
                PanelLoading = false;
                base.RaisePropertyChanged("ReturnPurchaseDetailList");
                IsBillBasedReturn = true;
            });            
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
            _returnPurchaseDetailsList = new ObservableCollection<ReturnPurchaseDetailExtn>();
            RaisePropertyChanged("ReturnSaleDetailList");
            BillNo = null;
            IsBillBasedReturn = false;
            TotalAmount = null;
            //sCompanyName = "";
            ModeOfPayment = "";
            PurchaseDate = null;
        }

        #endregion
    }
}
