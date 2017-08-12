using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System;
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
        List<PurchaseDetailExtn> _deletedItems;
        private RMSEntities _rmsEntities;        
        string _selectedCompanyText;
        private decimal? _coolieCharges;
        private decimal? _kCoolieCharges;
        private decimal? _transportCharges;
        private decimal? _localCoolieCharges;
        private int? _editBillNo;
        PurchaseParams _purchaseParams;

        public PurchaseEntryViewModel(PurchaseParams purchaseParams) : base(purchaseParams != null ? purchaseParams.ShowAllCompanies : false)
        {
            Title = "Purchase Entry ";
            var count = RMSEntitiesHelper.Instance.RMSEntities.Companies.ToList();
            _purchaseDetailsList = new ObservableCollection<PurchaseDetailExtn>();
            _purchaseDetailsList.CollectionChanged += PurchaseDetailsListCollectionChanged;

            _purchaseParams = purchaseParams;

            RMSEntitiesHelper.Instance.AddPurchaseNotifier(this);
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
            _rmsEntities = new RMSEntities();

            if (purchaseParams != null)
            {

                if (purchaseParams.Billno.HasValue)
                {
                    //Amend Bill             
                    OnEditBill(purchaseParams.Billno.Value);
                    Title = "Purchase Bill Amend : " + _runningBillNo;
                    return;
                }
            }
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
                 ApplyDiscountToItemCostPrice(_totalDiscountAmount);
                

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

                var discountValue = _totalAmount * (_totalDiscountPercent / 100);
                ApplyDiscountToItemCostPrice(discountValue);

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
                CalculateTotalAmount();
                ApplyExpensesToItemCostPrice();
                RaisePropertyChanged("CoolieCharges");
            }
        }

        public decimal? KCoolieCharges
        {
            get { return _kCoolieCharges; }
            set
            {
                _kCoolieCharges = value;
                CalculateTotalAmount();
                ApplyExpensesToItemCostPrice();
                RaisePropertyChanged("KCoolieCharges");
            }
        }

        public decimal? TransportCharges
        {
            get { return _transportCharges; }
            set
            {
                _transportCharges = value;
                CalculateTotalAmount();
                ApplyExpensesToItemCostPrice();
                RaisePropertyChanged("TransportCharges");
            }
        }

        public decimal? LocalCoolieCharges
        {
            get { return _localCoolieCharges; }
            set
            {
                _localCoolieCharges = value;
                CalculateTotalAmount();
                ApplyExpensesToItemCostPrice();
                RaisePropertyChanged("LocalCoolieCharges");
            }
        }

        public decimal? TotalTax { get; set; }

        #endregion

        private void CalculateTotalAmount()
        {
            decimal? tempTotal = _purchaseDetailsList.Sum(a => a.Amount);
            var expenses = CalculateExpenses();
            tempTotal = tempTotal - GetDiscount();

            if (_specialDiscountAmount.HasValue)
            {
                tempTotal -= _specialDiscountAmount.Value;
            }

            _totalAmount = tempTotal + expenses;
            RaisePropertyChanged("TotalAmount");
            RaisePropertyChanged("BalanceAmount");


        }

        private decimal? GetDiscount()
        {
            decimal? discountValue = 0;

            if (_totalDiscountAmount.HasValue)
            {
                discountValue = _totalDiscountAmount;
            }

            if (_totalDiscountPercent.HasValue)
            {
                discountValue = _totalAmount * (_totalDiscountPercent / 100);
            }

            return discountValue;
        }

        private decimal? CalculateLocalExpenses()
        {
            decimal? charges = 0M;
            if (_transportCharges.HasValue)
            {
                charges = _transportCharges;
            }

            if (_localCoolieCharges.HasValue)
            {
                charges += _localCoolieCharges;
            }
           
            return charges;
        }

        private decimal? CalculateExpenses()
        {
            decimal? charges = 0M;
            if (_coolieCharges.HasValue)
            {
                charges = _coolieCharges;
            }

            if (_kCoolieCharges.HasValue)
            {
                charges += _kCoolieCharges;
            }

            //ApplyExpensesToItemCostPrice();
            return charges;
        }

        private void ApplyDiscountToItemCostPrice(decimal? discountValue)
        {
            //Change the cost price based on 
            var count = _purchaseDetailsList.Count();
            if (count == 0) return;

            var expneses = CalculateExpenses() + CalculateLocalExpenses();

            var amountToReduce = (discountValue.HasValue ? discountValue.Value : 0) - expneses / count;
            foreach (var item in _purchaseDetailsList)
            {
                var itemAmt = item.Amount - amountToReduce;
                var totalQty = item.Qty + (item.FreeIssue.HasValue ? item.FreeIssue.Value : 0);
                var costPerItem = itemAmt / totalQty;
                if (costPerItem.HasValue)
                    item.CostPrice = costPerItem.Value;
            }
        }

        private void ApplyExpensesToItemCostPrice()
        {
            //Change the cost price based on expenses
            var count = _purchaseDetailsList.Count();
            if (count == 0) return;
           
            var expneses = CalculateExpenses() + CalculateLocalExpenses();
            var discount = GetDiscount();
            var amountToAdd = expneses - discount / count;
            foreach (var item in _purchaseDetailsList)
            {
                var itemAmt = item.Amount + amountToAdd;
                var totalQty = item.Qty + (item.FreeIssue.HasValue ? item.FreeIssue.Value : 0);
                var costPerItem = itemAmt / totalQty;
                if (costPerItem.HasValue)
                    item.CostPrice = costPerItem.Value;
            }
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

                var stock = _rmsEntities.Stocks.Where(s => s.ProductId == productPrice.ProductId && s.PriceId == productPrice.PriceId).FirstOrDefault();
                if (stock != null)
                {
                        purchaseDetailExtn.ExpiryDate = stock.ExpiryDate;
                }

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
                                    purchaseDetailExtn.CostPrice = purchaseDetailExtn.Amount.Value / totalQtyWithFreeIssue;
                                }
                                break;
                            }
                        case Constants.AMOUNT:
                            {
                                TotalAmount = _purchaseDetailsList.Sum(a => a.Amount);
                                if (totalQtyWithFreeIssue == 0) return;
                                purchaseDetailExtn.CostPrice = purchaseDetailExtn.Amount.Value / totalQtyWithFreeIssue;
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
                            purchaseDetailExtn.CostPrice = purchaseDetailExtn.Amount.Value / totalQtyWithFreeIssue;
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
            if (_isEditMode)
            {
                SaveOnEdit(parameter);
                return;
            }
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
                TransportCharges = TransportCharges,
                CoolieCharges = CoolieCharges,
                KCoolieCharges = KCoolieCharges,
                LocalCoolieCharges = LocalCoolieCharges,
                Tax = TotalTax,
                PaymentMode = SelectedPaymentId.ToString()
            };

            foreach (var item in _purchaseDetailsList)
            {
                var purchaseDetail = new PurchaseDetail();
                purchaseDetail.ProductId = item.ProductId;
                purchaseDetail.Discount = item.Discount;                
                purchaseDetail.ActualPrice = item.PurchasePrice.Value;
                purchaseDetail.Tax = item.Tax;

                var priceDetails = _rmsEntities.PriceDetails.Where(pr => pr.ProductId == item.ProductId
                                                                        && pr.Price == item.PurchasePrice
                                                                        && pr.SellingPrice == item.SellingPrice);
                var priceId = 0;
                PriceDetail priceDetailItem = null;
                if (priceDetails.Any())
                {
                    //Same item exists. Just update the with new billId 
                    priceDetailItem = priceDetails.FirstOrDefault();
                    priceDetailItem.BillId = item.BillId;
                    priceId = priceDetailItem.PriceId;
                    priceDetailItem.SellingPrice = item.SellingPrice.Value;
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
                var qty = item.Qty;
                if (item.FreeIssue.HasValue)
                {
                    qty = item.FreeIssue.Value + item.Qty.Value;
                    _rmsEntities.PurchaseFreeDetails.Add(
                        new PurchaseFreeDetail()
                        {
                            ProductId = item.ProductId,
                            FreeQty = item.FreeIssue.Value,
                            FreeAmount = item.PurchasePrice * item.FreeIssue.Value,
                            BillId = RunningBillNo
                           
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
                purchaseDetail.PriceId = priceDetailItem.PriceId;
                purchase.PurchaseDetails.Add(purchaseDetail);
            }

            //check if complete amount is paid, else mark it in PaymentDetails table against the customer
            var outstandingBalance = _totalAmount.Value - AmountPaid;
            if (outstandingBalance > 0)
            {
                //var msg = "Outstanding balance Rs " + outstandingBalance + ". Do you want to keep as pending balance amount?";
                //var result = Utility.ShowMessageBoxWithOptions(msg);
                //if (result == System.Windows.MessageBoxResult.Yes)
                //{
                    _rmsEntities.PurchasePaymentDetails.Add
                        (
                            new PurchasePaymentDetail   
                            {
                                PurchaseBillId = purchase.BillId,
                                AmountPaid = AmountPaid,
                                CompanyId = _selectedCompany.Id
                            }
                        );
                //}
                var company = _rmsEntities.Companies.FirstOrDefault(c => c.Id == SelectedCompany.Id);
                company.DueAmount = company.DueAmount.HasValue ? company.DueAmount.Value + outstandingBalance : outstandingBalance;
            }

            var _category = _rmsEntities.Categories.FirstOrDefault(c => c.Id == _categoryId);
            _category.RollingNo = _runningBillNo;

            _rmsEntities.Purchases.Add(purchase);
            _rmsEntities.SaveChanges();


            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
            RunningBillNo = _runningBillNo;            

            Clear();
        }

        private void SaveOnEdit(object parameter)
        {
            //Check if there are any deletions
            RemoveDeletedItems();

            var purchase = _rmsEntities.Purchases.Where(p => p.BillId == _editBillNo).FirstOrDefault();

            foreach (var purchaseDetailItemExtn in _purchaseDetailsList)
            {
                var purchaseDetail = _rmsEntities.PurchaseDetails.FirstOrDefault(b => b.BillId == purchaseDetailItemExtn.BillId
                                                                                 && b.ProductId == purchaseDetailItemExtn.ProductId);

                var priceDetails = _rmsEntities.PriceDetails.Where(pr => pr.ProductId == purchaseDetailItemExtn.ProductId
                                                                        && pr.Price == purchaseDetailItemExtn.PurchasePrice
                                                                        && pr.SellingPrice == purchaseDetailItemExtn.SellingPrice);
                var priceId = 0;
                PriceDetail priceDetailItem = null;
                priceDetailItem = GetPriceDetails(purchaseDetailItemExtn, priceDetails, ref priceId);

                if (purchaseDetail == null)
                {
                    purchaseDetail = _rmsEntities.PurchaseDetails.Create();
                    purchaseDetailItemExtn.OriginalQty = purchaseDetailItemExtn.Qty;
                    purchaseDetail.PriceId = priceDetailItem.PriceId;
                    _rmsEntities.PurchaseDetails.Add(purchaseDetail);

                    if (purchaseDetailItemExtn.FreeIssue.HasValue)
                    {
                        _rmsEntities.PurchaseFreeDetails.Add(
                            new PurchaseFreeDetail()
                            {
                                ProductId = purchaseDetailItemExtn.ProductId,
                                FreeQty = purchaseDetailItemExtn.FreeIssue.Value,
                                FreeAmount = purchaseDetailItemExtn.PurchasePrice * purchaseDetailItemExtn.FreeIssue.Value,
                                BillId = _editBillNo
                            });
                    }


                    SetPurchaseDetailItem(purchaseDetailItemExtn, purchaseDetail);

                    var stockNewItem = _rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == purchaseDetail.ProductId && s.PriceId == purchaseDetail.PriceId
                                                                          && s.ExpiryDate == purchaseDetailItemExtn.ExpiryDate);
                    if (stockNewItem != null)
                    {
                        stockNewItem.Quantity += purchaseDetail.PurchasedQty.Value;
                    }
                    continue;
                }

                if (purchaseDetailItemExtn.FreeIssue.HasValue)
                {
                    var freeIssueEdit = _rmsEntities.PurchaseFreeDetails.Where(f => f.BillId == purchaseDetailItemExtn.BillId &&
                                                                                f.ProductId == purchaseDetailItemExtn.ProductId).FirstOrDefault();
                    if (freeIssueEdit != null)
                        freeIssueEdit.FreeQty = purchaseDetailItemExtn.FreeIssue.Value;
                }


                SetPurchaseDetailItem(purchaseDetailItemExtn, purchaseDetail);
                var stock = _rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == purchaseDetail.ProductId && s.PriceId == purchaseDetail.PriceId
                                                                && s.ExpiryDate == purchaseDetailItemExtn.ExpiryDate);

                if (stock != null)
                {
                    var qtyToAdd = purchaseDetailItemExtn.OriginalQty.Value - purchaseDetail.PurchasedQty.Value;
                    stock.Quantity = (stock.Quantity + Math.Abs(qtyToAdd));
                }
            }

            if (purchase !=null)
            {
                purchase.Discount = GetDiscount();
                purchase.CoolieCharges = CoolieCharges;
                purchase.KCoolieCharges = KCoolieCharges;
                purchase.TransportCharges = TransportCharges;
                purchase.LocalCoolieCharges = LocalCoolieCharges;
                purchase.TotalBillAmount = TotalAmount;
                purchase.Tax = TotalTax;
            }
            _rmsEntities.SaveChanges();
            Clear();
            CloseCommand.Execute(null);

        }

        private PriceDetail GetPriceDetails(PurchaseDetailExtn purchaseDetailItemExtn, IQueryable<PriceDetail> priceDetails, ref int priceId)
        {
            PriceDetail priceDetailItem;
            if (priceDetails.Any())
            {
                //Same item exists. Just update the with new billId 
                priceDetailItem = priceDetails.FirstOrDefault();
                priceDetailItem.BillId = purchaseDetailItemExtn.BillId;
                priceId = priceDetailItem.PriceId;
                priceDetailItem.SellingPrice = purchaseDetailItemExtn.SellingPrice.Value;
            }
            else
            {
                //New Price, add it to price details list
                priceDetailItem = new PriceDetail()
                {
                    BillId = RunningBillNo,
                    ProductId = purchaseDetailItemExtn.ProductId,
                    Price = purchaseDetailItemExtn.PurchasePrice.Value,
                    SellingPrice = purchaseDetailItemExtn.SellingPrice.Value
                };
                _rmsEntities.PriceDetails.Add(priceDetailItem);
            }

            return priceDetailItem;
        }

        private void SetPurchaseDetailItem(PurchaseDetailExtn purchaseDetailExtn,  PurchaseDetail purchaseDetail)
        {
            purchaseDetail.Discount = purchaseDetailExtn.Discount;
            purchaseDetail.PriceId = purchaseDetailExtn.PriceId;
            purchaseDetail.ProductId = purchaseDetailExtn.ProductId;
            purchaseDetail.PurchasedQty = purchaseDetailExtn.Qty;
            purchaseDetail.ActualPrice = purchaseDetailExtn.PurchasePrice.Value;
            purchaseDetail.BillId = _editBillNo.Value;
            purchaseDetail.Tax = purchaseDetailExtn.Tax;
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
            PurchaseDetailList = new ObservableCollection<PurchaseDetailExtn>();
            TotalAmount = null;
            TotalDiscountAmount = null;
            TotalDiscountPercent = null;
            SpecialDiscountAmount = null;
            SelectedCompany = null;
            InvoiceNo = null;
            SelectedPaymentId = '0';
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId);
            CoolieCharges = null;
            KCoolieCharges = null;
            TransportCharges = null;
            LocalCoolieCharges = null;
            AmountPaid = 0.0M;
        }

        #endregion

        #region GetBill Command       

        private void OnEditBill(int? billNo)
        {
            if (billNo == null) throw new ArgumentNullException("Please enter a bill no");
            var runningBillNo = billNo;

            var purhcases = _rmsEntities.Purchases.Where(b => b.RunningBillNo == runningBillNo && b.CompanyId == _purchaseParams.CompanyId).FirstOrDefault();
            _editBillNo = purhcases.BillId;
            SelectedCompany = purhcases.Company;
            SelectedCompanyText = SelectedCompany.Name;
            TranscationDate = purhcases.AddedOn.Value;
            SelectedPaymentId = Char.Parse(purhcases.PaymentMode);
            InvoiceNo = purhcases.InvoiceNo;
            var purchaseDetailsForBill = _rmsEntities.PurchaseDetails.Where(b => b.BillId == purhcases.BillId).ToList();

            var tempTotalAmount = 0.0M;
            foreach (var item  in purchaseDetailsForBill.ToList())
            {
                var productPrice = _productsPriceList.Where(p => p.PriceId == item.PriceId).FirstOrDefault();
                var freeIssue = _rmsEntities.PurchaseFreeDetails.Where(p => p.BillId == item.BillId).FirstOrDefault();
                var purchaseDetailExtn = new PurchaseDetailExtn()
                {
                    Discount = item.Discount,
                    PriceId = item.PriceId.Value,
                    ProductId = item.ProductId.Value,
                    Qty = item.PurchasedQty,
                    OriginalQty = item.PurchasedQty,
                    //SellingPrice = item.SellingPrice,
                    BillId = item.BillId,
                    CostPrice = productPrice.Price,
                    AvailableStock = productPrice.Quantity,
                    Amount = item.ActualPrice * item.PurchasedQty
                };

                _purchaseDetailsList.Add(purchaseDetailExtn);
                SetPurchaseDetailExtn(productPrice, purchaseDetailExtn);

                purchaseDetailExtn.FreeIssue = freeIssue != null ? freeIssue.FreeQty : null;

                tempTotalAmount += item.ActualPrice * item.PurchasedQty.Value;
            }
            TotalAmount = tempTotalAmount;

            RunningBillNo = runningBillNo.Value;
            _isEditMode = true;

            if (_deletedItems == null)
                _deletedItems = new List<PurchaseDetailExtn>();
            else
                _deletedItems.Clear();
        }

        private void RemoveDeletedItems()
        {
            foreach (var item in _deletedItems)
            {
                var purchaseDetail = _rmsEntities.PurchaseDetails.FirstOrDefault(s => s.BillId == item.BillId && s.ProductId == item.ProductId);

                var stockNewItem = _rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == item.ProductId && s.PriceId == purchaseDetail.PriceId);
                if (stockNewItem != null)
                {
                    stockNewItem.Quantity += item.Qty.Value;
                }
                _rmsEntities.PurchaseDetails.Remove(purchaseDetail);
            }
        }

        private void PurchaseDetailsListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var purchaseDetailExtn = e.OldItems[0] as PurchaseDetailExtn;
                if (_isEditMode)
                    _deletedItems.Add(purchaseDetailExtn);
                TotalAmount -= purchaseDetailExtn.Amount;
            }
        }

        #endregion

    }
}
