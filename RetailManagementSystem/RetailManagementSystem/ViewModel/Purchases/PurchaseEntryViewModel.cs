using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;

namespace RetailManagementSystem.ViewModel.Purchases
{
    class PurchaseEntryViewModel : PurchaseViewModelbase
    {
        private Company _selectedCompany;
        private decimal? _specialDiscountAmount;
        private string _invoiceNo;
        ObservableCollection<PurchaseDetailExtn> _purchaseDetailsList;
        private RMSEntities _rmsEntities;
        
        public PurchaseEntryViewModel(bool showRestrictedCompanies) : base(showRestrictedCompanies)
        {
            Title = "Purchase Entry ";
            var count = RMSEntitiesHelper.Instance.RMSEntities.Companies.ToList();
            _purchaseDetailsList = new ObservableCollection<PurchaseDetailExtn>();

            RMSEntitiesHelper.Instance.AddPurchaseNotifier(this);
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
            _rmsEntities = new RMSEntities();
        }      

        public ObservableCollection<PurchaseDetailExtn> PurchaseDetailList
        {
            get { return _purchaseDetailsList; }
            private set
            {
                _purchaseDetailsList = value;
            }
        }

        public IEnumerable<Company> CompaniesList
        {
            get
            {                
                return RMSEntitiesHelper.Instance.RMSEntities.Companies.Local.Where(c => c.CategoryTypeId == _categoryId);
            }
        }     

        public Company SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                _selectedCompany = value;
                RaisePropertyChanged("SelectedCompany");
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

        public decimal? SpecialDiscountAmount
        {
            get { return _specialDiscountAmount; }
            set
            {
                _specialDiscountAmount = value;
                CalculateTotalAmount();
                RaisePropertyChanged("SpecialDiscountAmount");
            }
        }
  
        public string InvoiceNo
        {
            get { return _invoiceNo; }
            set
            {
                _invoiceNo = value;                
                RaisePropertyChanged("InvoiceNo");
            }
        }

        private void CalculateTotalAmount()
        {
            decimal? tempTotal = _purchaseDetailsList.Sum(a => a.Amount); ;
            if (_totalDiscountAmount.HasValue)
            {
                tempTotal -= _totalDiscountAmount;
            }

            if (_totalDiscountPercent.HasValue)
            {
                var discountValue = tempTotal * (_totalDiscountPercent / 100);
                tempTotal -= discountValue;
            }

            if(_specialDiscountAmount.HasValue)
            {
                tempTotal -= _specialDiscountAmount.Value;
            }

            _totalAmount = tempTotal;
            //TotalAmountDisplay = _totalAmount.Value;
            RaisePropertyChanged("TotalAmount");
            RaisePropertyChanged("BalanceAmount");            
        }

        public void SetProductDetails(ProductPrice productPrice, int selectedIndex)
        {
            if (productPrice == null) return;
            var saleItem = _purchaseDetailsList.FirstOrDefault(s => s.ProductId == productPrice.ProductId && s.PriceId == productPrice.PriceId);
            var selRowSaleDetailExtn = _purchaseDetailsList[selectedIndex];
            if (saleItem != null)
            {
                Utility.ShowWarningBox("Item is already added");
                selRowSaleDetailExtn.ProductId = 0;
                return;
            }
            SetPurchaseDetailExtn(productPrice, selRowSaleDetailExtn);
        }

