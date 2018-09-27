using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System;
using System.Globalization;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Reports.Purhcases;

namespace RetailManagementSystem.ViewModel.Purchases
{
    class ReturnsViewModel : DocumentViewModel
    {
        RMSEntities _rmsEntities;
        ObservableCollection<ReturnPurchaseDetailExtn> _returnPurchaseDetailsList;
        IEnumerable<PriceDetail> _returnPriceList;                
        decimal? _totalAmount;
        Company _selectedCompany;
        Purchase _selectedPurchaseBillNo;
        IEnumerable<Purchase> _billList;
        int _categoryId;
        ObservableCollection<ProductPrice> _productsPriceList;        
        public event CommonBusinessViewModel.INotifierCollectionChanged NotifierCollectionChangedEvent;
        bool _showRestrictedCompanies;

        public ObservableCollection<ProductPrice> ProductsPriceList
        {
            get { return _productsPriceList; }
            set
            {
                _productsPriceList = value;
                RaisePropertyChanged("ProductsPriceList");
            }
        }

        public bool ReadOnly { get; set; }

        public bool IsGridEnabled { get; set; }

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
            get { return _rmsEntities.Companies.Local.Where(c => c.CategoryTypeId == _categoryId).OrderBy(o => o.Name);  }
        }

        public IEnumerable<Purchase> BillList
        {
            get { return _billList; }
            set
            {
                _billList = value;
                //NotifyPropertyChanged(() => this._selectedCustomer);
                RaisePropertyChanged("BillList");
                if (_billList != null && _billList.Count() != 0)
                    ReadOnly = true;
                else
                    ReadOnly = false;
            }
        }

        public void SetProductDetails(ProductPrice productPrice, int selectedIndex)
        {
            if (productPrice == null) return;
            var returnItem = _returnPurchaseDetailsList[selectedIndex];//.Where(s => s.ProductId == productPrice.ProductId);
            //var returnItem = returnItemList.ElementAt(selectedIndex);

            if (returnItem != null)
            {
                returnItem.CostPrice = productPrice.Price;
                returnItem.PriceId = productPrice.PriceId;
                returnItem.ReturnAmount = returnItem.ReturnQty * productPrice.Price;
                returnItem.AvailableStock = productPrice.Quantity;
                returnItem.ReturnPrice = productPrice.Price;
                returnItem.ExpiryDate = DateTime.ParseExact(productPrice.ExpiryDate,"dd/MM/yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None);
            }

            returnItem.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case "ReturnAmount":
                        {
                            TotalAmount = _returnPurchaseDetailsList.Sum(a => a.ReturnAmount);
                            break;
                        }

                    case Constants.RETURN_QTY:
                    case "ReturnPrice":
                        {
                            TotalAmount = _returnPurchaseDetailsList.Sum(a => a.ReturnAmount);
                            returnItem.ReturnAmount = returnItem.ReturnQty * returnItem.ReturnPrice;
                            break;
                        }
                }
            };
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

        public Purchase SelectedPurchaseBillNo
        {
            get
            {
                return _selectedPurchaseBillNo;
            }

            set
            {
                _selectedPurchaseBillNo = value;
                RaisePropertyChanged("SelectedPurchase");
            }
        }

        public IEnumerable<CodeMaster> ReturnReasons
        { get { return _rmsEntities.CodeMasters.Local.Where(r => r.Code == "RTN"); } }

        public ReturnsViewModel(bool showRestrictedCustomers)
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
            var cnt3 = _rmsEntities.Purchases.ToList();
            _showRestrictedCompanies = showRestrictedCustomers;

            if (_showRestrictedCompanies)
                _categoryId = Constants.COMPANIES_OTHERS;
            else
                _categoryId = Constants.COMPANIES_MAIN;

