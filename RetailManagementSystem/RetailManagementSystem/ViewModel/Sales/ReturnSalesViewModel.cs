using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using System;

namespace RetailManagementSystem.ViewModel.Sales
{
    class ReturnSalesViewModel : DocumentViewModel
    {
        public delegate void MakeReadOnly(bool isReadOnly);
        public event MakeReadOnly MakeReadonlyEvent;

        RMSEntities _rmsEntities;
        ObservableCollection<ReturnSaleDetailExtn> _returnSalesDetailsList;
        IEnumerable<PriceDetail> _returnPriceList;
        Customer _selectedCustomer;
        string _selectedCustomerText;
        Sale _billSales;
        DateTime _saleDate;
        char _selectedPaymentId;
        bool _isBillBasedReturn;

        public int? BillNo { get; private set; }
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
        public DateTime SaleDate
        {
            get { return _saleDate; }
            set
            {
                _saleDate = value;
                RaisePropertyChanged("SaleDate");
            }
        }
        public char SelectedPaymentId
        {
            get { return _selectedPaymentId; }
            set
            {
                _selectedPaymentId = value;
                RaisePropertyChanged("SelectedPaymentId");
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
            _rmsEntities = RMSEntitiesHelper.Instance.GetNewInstanceOfRMSEntities();
            //_returnPriceList = _rmsEntities.PriceDetails.ToList();
            var cnt = _rmsEntities.Products.ToList();
            //var cnt1= _returnPriceList.Count();
            SaleDate = DateTime.Now;
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

        public IEnumerable<PriceDetail> GetProductPriceDetails(int productId)
        {
            return _rmsEntities.PriceDetails.Where(pr => pr.ProductId == productId).OrderByDescending(s => s.ModifiedOn).Take(3).ToList();
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

        private void OnClose()
        {
            Workspace.This.Close(this);
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
                }
            }
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
            return _returnSalesDetailsList.Count != 0 && _returnSalesDetailsList[0].ProductId != 0;
        }

        private void OnSave(object parameter)
        {
            foreach (var item in _returnSalesDetailsList)
            {
                if(item.Selected)
                {
                    _rmsEntities.ReturnDamagedStocks.Add(new ReturnDamagedStock()
                    {
                        isReturn = true,
                        PriceId = item.PriceId,
                        ProductId = item.ProductId,
                        Quantity = item.ReturnQty                        
                    });

                    var stock = _rmsEntities.Stocks.Where(s => s.PriceId == item.PriceId && s.ProductId == item.ProductId).FirstOrDefault();
                    stock.Quantity += item.Qty.HasValue ? item.Qty.Value : 0;
                }

            }
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
            return BillNo != -1;
        }

        private void OnGetBill(object parameter)
        {
            _returnSalesDetailsList = new ObservableCollection<ReturnSaleDetailExtn>();
            var customerBill = RMSEntitiesHelper.CheckIfBillExists(BillNo.Value, _categoryId);
            if (customerBill == null)
                return;

            _billSales = _rmsEntities.Sales.Where(s => s.RunningBillNo == BillNo && customerBill.CustomerId == s.CustomerId).FirstOrDefault();

            //SelectedCustomer = _billSales.Customer;
            //SelectedCustomerText = SelectedCustomer.Name;
            SaleDate = _billSales.AddedOn.Value;
            SelectedPaymentId = Char.Parse(_billSales.PaymentMode);            
            var saleDetailsForBill = _rmsEntities.SaleDetails.Where(b => b.BillId == _billSales.BillId);

            var tempTotalAmount = 0.0M;
            foreach (var saleDetailItem in saleDetailsForBill.ToList())
            {
                var productPrice = _rmsEntities.PriceDetails.Where(p => p.PriceId == saleDetailItem.PriceId).FirstOrDefault();
                var saleDetailExtn = new ReturnSaleDetailExtn()
                {
                    Discount = saleDetailItem.Discount,
                    PriceId = saleDetailItem.PriceId,
                    ProductId = saleDetailItem.ProductId,
                    Qty = saleDetailItem.Qty,
                    OriginalQty = saleDetailItem.Qty,
                    SellingPrice = saleDetailItem.SellingPrice,
                    BillId = saleDetailItem.BillId,
                    CostPrice = productPrice.Price,
                    //AvailableStock = productPrice.Quantity,
                    Amount = productPrice.SellingPrice * saleDetailItem.Qty
                };

                ReturnSaleDetailList.Add(saleDetailExtn);                

                tempTotalAmount += productPrice.SellingPrice * saleDetailItem.Qty.Value;
            }
            //TotalAmount = tempTotalAmount;

            //RunningBillNo = runningBillNo;
            //_isEditMode = true;
            //if (_deletedItems == null)
            //    _deletedItems = new List<SaleDetailExtn>();
            //else
            //    _deletedItems.Clear();

            IsBillBasedReturn = true;
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
            _returnSalesDetailsList = new ObservableCollection<ReturnSaleDetailExtn>();
            BillNo = null;
        }

        #endregion
    }
}