        private void SetPurchaseDetailExtn(ProductPrice productPrice, PurchaseDetailExtn purchaseDetailExtn)
        {
            if (purchaseDetailExtn != null)
            {
                //selectedRowSaleDetail.Qty = productPrice.Quantity;
                purchaseDetailExtn.PurchasePrice = productPrice.Price;
                purchaseDetailExtn.OldCostPrice = productPrice.Price;
                purchaseDetailExtn.CostPrice = productPrice.Price;
                purchaseDetailExtn.PriceId = productPrice.PriceId;
                purchaseDetailExtn.AvailableStock = productPrice.Quantity;
                purchaseDetailExtn.SellingPrice = productPrice.SellingPrice;
                purchaseDetailExtn.OldSellingPrice = productPrice.SellingPrice;

                purchaseDetailExtn.PropertyChanged += (sender, e) =>
                {
                    var prop = e.PropertyName;
                    var totalQtyWithFreeIssue = 0.0M;

                    if (purchaseDetailExtn.FreeIssue.HasValue)
                        totalQtyWithFreeIssue = purchaseDetailExtn.Qty.Value + purchaseDetailExtn.FreeIssue.Value;
                    else
                        totalQtyWithFreeIssue = purchaseDetailExtn.Qty.Value;

                    switch (e.PropertyName)
                    {
                        case Constants.FREE_ISSUE:
                            {
                                if (purchaseDetailExtn.Qty.HasValue && purchaseDetailExtn.FreeIssue.HasValue)
                                {
                                    purchaseDetailExtn.CostPrice = TotalAmount.Value / totalQtyWithFreeIssue;
                                }
                                break;
                            }


                        case Constants.AMOUNT:
                            {
                                TotalAmount = _purchaseDetailsList.Sum(a => a.Amount);
                                purchaseDetailExtn.CostPrice = TotalAmount.Value / totalQtyWithFreeIssue;
                                break;
                            }

                        //case Constants.PURCHASE_PRICE:
                        //    {
                        //        if (purchaseDetailExtn.Qty.HasValue)
                        //        {
                        //            purchaseDetailExtn.CostPrice = TotalAmount.Value / purchaseDetailExtn.Qty.Value;
                        //        }
                        //        break;
                        //    }
                    }
                                       
                    var amount = purchaseDetailExtn.PurchasePrice * purchaseDetailExtn.Qty;
                    var discountAmount = purchaseDetailExtn.DiscountPercentage != 0 ?
                                         amount - (amount * (purchaseDetailExtn.DiscountPercentage / 100)) :
                                         purchaseDetailExtn.DiscountAmount != 0 ?
                                         amount - purchaseDetailExtn.DiscountAmount :
                                         0;

                    if (discountAmount != 0)
                    {
                        purchaseDetailExtn.Amount = discountAmount;
                        purchaseDetailExtn.Discount = discountAmount;
                        if (purchaseDetailExtn.Qty.HasValue)
                        {
                            purchaseDetailExtn.CostPrice = TotalAmount.Value / totalQtyWithFreeIssue;
                        }
                        return;
                    }

                    purchaseDetailExtn.Amount = amount;
                    purchaseDetailExtn.Discount = 0;
                };
            }
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
            //return _selectedCustomer != null && _selectedCustomer.Id != 0 && _salesDetailsList.Count != 0 &&
            //        _salesDetailsList[0].ProductId != 0 && _selectedCustomerText == _selectedCustomer.Name;
            ////return IsDirty;
            return true;
        }

        private void OnSave(object parameter)
        {                        
            Clear();

            //Add free items to free items table
            //Sum up the free item to main stock
            var purchase = new Purchase()
            {
                RunningBillNo = RunningBillNo,
                CompanyId = SelectedCompany.Id,
                Discount = GetDiscountValue(),
                SpecialDiscount = SpecialDiscountAmount,
                InvoiceNo = InvoiceNo,
                TotalBillAmount = TotalAmount,
                //TransportCharges = 
                //Tax
            };

            foreach (var item in _purchaseDetailsList)
            {
                var purchaseDetail = new PurchaseDetail();
                purchaseDetail.ProductId = item.ProductId;
                purchaseDetail.Discount = item.Discount;
                purchaseDetail.PurchasedQty = item.Qty;
                //purchaseDetail.

            }

            _rmsEntities.Purchases.Add(purchase);
        }

        private void Clear()
        {
            _purchaseDetailsList = new ObservableCollection<PurchaseDetailExtn>();
            TotalAmount = null;
            TotalDiscountAmount = null;
            TotalDiscountPercent = null;
            SpecialDiscountAmount = null;
            SelectedCompany = null;
            InvoiceNo = null;
            SelectedPaymentId = '0';
        }

        #endregion
    }
}