            this.Title = "Returns Entry";
            ReadOnly = false;
            IsGridEnabled = false;
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
                if (SelectedCompany ==null || SelectedCompany.Id == 0)
                {
                    Utility.ShowErrorBox("Select a supplier to select products");
                    return null;
                }
                return _rmsEntities.Products.Local.Where(p => p.CompanyId == SelectedCompany.Id);
            }
        }
      
        //public void SetProductPriceDetails(int productId, int selectedIndex)
        //{
        //    var product = _rmsEntities.Products.Where(p => p.Id == productId).FirstOrDefault();
        //    _returnPurchaseDetailsList[selectedIndex].ProductName = product.Name;
        //    //var selectedProduct = _returnSalesDetailsList.Where(s => s.ProductId == product.Id).FirstOrDefault();
        //    //selectedProduct.ProductName = product.Name;
        //}

        //public void SetPriceDetails(int priceId,int selectedIndex)
        //{
        //    var returnPrice = _rmsEntities.PriceDetails.Where(pr => pr.PriceId == priceId).FirstOrDefault();
        //    var returnStock = _rmsEntities.Stocks.Where(pr => pr.PriceId == priceId).FirstOrDefault();
        //    var returnItem = _returnPurchaseDetailsList[selectedIndex];
        //    returnItem.ReturnPrice = returnPrice.Price;
        //    returnItem.Amount = returnItem.ReturnPrice * returnItem.ReturnQty;
        //    //returnItem.ExpiryDate = 
        //    TotalAmount = _returnPurchaseDetailsList.Sum(p => p.Amount);
        //    returnItem.PropertyChanged += (s, e) =>
        //    {
        //        if(e.PropertyName == Constants.RETURN_QTY)
        //        {
        //            returnItem.Amount = returnItem.ReturnPrice * returnItem.ReturnQty;
        //            TotalAmount = _returnPurchaseDetailsList.Sum(p => p.Amount);
        //        }
        //    };
        //}

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
            foreach (var item in _returnPurchaseDetailsList)
            {
                if(item.SelectedReturnReason == null)
                {
                    Utility.ShowErrorBox("Choose a return reason");
                    return;
                }

                if(item.Qty <=0)
                {
                    Utility.ShowErrorBox("Quantity can't be 0 or negative number");
                    return;
                }
                 
                if (item.ExpiryDate == null)
                {
                    Utility.ShowErrorBox("Expiry date can't empty");
                    return;
                }

                var itemSelected = item.Selected;
                if(!itemSelected && _selectedPurchaseBillNo == null)
                {
                    Utility.ShowErrorBox("Item as to be marked for return or Bill No has to be choosen");
                    return;
                }

                //New item
                if (item.AddedOn == null)
                {
                    _rmsEntities.PurchaseReturns.Add(new PurchaseReturn()
                    {
                        PriceId = item.PriceId,
                        ProductId = item.ProductId,
                        Quantity = item.ReturnQty,
                        BillId = SelectedPurchaseBillNo == null? 0 : SelectedPurchaseBillNo.BillId,
                        ReturnReasonCode = item.SelectedReturnReason.Id,
                        MarkedForReturn = item.Selected,
                        comments = item.Comments,
                        ExpiryDate = item.ExpiryDate,
                        ReturnPrice  = item.ReturnPrice
                    });

                    var itemDate = item.ExpiryDate.Value;
                    var stock = _rmsEntities.Stocks.Where(s => s.PriceId == item.PriceId && s.ProductId == item.ProductId
                                                            && s.ExpiryDate.Year == itemDate.Year
                                                            && s.ExpiryDate.Month == itemDate.Month
                                                            && s.ExpiryDate.Day == itemDate.Day).FirstOrDefault();
                    if (stock == null)
                    {
                        Utility.ShowErrorBox("Expiry date is not found in stock");
                        return;
                    }

                    stock.Quantity -= item.ReturnQty;
                }
                //Item is unselected for returning
                if(!item.Selected && item.AddedOn !=null)
                {
                    if (_selectedPurchaseBillNo == null)
                    {
                        Utility.ShowErrorBox("Choose a bill for which retun needs to be marked");
                        return;
                    }
                    var purchaseRtn = _rmsEntities.PurchaseReturns.FirstOrDefault(r => r.ProductId == item.ProductId && r.PriceId == item.PriceId 
                                                                && r.Quantity == item.ReturnQty && r.BillId ==0 && r.ReturnPrice == item.ReturnPrice);

                    if (purchaseRtn == null) continue;
                    purchaseRtn.BillId = _selectedPurchaseBillNo.BillId;
                    purchaseRtn.MarkedForReturn = false;
                    _rmsEntities.Entry<PurchaseReturn>(purchaseRtn).State = System.Data.Entity.EntityState.Modified;
                }
            }

            //if (_selectedPurchase !=null && _selectedPurchase.BillId !=0)
            //{
            //    var purchase = _rmsEntities.Purchases.FirstOrDefault(p => p.BillId == _selectedPurchase.BillId);
            //    if (purchase != null)
            //    {
            //        purchase.TotalBillAmount -= _returnPurchaseDetailsList.Where(r => r.Selected == false).Sum(a => a.Amount);
            //    }
            //}

            _rmsEntities.SaveChanges();

            if(parameter != null && parameter.ToString() =="Print" && SelectedPurchaseBillNo != null)
            {
                PurchaseSummaryViewModel psummVM = new PurchaseSummaryViewModel(_showRestrictedCompanies, SelectedPurchaseBillNo.RunningBillNo);
                psummVM.RunningBillNo = SelectedPurchaseBillNo.RunningBillNo;
                psummVM.PrintCommand.Execute(null);
            }
            Clear();
        }
        #endregion

        #region Clear Command

        internal override void Clear()
        {
            _returnPurchaseDetailsList = new ObservableCollection<ReturnPurchaseDetailExtn>();
            RaisePropertyChanged("ReturnPurchaseDetailList");            
            TotalAmount = null;
            SelectedCompany = null;
            BillList = null;
            IsGridEnabled = false;
        }

        #endregion

        #region GetBillsItemsCommand
        RelayCommand<object> _getBillsItemsCommand = null;
        public ICommand GetBillsItemsCommand
        {
            get
            {
                if (_getBillsItemsCommand == null)
                {
                    _getBillsItemsCommand = new RelayCommand<object>((p) => OnGetBillsItems(), (p) => CanGetBilllsItems());
                }

                return _getBillsItemsCommand;
            }
        }

        private bool CanGetBilllsItems()
        {
            return _selectedCompany != null && SelectedCompany.Id != 0;
        }

        private void OnGetBillsItems()
        {
            if (SelectedCompany == null) return;

            IsGridEnabled = true;
            ProductsPriceList = RMSEntitiesHelper.Instance.GetProductPriceList(_selectedCompany.Id);
            NotifierCollectionChangedEvent?.Invoke();

            _returnPurchaseDetailsList.Clear();

             BillList = _rmsEntities.Purchases.Local.Where(s => s.CompanyId == SelectedCompany.Id);

            var mysqlParam = new MySql.Data.MySqlClient.MySqlParameter("@filterCompanyId", MySql.Data.MySqlClient.MySqlDbType.Int32);
            mysqlParam.Value = SelectedCompany.Id;

            var purchaseReturn = _rmsEntities.Database.SqlQuery<PurchaseReturn>
                                   ("GetPurchaseReturnForCompany(@filterCompanyId)", mysqlParam).ToList();
            foreach (var item in purchaseReturn)
            {
                var itemPriceDetails = _rmsEntities.PriceDetails.FirstOrDefault(p => p.ProductId == item.ProductId && p.PriceId == item.PriceId);
                decimal itemPrice = 0;

                var itemProduct = _rmsEntities.Products.FirstOrDefault(p => p.Id == item.ProductId);
                var itemProductName = "";

                if (itemPriceDetails != null)
                {
                    itemPrice = itemPriceDetails.Price; 
                }

                if(itemProduct !=null)
                {
                    itemProductName = itemProduct.Name;
                }

                var rtnReason = _rmsEntities.CodeMasters.FirstOrDefault(s => s.Id == item.ReturnReasonCode);
                var itemPriceValue = item.ReturnPrice.HasValue ? item.ReturnPrice.Value : itemPrice;
                var returnItem = new ReturnPurchaseDetailExtn()
                {
                    ProductId = item.ProductId,
                    PriceId = item.PriceId,
                    SelectedReturnReason = rtnReason,
                    ReturnQty = item.Quantity,
                    ReturnPrice = itemPriceValue,
                    ReturnAmount = item.Quantity * itemPriceValue,
                    ProductName = itemProductName,
                    Selected = item.MarkedForReturn.HasValue ? item.MarkedForReturn.Value : false,
                    Comments = item.comments,
                    ExpiryDate = item.ExpiryDate,
                    AddedOn = item.CreatedOn
                };

                _returnPurchaseDetailsList.Add(returnItem);

                returnItem.PropertyChanged += (sender, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case "ReturnAmount":
                            {
                                TotalAmount = _returnPurchaseDetailsList.Sum(a => a.ReturnAmount);
                                break;
                            }

                        case Constants.RETURN_QTY:
                        case "ReturnPrice":
                            {
                                TotalAmount = _returnPurchaseDetailsList.Sum(a => a.ReturnAmount);
                                returnItem.ReturnAmount = returnItem.ReturnQty * returnItem.ReturnPrice;
                                break;
                            }
                    }
                };
            }
            TotalAmount = _returnPurchaseDetailsList.Sum(a => a.ReturnAmount);
        }

        #endregion
    }
}
