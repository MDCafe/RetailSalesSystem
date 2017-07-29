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
        string _selectedCompanyText;
        private decimal? _coolieCharges;
        private decimal? _kCoolieCharges;
        private decimal? _transportCharges;
        private decimal? _localCoolieCharges;


        public PurchaseEntryViewModel(bool showRestrictedCompanies) : base(showRestrictedCompanies)
        {
            Title = "Purchase Entry ";
            var count = RMSEntitiesHelper.Instance.RMSEntities.Companies.ToList();
            _purchaseDetailsList = new ObservableCollection<PurchaseDetailExtn>();

            RMSEntitiesHelper.Instance.AddPurchaseNotifier(this);
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
            _rmsEntities = new RMSEntities();
        }

        #region Getters and Setters
        public ObservableCollection<PurchaseDetailExtn> PurchaseDetailList
        {
            get { return _purchaseDetailsList; }
            private set
            {
                _purchaseDetailsList = value;
                RaisePropertyChanged("PurchaseDetailList");
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

        public string SelectedCompanyText
        {
            get { return _selectedCompanyText; }
            set
            {
                _selectedCompanyText = value;
                //NotifyPropertyChanged(() => this._selectedCustomer);
                RaisePropertyChanged("SelectedCompanyText");
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

                if (_totalDiscountAmount.HasValue)
                    DiscountPercentEnabled = false;
                else
                    DiscountPercentEnabled = true;

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

                if (_totalDiscountPercent.HasValue)
                    DiscountEnabled = false;
                else
                    DiscountEnabled = true;

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

        public decimal? CoolieCharges
        {
            get { return _coolieCharges; }
            set
            {
                _coolieCharges = value;
                CalculateTotalCharges();
                RaisePropertyChanged("CoolieCharges");
            }
        }

        public decimal? KCoolieCharges
        {
            get { return _kCoolieCharges; }
            set
            {
                _kCoolieCharges = value;
                CalculateTotalCharges();
                RaisePropertyChanged("KCoolieCharges");
            }
        }

        public decimal? TransportCharges
        {
            get { return _transportCharges; }
            set
            {
                _transportCharges = value;
                RaisePropertyChanged("TransportCharges");
            }
        }

        public decimal? LocalCoolieCharges
        {
            get { return _localCoolieCharges; }
            set
            {
                _localCoolieCharges = value;
                RaisePropertyChanged("LocalCoolieCharges");
            }
        }

        private void CalculateTotalCharges()
        {
           if(_coolieCharges.HasValue)
            {
                TotalAmount += _coolieCharges;
            }

            if (_kCoolieCharges.HasValue)
            {
                TotalAmount += _kCoolieCharges;
            }
        }

        #endregion

        private void CalculateTotalAmount()
        {
            decimal? tempTotal = _purchaseDetailsList.Sum(a => a.Amount);
            decimal? discountValue = 0;

            if (_totalDiscountAmount.HasValue)
            {
                tempTotal -= _totalDiscountAmount;
                discountValue = _totalDiscountAmount;
            }

            if (_totalDiscountPercent.HasValue)
            {
                discountValue = tempTotal * (_totalDiscountPercent / 100);
                tempTotal -= discountValue;
            }

            var count = _purchaseDetailsList.Count();
            if (count != 0)
            {
                //Change the cost price based on discounts
                var amountToReduce = discountValue / count;
                foreach (var item in _purchaseDetailsList)
                {
                    var itemAmt = item.Amount - amountToReduce;
                    var totalQty = item.Qty + (item.FreeIssue.HasValue ? item.FreeIssue.Value : 0);
                    var costPerItem = itemAmt / totalQty; 
                    if(costPerItem.HasValue)
                        item.CostPrice = costPerItem.Value;
                }
            }

            if (_specialDiscountAmount.HasValue)
            {
                tempTotal -= _specialDiscountAmount.Value;
            }


            _totalAmount = tempTotal;
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
                        totalQtyWithFreeIssue = purchaseDetailExtn.Qty.HasValue ? purchaseDetailExtn.Qty.Value : 0;

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
                                if (totalQtyWithFreeIssue == 0) return;
                                purchaseDetailExtn.CostPrice = TotalAmount.Value / totalQtyWithFreeIssue;
                                break;
                            }
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
                purchaseDetail.ActualPrice = item.PurchasePrice.Value;

                var priceDetails = _rmsEntities.PriceDetails.Where(pr => pr.ProductId == item.ProductId
                                                                        && pr.Price == item.PurchasePrice
                                                                        && pr.SellingPrice == item.SellingPrice);
                var priceId = 0;
                PriceDetail priceDetailItem = null;
                if (priceDetails.Any())
                {
                    //Same item exists. Just update the with new billId 
                    var priceItem  = priceDetails.FirstOrDefault();
                    priceItem.BillId = RunningBillNo;
                    priceId = priceItem.PriceId;
                }
                else
                {
                    //New Price, add it to price details list
                    priceDetailItem = new PriceDetail()
                    {
                        BillId = RunningBillNo,
                        ProductId = item.ProductId,
                        Price = item.PurchasePrice.Value,
                        SellingPrice = item.SellingPrice.Value
                    };
                    _rmsEntities.PriceDetails.Add(priceDetailItem);
                }

                var stock = _rmsEntities.Stocks.Where(s => s.ProductId == item.ProductId
                                                            && s.PriceId == priceId
                                                            && s.ExpiryDate.CompareTo(item.ExpiryDate.Value) == 0);

                var stock1 = _rmsEntities.Stocks.Where(s => s.ProductId == item.ProductId
                                                            && s.PriceId == priceId
                                                            && s.ExpiryDate.ToString("dd/MM/yyyy") == item.ExpiryDate.Value.ToString("dd/MM/yyyy"));
                var qty = item.Qty;
                if (item.FreeIssue.HasValue)
                {
                    qty = item.FreeIssue.Value + item.Qty.Value;
                    _rmsEntities.PurchaseFreeDetails.Add(
                        new PurchaseFreeDetail()
                        {
                            ProductId = item.ProductId,
                            FreeQty = item.FreeIssue.Value,
                            FreeAmount = item.PurchasePrice * item.FreeIssue.Value
                        });
                }
                if (stock.Any())
                {                                      
                    stock.FirstOrDefault().Quantity += qty.Value;
                }
                else
                {
                    //Add stock for new price
                    _rmsEntities.Stocks.Add(new Stock()
                    {
                        PriceId = priceDetailItem.PriceId,
                        ExpiryDate = item.ExpiryDate.Value,
                        Quantity = qty.Value
                    });
                }

                purchaseDetail.PurchasedQty = qty;
                purchase.PurchaseDetails.Add(purchaseDetail);
            }


            var _category = _rmsEntities.Categories.FirstOrDefault(c => c.Id == _categoryId);
            _category.RollingNo = _runningBillNo;

            _rmsEntities.Purchases.Add(purchase);
            _rmsEntities.SaveChanges();


            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
            RunningBillNo = _runningBillNo;            

            Clear();
        }

        private void Clear()
        {
            PurchaseDetailList = new ObservableCollection<PurchaseDetailExtn>();
            TotalAmount = null;
            TotalDiscountAmount = null;
            TotalDiscountPercent = null;
            SpecialDiscountAmount = null;
            SelectedCompany = null;
            InvoiceNo = null;
            SelectedPaymentId = '0';
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
        }

        #endregion
    }
}
