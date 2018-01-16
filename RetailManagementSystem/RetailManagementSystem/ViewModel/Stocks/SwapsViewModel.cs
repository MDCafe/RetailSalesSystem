using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Base;

namespace RetailManagementSystem.ViewModel.Stocks
{
    class SwapsViewModel : CommonBusinessViewModel
    {
        CodeMaster _selectedSwapMode;
        RMSEntities _rmsEntities;

        ObservableCollection<SaleDetailExtn> _swapsDetailsList;

        IEnumerable<CodeMaster> _swapsCodeList;

        public CodeMaster SelectedSwapMode
        {
            get
            {
                return _selectedSwapMode;
            }

            set
            {
                _selectedSwapMode = value;
                RaisePropertyChanged("SelectedSwapMode");
            }
        }

        public IEnumerable<CodeMaster> SwapsCodeList
        {
            get
            {
                if (_swapsCodeList == null)
                {
                    return _rmsEntities.CodeMasters.Local.Where(c => c.Code == "SWP");
                }
                return _swapsCodeList;
            }

            set
            {
                _swapsCodeList = value;
            }
        }

        public IEnumerable<Customer> SwapsCustomersList
        {
            get
            {               
                var cnt = _rmsEntities.Customers.ToList().Count();
                var swapsCustomersList = new List<Customer>(cnt);

                foreach (var item in _rmsEntities.Customers)
                {
                    if (item.IsLenderBorrower)
                    {
                        swapsCustomersList.Add(item);
                    } 
                }
                return swapsCustomersList;
            }
            //set
            //{
            //    _swapsCustomersList = value;
            //}
        }

        public ObservableCollection<SaleDetailExtn> SwapsDetailList
        {
            get { return _swapsDetailsList; }
            private set
            {
                _swapsDetailsList = value;
            }
        }

        public SwapsViewModel()
        {
            _rmsEntities = new RMSEntities();
            var cnt =_rmsEntities.CodeMasters.ToList().Count();
            Title = "Lend/Borrow stocks";
            _transcationDate = DateTime.Now;
            _swapsDetailsList = new ObservableCollection<SaleDetailExtn>();
        }

        private void Clear()
        {
            _rmsEntities = new RMSEntities();
            RefreshProductList();
            //SelectedCustomer.Id = 1;
            //SelectedPaymentId = '0';
            //OrderNo = "";
            SwapsDetailList = new ObservableCollection<SaleDetailExtn>();
            //_billSales = _rmsEntities.Sales.Create();
            //_totalAmount = 0;
            //TotalAmount = null;
            //AmountPaid = 0.0M;
            //_extensions.Clear();
            //RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
            //_isEditMode = false;
        }

        internal void SetProductDetails(ProductPrice productPrice, int selectedIndex)
        {
            if (productPrice == null) return;
            var saleItem = _swapsDetailsList.FirstOrDefault(s => s.ProductId == productPrice.ProductId && s.PriceId == productPrice.PriceId);
            var selRowSaleDetailExtn = _swapsDetailsList[selectedIndex];
            //SetSaleDetailExtn(productPrice, selRowSaleDetailExtn, selectedIndex);
        }

        //private void SetSaleDetailExtn(ProductPrice productPrice, SaleDetailExtn SaleDetailExtn, int selectedIndex)
        //{
        //    if (SaleDetailExtn != null)
        //    {
        //        SaleDetailExtn.SellingPrice = productPrice.SellingPrice;
        //        SaleDetailExtn.CostPrice = productPrice.Price;
        //        SaleDetailExtn.PriceId = productPrice.PriceId;
        //        SaleDetailExtn.AvailableStock = productPrice.Quantity;
        //        SaleDetailExtn.SerialNo = selectedIndex;

        //        //var customerSales = _rmsEntities.Sales.Where(s => s.CustomerId == _selectedCustomer.Id);//.OrderByDescending(d => d.ModifiedOn);
        //        var lastSoldPrice = RMSEntitiesHelper.Instance.GetLastSoldPrice(productPrice.ProductId, _selectedCustomer.Id);
        //        if (lastSoldPrice != null)
        //        {
        //            //var lastSaleDetail = customerSales. SaleDetails.Where(p => p.ProductId == productPrice.ProductId).OrderByDescending(d => d.ModifiedOn);
        //            //var lastSaleDetail1 = customerSales.SaleDetails.Where(p => p.ProductId == productPrice.ProductId);
        //            ////var lastSoldPrice = lastSaleDetail != null ? lastSaleDetail.SellingPrice : 0;

        //            SaleDetailExtn.LastSoldPrice = lastSoldPrice;
        //        }

        //        //if (lastSoldDateByCustomer.Any())
        //        //{
        //        //    var lastSaleDetail = lastSoldDateByCustomer.FirstOrDefault().SaleDetails.Where(sd => sd.ProductId == productPrice.ProductId).FirstOrDefault();

        //        //    var lastSoldPrice = lastSaleDetail != null ? lastSaleDetail.SellingPrice : 0;

        //        //    SaleDetailExtn.LastSoldPrice = lastSoldPrice;
        //        //}


        //        SaleDetailExtn.PropertyChanged += (sender, e) =>
        //        {
        //            var prop = e.PropertyName;
        //            if (prop == Constants.AMOUNT)
        //            {
        //                TotalAmount = SaleDetailList.Sum(a => a.Amount);
        //                return;
        //            }
        //            var amount = SaleDetailExtn.SellingPrice * SaleDetailExtn.Qty;
        //            var discountAmount = SaleDetailExtn.DiscountPercentage != 0 ?
        //                                 amount - (amount * (SaleDetailExtn.DiscountPercentage / 100)) :
        //                                 SaleDetailExtn.DiscountAmount != 0 ?
        //                                 amount - SaleDetailExtn.DiscountAmount :
        //                                 0;

        //            if (discountAmount != 0)
        //            {
        //                SaleDetailExtn.Amount = discountAmount;
        //                SaleDetailExtn.Discount = amount - discountAmount;
        //                return;
        //            }

        //            SaleDetailExtn.Amount = amount;
        //            SaleDetailExtn.Discount = 0;
        //        };
        //        //&& SaleDetailExtn.AvailableStock >
        //        //SaleDetailExtn.Qty.Value
        //        SaleDetailExtn.Qty = SaleDetailExtn.Qty.HasValue ? SaleDetailExtn.Qty.Value : 1;
        //    }
        //}


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

        protected void OnClose()
        {
            Workspace.This.Close(this);
        }

        #endregion
    }
}
