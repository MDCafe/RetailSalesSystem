using log4net;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.ViewModel.Entitlements;
using RetailManagementSystem.ViewModel.Reports.Purhcases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Purchases
{
    class PurchaseEntryViewModel : PurchaseViewModelbase
    {
        private Company _selectedCompany;
        private decimal? _specialDiscountAmount;
        private string _invoiceNo;
        ObservableCollection<PurchaseDetailExtn> _purchaseDetailsList;
        List<PurchaseDetailExtn> _deletedItems;
        private readonly RMSEntities _rmsEntities;
        string _selectedCompanyText;
        private decimal? _coolieCharges;
        private decimal? _kCoolieCharges;
        private decimal? _transportCharges;
        private decimal? _localCoolieCharges;
        private int? _editBillNo;
        readonly PurchaseParams _purchaseParams;
        private readonly AutoResetEvent _autoResetEvent;

        static readonly ILog _log = LogManager.GetLogger(typeof(PurchaseEntryViewModel));

        public PurchaseEntryViewModel(PurchaseParams purchaseParams) : base(purchaseParams != null && purchaseParams.ShowAllCompanies)
        {
            if (purchaseParams != null && purchaseParams.ShowAllCompanies)
                Title = "Purchase Entry*";
            else
                Title = "Purchase Entry";

            _rmsEntities = new RMSEntities();
            _autoResetEvent = new AutoResetEvent(false);

            var count = _rmsEntities.Companies.ToList();
            _purchaseDetailsList = new ObservableCollection<PurchaseDetailExtn>();
            _purchaseDetailsList.CollectionChanged += PurchaseDetailsListCollectionChanged;
            _purchaseParams = purchaseParams;

            RMSEntitiesHelper.Instance.AddPurchaseNotifier(this);
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId, true);

            if (purchaseParams != null)
            {

                if (purchaseParams.Billno.HasValue)
                {
                    //Amend Bill             
                    OnEditBill(purchaseParams.Billno.Value);
                    Title = "Purchase Bill Amend : " + _runningBillNo;
                    IsVisible = System.Windows.Visibility.Visible;
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
                RaisePropertyChanged(nameof(PurchaseDetailList));
            }
        }

        public IEnumerable<Company> CompaniesList
        {
            get
            {
                return _rmsEntities.Companies.Local.Where(c => c.CategoryTypeId == _categoryId).OrderBy(s => s.Name);
            }
        }

        public Company SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                _selectedCompany = value;
                RaisePropertyChanged(nameof(SelectedCompany));
            }
        }

        public string SelectedCompanyText
        {
            get { return _selectedCompanyText; }
            set
            {
                _selectedCompanyText = value;
                //NotifyPropertyChanged(() => this._selectedCustomer);
                RaisePropertyChanged(nameof(SelectedCompanyText));
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


                if (_totalDiscountAmount.HasValue && _totalDiscountAmount != 0)
                    DiscountPercentEnabled = false;
                else
                    DiscountPercentEnabled = true;

                CalculateTotalAmount();
                RaisePropertyChanged(nameof(TotalDiscountAmount));
            }
        }

        public decimal? TotalDiscountPercent
        {
            get { return _totalDiscountPercent; }
            set
            {
                _totalDiscountPercent = value;

                var totalAmount = _purchaseDetailsList.Sum(i => i.Amount);

                var discountValue = totalAmount * (_totalDiscountPercent / 100);
                ApplyDiscountToItemCostPrice(discountValue);

                if (_totalDiscountPercent.HasValue && _totalDiscountPercent != 0)
                    DiscountEnabled = false;
                else
                    DiscountEnabled = true;

                CalculateTotalAmount();
                RaisePropertyChanged(nameof(TotalDiscountPercent));
            }
        }

        public decimal? SpecialDiscountAmount
        {
            get { return _specialDiscountAmount; }
            set
            {
                _specialDiscountAmount = value;
                CalculateTotalAmount();
                RaisePropertyChanged(nameof(SpecialDiscountAmount));
            }
        }

        public string InvoiceNo
        {
            get { return _invoiceNo; }
            set
            {
                _invoiceNo = value;
                RaisePropertyChanged(nameof(InvoiceNo));
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
                RaisePropertyChanged(nameof(CoolieCharges));
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
                RaisePropertyChanged(nameof(KCoolieCharges));
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
                RaisePropertyChanged(nameof(TransportCharges));
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
                RaisePropertyChanged(nameof(LocalCoolieCharges));
            }
        }

        public decimal? TotalTax { get; set; }


        #endregion

        private void CalculateTotalAmount()
        {
            decimal? tempTotal = _purchaseDetailsList.Sum(a => a.Amount);
            var expenses = CalculateExpenses();
            tempTotal -= GetDiscount();

            if (_specialDiscountAmount.HasValue)
            {
                tempTotal -= _specialDiscountAmount.Value;
            }

            _totalAmount = tempTotal + expenses;
            RaisePropertyChanged(nameof(TotalAmount));
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
            var count = _purchaseDetailsList.Count;
            if (count == 0) return;

            var expneses = CalculateExpenses() + CalculateLocalExpenses();

            var amountToReduce = ((discountValue ?? 0) - expneses) / count;
            foreach (var item in _purchaseDetailsList)
            {
                //var itemAmt = item.Amount - amountToReduce;

                var totalQty = item.Qty + (item.FreeIssue ?? 0);
                var amtToReducePerItem = amountToReduce / totalQty;
                //var costPerItem = itemAmt / totalQty;
                //if (costPerItem.HasValue)
                item.CostPrice = item.PurchasePrice.Value - amtToReducePerItem.Value;
            }
        }

        private void ApplyExpensesToItemCostPrice()
        {
            //Change the cost price based on expenses
            var count = _purchaseDetailsList.Count;
            if (count == 0) return;

            var expneses = CalculateExpenses() + CalculateLocalExpenses();
            var discount = GetDiscount();
            var amountToAdd = expneses - discount / count;
            foreach (var item in _purchaseDetailsList)
            {
                var itemAmt = item.Amount + amountToAdd;
                var totalQty = item.Qty + (item.FreeIssue ?? 0);
                var costPerItem = itemAmt / totalQty;
                if (costPerItem.HasValue)
                    item.CostPrice = costPerItem.Value;
            }
        }

        public void SetProductDetails(ProductPrice productPrice, int selectedIndex)
        {
            if (productPrice == null) return;
            //var saleItem = _purchaseDetailsList.FirstOrDefault(s => s.ProductId == productPrice.ProductId && s.PriceId == productPrice.PriceId);
            var saleItem = _purchaseDetailsList.FirstOrDefault(s => s.ProductId == productPrice.ProductId);
            SetPurchaseDetailExtn(productPrice, saleItem, selectedIndex);
        }

        private void SetPurchaseDetailExtn(ProductPrice productPrice, PurchaseDetailExtn purchaseDetailExtn, int selectedIndex)
        {
            if (purchaseDetailExtn != null)
            {
                purchaseDetailExtn.PurchasePrice = productPrice.Price;
                purchaseDetailExtn.OldCostPrice = productPrice.Price;
                purchaseDetailExtn.CostPrice = productPrice.Price;
                purchaseDetailExtn.PriceId = productPrice.PriceId;
                purchaseDetailExtn.AvailableStock = productPrice.Quantity;
                purchaseDetailExtn.SellingPrice = productPrice.SellingPrice;
                purchaseDetailExtn.OldSellingPrice = productPrice.SellingPrice;
                purchaseDetailExtn.SerialNo = ++selectedIndex;
                purchaseDetailExtn.SupportsMultiplePrice = productPrice.SupportsMultiplePrice;
                purchaseDetailExtn.UnitPerCase = productPrice.UnitPerCase ?? 0;
                purchaseDetailExtn.ExpiryDate = DateTime.Now.AddYears(1);

                purchaseDetailExtn.SubscribeToAmountChange(() =>
                {
                    TotalAmount = PurchaseDetailList.Sum(a => a.Amount);
                    //purchaseDetailExtn.CalculateCost(purchaseDetailExtn.FreeIssue);
                });

                //purchaseDetailExtn.PropertyChanged += (sender, e) =>
                //{
                //    var prop = e.PropertyName;
                //    var totalQtyWithFreeIssue = 0.0M;

                //    if (purchaseDetailExtn.FreeIssue.HasValue)
                //        totalQtyWithFreeIssue = purchaseDetailExtn.Qty.HasValue ?  purchaseDetailExtn.Qty.Value + purchaseDetailExtn.FreeIssue.Value : 0;
                //    else
                //        totalQtyWithFreeIssue = purchaseDetailExtn.Qty.HasValue ? purchaseDetailExtn.Qty.Value : 0;

                //    switch (e.PropertyName)
                //    {
                //        case Constants.FREE_ISSUE:
                //            {
                //                if (purchaseDetailExtn.Qty.HasValue && purchaseDetailExtn.FreeIssue.HasValue)
                //                {
                //                    purchaseDetailExtn.CostPrice = purchaseDetailExtn.Amount.Value / totalQtyWithFreeIssue;
                //                }
                //                break;
                //            }
                //        case Constants.AMOUNT:
                //            {
                //                TotalAmount = _purchaseDetailsList.Sum(a => a.Amount);
                //                if (totalQtyWithFreeIssue == 0) return;
                //                purchaseDetailExtn.CostPrice = purchaseDetailExtn.Amount.Value / totalQtyWithFreeIssue;
                //                break;
                //            }
                //    }

                //    var amount = purchaseDetailExtn.PurchasePrice * purchaseDetailExtn.Qty;
                //    var discountAmount = purchaseDetailExtn.DiscountPercentage != 0 ?
                //                         amount - (amount * (purchaseDetailExtn.DiscountPercentage / 100)) :
                //                         purchaseDetailExtn.DiscountAmount != 0 ?
                //                         amount - purchaseDetailExtn.DiscountAmount :
                //                         0;

                //    if (discountAmount != 0)
                //    {
                //        purchaseDetailExtn.Amount = discountAmount;
                //        purchaseDetailExtn.Discount = amount - discountAmount;
                //        if (purchaseDetailExtn.Qty.HasValue)
                //        {
                //            purchaseDetailExtn.CostPrice = purchaseDetailExtn.Amount.Value / totalQtyWithFreeIssue;
                //        }
                //        return;
                //    }

                //    purchaseDetailExtn.Amount = amount;
                //    purchaseDetailExtn.Discount = 0;
                //};
            }
        }

        public void SetProductId()
        {
            foreach (var item in _purchaseDetailsList)
            {
                item.OnPropertyChanged("ProductId");
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
                    _saveCommand = new RelayCommand<object>((p) => OnSave(p), (p) => CanSave());
                }

                return _saveCommand;
            }
        }

        public bool CanSave()
        {
            return _selectedCompany != null && _selectedCompany.Id != 0 && _purchaseDetailsList.Count != 0 &&
                    _purchaseDetailsList[0].ProductId != 0 && _selectedCompanyText == _selectedCompany.Name;
        }

        private void OnSave(object parameter)
        {
            if (_isEditMode)
            {
                //SaveOnEdit(parameter);
                return;
            }

            if (!Validate())
            {
                return;
            }

            PanelLoading = true;
            var purchaseSaveTask = System.Threading.Tasks.Task.Run(() =>
             {
                 try
                 {
                     using (var rmsEntities = new RMSEntities())
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
                             TransportCharges = TransportCharges,
                             CoolieCharges = CoolieCharges,
                             KCoolieCharges = KCoolieCharges,
                             LocalCoolieCharges = LocalCoolieCharges,
                             Tax = TotalTax,
                             PaymentMode = SelectedPaymentId.ToString(),
                             AddedOn = _transcationDate,
                             ModifiedOn = RMSEntitiesHelper.GetServerDate(),
                             UpdatedBy = EntitlementInformation.UserInternalId
                         };

                         var combinedDateTime = RMSEntitiesHelper.GetCombinedDateTime();

                         foreach (var item in _purchaseDetailsList)
                         {                             
                             var purchaseDetail = new PurchaseDetail
                             {
                                 ProductId = item.ProductId,
                                 Discount = item.Discount,
                                 ActualPrice = item.PurchasePrice.Value,
                                 Tax = item.Tax,
                                 AddedOn = _transcationDate,
                                 ModifiedOn = RMSEntitiesHelper.GetServerDate(),
                                 UpdatedBy = EntitlementInformation.UserInternalId,
                                 VATAmount = item.VATPercentage.HasValue && item.VATPercentage.Value != 0 ?
                                                         (item.PurchasePrice * (item.VATPercentage / 100)) * item.Qty
                                                         :
                                                         item.VATAmount,


                                 ItemCoolieCharges = item.ItemCoolieCharges,
                                 ItemTransportCharges = item.ItemTransportCharges,                                 
                             };

                             SetPriceDetails(rmsEntities, item, purchaseDetail, out int priceId, out PriceDetail priceDetailItem, item.SupportsMultiplePrice);

                             var qty = item.GetQty();
                             if (item.FreeIssue.HasValue)
                             {
                                 qty = item.FreeIssue.Value + item.Qty.Value;
                                 rmsEntities.PurchaseFreeDetails.Add(
                                     new PurchaseFreeDetail()
                                     {
                                         ProductId = item.ProductId,
                                         FreeQty = item.FreeIssue.Value,
                                         FreeAmount = item.PurchasePrice * item.FreeIssue.Value,
                                         IsFreeOnly = item.Qty == 0 && item.FreeIssue.HasValue,
                                         AddedOn = _transcationDate,
                                         ModifiedOn = RMSEntitiesHelper.GetServerDate(),
                                         UpdatedBy = EntitlementInformation.UserInternalId
                                     });
                             }

                             purchaseDetail.PurchasedQty = qty;
                             SetStockDetails(rmsEntities, item, purchaseDetail, priceId, priceDetailItem, qty, item.SupportsMultiplePrice, combinedDateTime);

                             purchaseDetail.PriceId = priceDetailItem.PriceId;
                             purchaseDetail.PriceDetail = priceDetailItem;
                             purchase.PurchaseDetails.Add(purchaseDetail);
                         }

                         //check if complete amount is paid, else mark it in PaymentDetails table against the customer
                         var outstandingBalance = _totalAmount.Value - AmountPaid;
                         if (outstandingBalance > 0)
                         {
                             rmsEntities.PurchasePaymentDetails.Add
                                 (
                                     new PurchasePaymentDetail
                                     {
                                         PurchaseBillId = purchase.BillId,
                                         AmountPaid = AmountPaid,
                                         CompanyId = _selectedCompany.Id
                                     }
                                 );
                             var company = rmsEntities.Companies.FirstOrDefault(c => c.Id == SelectedCompany.Id);
                             company.DueAmount = company.DueAmount.HasValue ? company.DueAmount.Value + outstandingBalance : outstandingBalance;
                         }


                         RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId, false);
                         purchase.RunningBillNo = _runningBillNo;

                         var _category = rmsEntities.Categories.FirstOrDefault(c => c.Id == _categoryId);
                         _category.RollingNo = _runningBillNo;

                         rmsEntities.Purchases.Add(purchase);
                         rmsEntities.SaveChanges();
                     }

                     var currentRunningBillNo = _runningBillNo;

                     if (parameter.ToString() == "PrintSave")
                     {
                         App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            //Call the print on print & save
                            PurchaseSummaryViewModel psummVM = new PurchaseSummaryViewModel(_showRestrictedCompanies, _runningBillNo)
                            {
                                RunningBillNo = currentRunningBillNo
                            };
                            psummVM.PrintCommand.Execute(null);
                        }));
                     }

                     Clear();
                 }
                 catch (Exception ex)
                 {
                     Utility.ShowErrorBox("Error while saving..!!" + ex.Message);
                     _log.Error("Error while saving..!!", ex);
                 }

             }).ContinueWith(
            (t) =>
            {
                PanelLoading = false;
                _autoResetEvent.Set();
            });

        }

        private bool Validate()
        {
            foreach (var item in _purchaseDetailsList)
            {
                var qty = item.GetQty();

                if ((qty == null || qty <= 0) && (!item.FreeIssue.HasValue))
                {
                    Utility.ShowErrorBox("Purchase quantity can't be empty or zero");
                    return false;
                }

                if (item.SellingPrice == null || item.SellingPrice <= 0)
                {
                    Utility.ShowErrorBox("Selling price can't be empty or zero");
                    return false;
                }
                if (item.ExpiryDate == null || item.ExpiryDate <= DateTime.Now)
                {
                    Utility.ShowErrorBox("Expiry Date can't be empty or less than today's date");
                    return false;
                }

                if (item.CostPrice < 0)
                {
                    Utility.ShowErrorBox("Cost Price can't be less than zero");
                    return false;
                }

                if (item.CostPrice > item.SellingPrice)
                {
                    Utility.ShowErrorBox("Cost Price can't be greater than Selling Price");
                    return false;
                }
            }

            if (_selectedCompany == null)
            {
                Utility.ShowMessageBox("Select a supplier");
                return false;
            }

            return true;
        }

        private void SetStockDetails(RMSEntities rmsEntities, PurchaseDetailExtn item, PurchaseDetail purchaseDetail, int priceId, PriceDetail priceDetailItem, decimal? qty,
                                    bool supportsMultiplePrice, DateTime combinedDateTime)
        {
            IEnumerable<Stock> stock = null;


            if (supportsMultiplePrice)
                stock = rmsEntities.Stocks.Where(s => s.ProductId == item.ProductId
                                                && s.PriceId == priceId
                                                && s.ExpiryDate.CompareTo(item.ExpiryDate.Value) == 0);
            else
                stock = rmsEntities.Stocks.Where(s => s.ProductId == item.ProductId
                                                                && s.PriceId == priceId);

            if (stock.Any())
            {
                var st = stock.FirstOrDefault();
                st.Quantity += qty.Value;
                st.ExpiryDate = item.ExpiryDate.Value;

                SetStockTransaction(rmsEntities, purchaseDetail, st, combinedDateTime);
            }
            else
            {
                //Add stock for new price
                var newStock = new Stock()
                {
                    //PriceId = priceDetailItem.PriceId,
                    ExpiryDate = item.ExpiryDate.Value,
                    Quantity = qty.Value,
                    ProductId = item.ProductId,
                    PriceDetail = priceDetailItem
                };
                //first time stock for the new price
                var firstStockTrans = new StockTransaction()
                {
                    OpeningBalance = item.Qty,
                    Inward = item.Qty,
                    ClosingBalance = item.Qty,
                    AddedOn = combinedDateTime
                };
                firstStockTrans.Stock = newStock;

                rmsEntities.StockTransactions.Add(firstStockTrans);
                rmsEntities.Stocks.Add(newStock);

                newStock.PriceDetail = priceDetailItem;
            }
        }

        private void SetPriceDetails(RMSEntities rmsEntities, PurchaseDetailExtn item, PurchaseDetail purchaseDetail, out int priceId, out PriceDetail priceDetailItem,
                                     bool supportsMultiplePrice)
        {
            IEnumerable<PriceDetail> priceDetails = null;

            if (supportsMultiplePrice)
                priceDetails = rmsEntities.PriceDetails.Where(pr => pr.ProductId == item.ProductId
                                                                && pr.Price == item.CostPrice
                                                                && pr.SellingPrice == item.SellingPrice);
            else
                priceDetails = rmsEntities.PriceDetails.Where(pr => pr.ProductId == item.ProductId
                                                                    && pr.PriceId == item.PriceId);
            priceId = 0;
            priceDetailItem = null;
            if (priceDetails.Any())
            {
                //Same item exists. Just update the with new billId 
                priceDetailItem = priceDetails.FirstOrDefault();
                //priceDetailItem.BillId = item.BillId;
                priceId = priceDetailItem.PriceId;
                priceDetailItem.SellingPrice = item.SellingPrice.Value;
                priceDetailItem.Price = item.CostPrice;
                purchaseDetail.PriceDetail = priceDetailItem;
            }
            else
            {
                //New Price, add it to price details list
                priceDetailItem = new PriceDetail()
                {
                    BillId = RunningBillNo,
                    ProductId = item.ProductId,
                    Price = item.CostPrice,
                    SellingPrice = item.SellingPrice.Value
                };
                rmsEntities.PriceDetails.Add(priceDetailItem);
            }
        }

        //private void SaveOnEdit(object parameter)
        //{
        //    try
        //    {
        //        //Check if there are any deletions
        //        RemoveDeletedItems();

        //        var purchase = _rmsEntities.Purchases.Where(p => p.BillId == _editBillNo).FirstOrDefault();

        //        foreach (var purchaseDetailItemExtn in _purchaseDetailsList)
        //        {
        //            var purchaseDetail = _rmsEntities.PurchaseDetails.FirstOrDefault(b => b.BillId == purchaseDetailItemExtn.BillId
        //                                                                             && b.ProductId == purchaseDetailItemExtn.ProductId);

        //            var priceDetails = _rmsEntities.PriceDetails.Where(pr => pr.ProductId == purchaseDetailItemExtn.ProductId
        //                                                                    && pr.Price == purchaseDetailItemExtn.PurchasePrice
        //                                                                    && pr.SellingPrice == purchaseDetailItemExtn.SellingPrice);
        //            var priceId = 0;
        //            PriceDetail priceDetailItem = null;
        //            priceDetailItem = GetPriceDetails(purchaseDetailItemExtn, priceDetails, ref priceId);

        //            //New Item added on edit
        //            if (purchaseDetail == null)
        //            {
        //                if (purchaseDetailItemExtn.FreeIssue.HasValue)
        //                {
        //                    _rmsEntities.PurchaseFreeDetails.Add(
        //                        new PurchaseFreeDetail()
        //                        {
        //                            ProductId = purchaseDetailItemExtn.ProductId,
        //                            FreeQty = purchaseDetailItemExtn.FreeIssue.Value,
        //                            FreeAmount = purchaseDetailItemExtn.PurchasePrice * purchaseDetailItemExtn.FreeIssue.Value,
        //                            BillId = _editBillNo.Value,
        //                            IsFreeOnly = purchaseDetailItemExtn.Qty.HasValue ? true : false
        //                        });

        //                    //Continue only if free item is added
        //                    if (purchaseDetailItemExtn.Qty == 0 || purchaseDetailItemExtn.Qty == null)
        //                    {
        //                        var maxPriceId = _rmsEntities.Stocks.Where(s => s.ProductId == purchaseDetailItemExtn.ProductId).Max(i => i.PriceId);
        //                        var stockLastAddedItem = _rmsEntities.Stocks.FirstOrDefault(m => m.PriceId == maxPriceId);
        //                        //.Aggregate((agg,next) => next.PriceId > agg.PriceId ? next : agg);
        //                        stockLastAddedItem.Quantity += purchaseDetailItemExtn.FreeIssue.Value;

        //                        //This object is created only for free items only
        //                        var purchaseFreeItemOnly = new PurchaseDetail()
        //                        {
        //                            PurchasedQty = purchaseDetailItemExtn.FreeIssue.Value,
        //                            ProductId = purchaseDetailItemExtn.ProductId,
        //                            BillId = _editBillNo.Value
        //                        };

        //                        SetStockTransaction(purchaseFreeItemOnly, stockLastAddedItem);

        //                        continue;
        //                    }
        //                }

        //                var serverDate = RMSEntitiesHelper.GetServerDate();
        //                purchaseDetail = _rmsEntities.PurchaseDetails.Create();
        //                purchaseDetailItemExtn.OriginalQty = purchaseDetailItemExtn.Qty;
        //                purchaseDetail.PriceId = priceDetailItem.PriceId;
        //                purchaseDetail.AddedOn = serverDate;
        //                purchaseDetail.ModifiedOn = serverDate;
        //                //purchaseDetail. = purchaseDetailItemExtn.Qty;
        //                _rmsEntities.PurchaseDetails.Add(purchaseDetail);

        //                SetPurchaseDetailItem(purchaseDetailItemExtn, purchaseDetail);

        //                var stockNewItem = _rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == purchaseDetail.ProductId && s.PriceId == purchaseDetail.PriceId
        //                                                                      && s.ExpiryDate == purchaseDetailItemExtn.ExpiryDate);
        //                if (stockNewItem != null)
        //                {
        //                    stockNewItem.Quantity += purchaseDetail.PurchasedQty.Value;
        //                    SetStockTransaction(purchaseDetail, stockNewItem);
        //                }

        //                continue;
        //            }

        //            if (purchaseDetailItemExtn.FreeIssue.HasValue && purchaseDetailItemExtn.FreeIssue.Value > 0)
        //            {
        //                var freeIssueEdit = _rmsEntities.PurchaseFreeDetails.Where(f => f.BillId == purchaseDetailItemExtn.BillId &&
        //                                                                            f.ProductId == purchaseDetailItemExtn.ProductId).FirstOrDefault();
        //                if (freeIssueEdit != null)
        //                    freeIssueEdit.FreeQty = purchaseDetailItemExtn.FreeIssue.Value;
        //                else
        //                //New free item
        //                {
        //                    var purchaseFreeItem = new PurchaseFreeDetail()
        //                    {
        //                        BillId = purchaseDetailItemExtn.BillId,
        //                        FreeQty = purchaseDetailItemExtn.FreeIssue.Value,
        //                        FreeAmount = purchaseDetailItemExtn.FreeIssue.Value * purchaseDetailItemExtn.SellingPrice,
        //                        ProductId = purchaseDetailItemExtn.ProductId
        //                    };

        //                    _rmsEntities.PurchaseFreeDetails.Add(purchaseFreeItem);
        //                }
        //            }

        //            SetPurchaseDetailItem(purchaseDetailItemExtn, purchaseDetail);

        //            var oldPriceId = purchaseDetail.PriceId;
        //            //New  Price
        //            if (priceDetailItem.PriceId == 0)
        //                purchaseDetail.PriceDetail = priceDetailItem;
        //            else
        //                purchaseDetail.PriceId = priceDetailItem.PriceId;

        //            var stock = _rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == purchaseDetail.ProductId && s.PriceId == oldPriceId
        //                                                            && s.ExpiryDate == purchaseDetailItemExtn.ExpiryDate);

        //            if (stock != null)
        //            {
        //                var stockTransExisting = _rmsEntities.StockTransactions.AsEnumerable().FirstOrDefault(st => st.StockId == stock.Id
        //                                            && st.AddedOn.Value.Date == purchaseDetail.AddedOn.Value.Date);

        //                var purchaseQty = purchaseDetail.PurchasedQty.Value;
        //                if (priceDetailItem.PriceId == 0)
        //                    stock.PriceDetail = priceDetailItem;
        //                else
        //                    stock.PriceId = priceDetailItem.PriceId;
        //                if (purchaseDetailItemExtn.OriginalQty.Value > purchaseDetail.PurchasedQty.Value)
        //                {
        //                    var qty = purchaseDetailItemExtn.OriginalQty.Value - purchaseQty;
        //                    stock.Quantity -= qty;
        //                    if (stockTransExisting != null)
        //                    {
        //                        stockTransExisting.Inward -= qty;
        //                        stockTransExisting.ClosingBalance -= qty;
        //                    }
        //                }
        //                else if (purchaseDetailItemExtn.OriginalQty.Value < purchaseDetail.PurchasedQty.Value)
        //                {
        //                    var qtySmall = purchaseQty - purchaseDetailItemExtn.OriginalQty.Value;
        //                    stock.Quantity += qtySmall;
        //                    if (stockTransExisting != null)
        //                    {
        //                        stockTransExisting.Inward += qtySmall;
        //                        stockTransExisting.ClosingBalance += qtySmall;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //priceDetailItem.Stocks.Add(new Stock()
        //                //{
        //                // //   ProductId = purchaseDetailItemExtn.ProductId,
        //                //   // Quantity = purchaseDetailItemExtn.Qty.HasValue ? purchaseDetailItemExtn.Qty: 0.0F,


        //                //});
        //            }
        //        }

        //        if (purchase != null)
        //        {
        //            purchase.Discount = GetDiscount();
        //            purchase.CoolieCharges = CoolieCharges;
        //            purchase.KCoolieCharges = KCoolieCharges;
        //            purchase.TransportCharges = TransportCharges;
        //            purchase.LocalCoolieCharges = LocalCoolieCharges;
        //            purchase.TotalBillAmount = TotalAmount;
        //            purchase.Tax = TotalTax;
        //            purchase.SpecialDiscount = SpecialDiscountAmount;
        //            //purchase.ModifiedOn = RMSEntitiesHelper.GetServerDate();
        //        }
        //        _rmsEntities.SaveChanges();

        //        if (parameter.ToString() == "PrintSave")
        //        {
        //            App.Current.Dispatcher.BeginInvoke((Action)(() =>
        //            {
        //            //Call the print on print & save
        //            PurchaseSummaryViewModel psummVM = new PurchaseSummaryViewModel(_showRestrictedCompanies, _runningBillNo);
        //                psummVM.RunningBillNo = purchase.RunningBillNo;
        //                psummVM.PrintCommand.Execute(null);
        //            }));
        //        }

        //        Clear();
        //        CloseCommand.Execute(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(ex.Message, ex);
        //        Utility.ShowErrorBox(ex.Message);
        //    }
        //}

        private void SetStockTransaction(RMSEntities rmsEntities, PurchaseDetail purchaseDetail, Stock stockNewItem, DateTime combinedDateTime)
        {
            var stockTrans = rmsEntities.StockTransactions.Where(s => s.StockId == stockNewItem.Id).OrderByDescending(s => s.Id).FirstOrDefault();

            //stock transaction not available for this product. Add them
            if (stockTrans == null)
            {
                var firstStockTrans = new StockTransaction()
                {
                    OpeningBalance = stockNewItem.Quantity - purchaseDetail.PurchasedQty, //Opening balance will be the one from stock table 
                    Inward = purchaseDetail.PurchasedQty,
                    ClosingBalance = stockNewItem.Quantity,
                    StockId = stockNewItem.Id,
                    AddedOn = combinedDateTime
                };

                rmsEntities.StockTransactions.Add(firstStockTrans);
            }
            //stock transaction available. Check if it is for the current date else get the latest date and mark the opening balance
            else
            {
                using (RMSEntities stkTransRMSEntities = new RMSEntities())
                {
                    var sqlCheckSTForTransactionDate = "Select * from StockTransaction where StockId = " + stockNewItem.Id
                                                    + " and date(AddedOn) = '" + _transcationDate.ToString("yyyy-MM-dd") + "' order by AddedOn Desc";

                    var sqlCheckSTForTransactionDateResult = stkTransRMSEntities.Database.SqlQuery<StockTransaction>(sqlCheckSTForTransactionDate);

                    decimal openingBalanceForOthers = 0;

                    var stockAdjustCheck = RMSEntitiesHelper.CheckStockAdjustment(rmsEntities, stockNewItem.Id);

                    if (sqlCheckSTForTransactionDateResult.Any() && stockAdjustCheck != null && !stockAdjustCheck.StockTransId.HasValue)
                    {
                        var stockIdForTransDate = sqlCheckSTForTransactionDateResult.FirstOrDefault().Id;
                        var stockForTransDate = rmsEntities.StockTransactions.FirstOrDefault(std => std.Id == stockIdForTransDate);

                        stockForTransDate.Inward = stockForTransDate.Inward.HasValue ? stockForTransDate.Inward + purchaseDetail.PurchasedQty : purchaseDetail.PurchasedQty;
                        stockForTransDate.ClosingBalance += purchaseDetail.PurchasedQty;
                        openingBalanceForOthers = stockForTransDate.ClosingBalance.Value;
                    }
                    else
                    {
                        //Add a new transaction for that transaction date
                        var newStockTrans = new StockTransaction()
                        {
                            OpeningBalance = stockTrans.ClosingBalance,
                            Inward = purchaseDetail.PurchasedQty,
                            ClosingBalance = purchaseDetail.PurchasedQty + stockTrans.ClosingBalance,
                            StockId = stockNewItem.Id,
                            AddedOn = combinedDateTime
                        };

                        openingBalanceForOthers = purchaseDetail.PurchasedQty.Value + stockTrans.ClosingBalance.Value;
                        rmsEntities.StockTransactions.Add(newStockTrans);
                    }

                    var stockLastTransDate = stockTrans.AddedOn.Value.Date;

                    DateTime fromDate, todate;

                    if (DateTime.Compare(_transcationDate, stockLastTransDate) >= 1)
                    {
                        fromDate = stockLastTransDate;
                        todate = _transcationDate;
                    }
                    else
                    {
                        fromDate = _transcationDate;
                        todate = stockLastTransDate;
                    }

                    //update all the entires that are above
                    var queryStockTrans = "select * from StockTransaction where StockId = " + stockNewItem.Id
                                            + " and date(AddedOn) > '" + fromDate.ToString("yyyy-MM-dd") +
                                            "' and date(AddedOn) <='" + todate.ToString("yyyy-MM-dd") + "' order by Addedon";

                    var sqlResult = stkTransRMSEntities.Database.SqlQuery<StockTransaction>(queryStockTrans).ToList();

                    foreach (var item in sqlResult)
                    {
                        if (item.Id == stockTrans.Id)
                        {
                            stockTrans.OpeningBalance = openingBalanceForOthers;
                            stockTrans.ClosingBalance = (openingBalanceForOthers + (item.Inward ?? 0)) - (item.Outward ?? 0);
                            openingBalanceForOthers = item.ClosingBalance.Value;
                            continue;
                        }
                        item.OpeningBalance = openingBalanceForOthers;
                        item.ClosingBalance = (openingBalanceForOthers + (item.Inward ?? 0)) - (item.Outward ?? 0);
                        openingBalanceForOthers = item.ClosingBalance.Value;
                        rmsEntities.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }
        }
        #endregion



        #region Clear Command

        override internal void Clear()
        {
            PurchaseDetailList = new ObservableCollection<PurchaseDetailExtn>();
            TotalAmount = null;
            TotalDiscountAmount = null;
            TotalDiscountPercent = null;
            SpecialDiscountAmount = null;
            SelectedCompany = null;
            InvoiceNo = null;
            SelectedPaymentId = '0';
            RMSEntitiesHelper.Instance.SelectRunningBillNo(_categoryId, true);
            RunningBillNo = _runningBillNo;
            CoolieCharges = null;
            KCoolieCharges = null;
            TransportCharges = null;
            LocalCoolieCharges = null;
            AmountPaid = 0.0M;
            IsVisible = System.Windows.Visibility.Collapsed;
        }

        #endregion

        #region GetBill Command       

        private void OnEditBill(int? billNo)
        {
            try
            {
                if (billNo == null) throw new ArgumentNullException("Please enter a bill no");
                var runningBillNo = billNo;

                var purchases = _rmsEntities.Purchases.Where(b => b.RunningBillNo == runningBillNo && b.CompanyId == _purchaseParams.CompanyId).FirstOrDefault();
                _editBillNo = purchases.BillId;
                SelectedCompany = purchases.Company;
                SelectedCompanyText = SelectedCompany.Name;
                TranscationDate = purchases.AddedOn.Value;
                SelectedPaymentId = Char.Parse(purchases.PaymentMode);
                InvoiceNo = purchases.InvoiceNo;

                //only free items without any purchase
                var freeIssueOnly = _rmsEntities.PurchaseFreeDetails.Where(p => p.BillId == _editBillNo && p.IsFreeOnly == true).FirstOrDefault();

                if (freeIssueOnly != null && freeIssueOnly.FreeQty != 0)
                {
                    var purchaseDetailExtn = new PurchaseDetailExtn()
                    {
                        ProductId = freeIssueOnly.ProductId,
                        FreeIssue = freeIssueOnly.FreeQty,
                        Qty = 0,
                        //SellingPrice = item.SellingPrice,
                        BillId = freeIssueOnly.BillId
                    };

                    _purchaseDetailsList.Add(purchaseDetailExtn);
                }

                var purchaseDetailsForBill = _rmsEntities.PurchaseDetails.Where(b => b.BillId == purchases.BillId).ToList();

                var tempTotalAmount = 0.0M;
                var i = 0;
                foreach (var item in purchaseDetailsForBill.ToList())
                {
                    //var productPrice = _productsPriceList.Where(p => p.PriceId == item.PriceId).FirstOrDefault();

                    var mySQLparam = new MySql.Data.MySqlClient.MySqlParameter("@priceId", MySql.Data.MySqlClient.MySqlDbType.Int32)
                    {
                        Value = item.PriceId
                    };


                    var productPrice = _rmsEntities.Database.SqlQuery<ProductPrice>
                                           ("GetProductPriceForPriceId(@priceId)", mySQLparam).FirstOrDefault();


                    //foreach (var logitem in _productsPriceList)
                    //{
                    //    _log.Info(logitem.PriceId + "," + logitem.ProductName + "," + logitem.ProductId + "," + item.Price);
                    //}

                    var freeIssue = _rmsEntities.PurchaseFreeDetails.Where(p => p.BillId == item.BillId
                                                                         && p.ProductId == item.ProductId).FirstOrDefault();
                    var freeIssueQty = freeIssue != null ? freeIssue.FreeQty : 0;

                    var productUnitPercase = _rmsEntities.ProductCaseMappings.FirstOrDefault(u => u.ProductId == item.ProductId);
                    int unitPerCase = productUnitPercase != null ? (int)productUnitPercase.ItemPerCase.Value : 0;
                    productPrice.UnitPerCase = unitPerCase;

                    var purchaseDetailExtn = new PurchaseDetailExtn()
                    {
                        UnitPerCase = unitPerCase,
                        PurchasePrice = item.ActualPrice,
                        Discount = item.Discount ?? 0,
                        PriceId = item.PriceId.Value,
                        ProductId = item.ProductId.Value,
                        Qty = (item.PurchasedQty  % 1 == 0 ? Math.Truncate(item.PurchasedQty.Value) : item.PurchasedQty) - freeIssueQty,
                         //item.PurchasedQty - freeIssueQty,
                        OriginalQty = item.PurchasedQty - freeIssueQty,
                        SellingPrice = productPrice.SellingPrice,
                        BillId = item.BillId,
                        CostPrice = productPrice.Price,
                        AvailableStock = productPrice.Quantity,
                        Amount = item.ActualPrice * (item.PurchasedQty - freeIssueQty),
                        ExpiryDate = DateTime.ParseExact(productPrice.ExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None),
                        PropertyReadOnly = true,
                        ItemCoolieCharges = item.ItemCoolieCharges,
                        ItemTransportCharges = item.ItemTransportCharges
                    };

                    _purchaseDetailsList.Add(purchaseDetailExtn);
                    SetPurchaseDetailExtn(productPrice, purchaseDetailExtn, i);
                    purchaseDetailExtn.PurchasePrice = item.ActualPrice;
                    purchaseDetailExtn.DiscountAmount = item.Discount ?? 0;

                    purchaseDetailExtn.FreeIssue = freeIssueQty;

                    var itemAmount = item.ActualPrice * (item.PurchasedQty - freeIssueQty);
                    tempTotalAmount += itemAmount.Value;
                }
                TotalAmount = tempTotalAmount;

                CoolieCharges = purchases.CoolieCharges;
                KCoolieCharges = purchases.KCoolieCharges;
                TransportCharges = purchases.TransportCharges;
                LocalCoolieCharges = purchases.TransportCharges;
                SpecialDiscountAmount = purchases.SpecialDiscount;
                TotalDiscountAmount = purchases.Discount;

                RunningBillNo = runningBillNo.Value;
                _isEditMode = true;

                if (_deletedItems == null)
                    _deletedItems = new List<PurchaseDetailExtn>();
                else
                    _deletedItems.Clear();
            }
            catch (Exception ex)
            {
                _log.Error("Error while getting bill details.", ex);
                Utility.ShowErrorBox(ex.Message);
            }
        }

        private void PurchaseDetailsListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var purchaseDetailExtn = e.OldItems[0] as PurchaseDetailExtn;
                if (_isEditMode && purchaseDetailExtn.ProductId != 0)
                {
                    _deletedItems.Add(purchaseDetailExtn);
                    TotalAmount -= purchaseDetailExtn.Amount;
                }
                var i = 0;
                foreach (var item in _purchaseDetailsList)
                {
                    item.SerialNo = ++i;
                }
            }
        }

        #endregion

        #region CancelPurchaseCommand
        RelayCommand<object> _cancelPurchaseCommand = null;

        public ICommand CancelPurchaseCommand
        {
            get
            {
                if (_cancelPurchaseCommand == null)
                {
                    _cancelPurchaseCommand = new RelayCommand<object>((p) => OnBillCancel(), (p) => CanBillCancel());
                }

                return _cancelPurchaseCommand;
            }
        }


        private bool CanBillCancel()
        {
            return _purchaseParams != null && _purchaseParams.Billno != null;
        }

        private void OnBillCancel()
        {
            using (var rmsEntities = new RMSEntities())
            {
                var cancelBill = rmsEntities.Purchases.FirstOrDefault(s => s.RunningBillNo == _purchaseParams.Billno && s.CompanyId == _purchaseParams.CompanyId);

                var msgResult = Utility.ShowMessageBoxWithOptions("Do you want to cancel the Purchase?");
                if (msgResult != System.Windows.MessageBoxResult.Yes) return;

                var cancelBillItems = rmsEntities.PurchaseDetails.Where(s => s.BillId == cancelBill.BillId);
                foreach (var item in cancelBillItems.ToList())
                {
                    var stockItem = rmsEntities.Stocks.FirstOrDefault(st => st.ProductId == item.ProductId && st.PriceId == item.PriceId);
                    var stockTrans = rmsEntities.StockTransactions.Where(str => str.StockId == stockItem.Id).OrderByDescending(d => d.Id).FirstOrDefault();

                    var purchaseQty = item.PurchasedQty.Value;
                    var purchaseFreeDetail = rmsEntities.PurchaseFreeDetails.FirstOrDefault(pf => pf.BillId == item.BillId && pf.ProductId == item.ProductId);
                    if (purchaseFreeDetail != null && purchaseFreeDetail.FreeQty.HasValue)
                    {
                        rmsEntities.PurchaseFreeDetails.Remove(purchaseFreeDetail);
                    }

                    //var stockTransaction = new StockTransaction();
                    ////No entry in stock transaction table use stock item details
                    //if (stockTrans == null)
                    //{
                    //    stockTransaction.SalesPurchaseCancelQty = purchaseQty * -1;
                    //    stockTransaction.ClosingBalance = stockItem.ClosingBalance - purchaseQty;
                    //    stockTransaction.StockId = stockTrans.StockId;
                    //    stockTransaction.OpeningBalance = stockTrans.ClosingBalance;
                    //}
                    //else
                    //{
                    //Due to cancel of purchase make a new entry in StockTransaction table
                    var stockTransaction = new StockTransaction()
                    {
                        SalesPurchaseCancelQty = purchaseQty * -1,
                        ClosingBalance = stockTrans.ClosingBalance - purchaseQty,
                        StockId = stockTrans.StockId,
                        OpeningBalance = stockTrans.ClosingBalance,
                        AddedOn = _transcationDate
                    };
                    //}
                    rmsEntities.StockTransactions.Add(stockTransaction);
                    stockItem.Quantity -= purchaseQty;
                }
                cancelBill.IsCancelled = true;
                cancelBill.UpdatedBy = EntitlementInformation.UserInternalId;
                rmsEntities.SaveChanges();
            }
            base.OnClose();
        }
        #endregion


        override protected bool OnClose()
        {
            if (_isEditMode)
            {
                base.OnClose();
                return true;
            }
            if (_purchaseDetailsList.Count > 0)
            {
                var options = Utility.ShowMessageBoxWithOptions("Unsaved items are available, do you want to save them?", System.Windows.MessageBoxButton.YesNo);
                if (options == System.Windows.MessageBoxResult.Yes)
                {
                    if (!Validate()) return false;
                    OnSave("PrintSave");
                    _autoResetEvent.WaitOne();
                }
            }
            base.OnClose();
            return true;
        }
    }
}
