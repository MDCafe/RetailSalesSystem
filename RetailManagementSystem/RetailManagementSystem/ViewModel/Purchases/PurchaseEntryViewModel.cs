using RetailManagementSystem.Command;
using RetailManagementSystem.Interfaces;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.ViewModel.Sales;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Purchases
{
    class PurchaseEntryViewModel : CommonBusinessViewModel
    {
        private Company _selectedCompany;
        ObservableCollection<PurchaseDetailExtn> _purchaseDetailsList;        

        public PurchaseEntryViewModel(bool showRestrictedCustomer) : base(showRestrictedCustomer)
        {
            Title = "Purchase Entry ";
            var count = RMSEntitiesHelper.Instance.RMSEntities.Companies.ToList();
            _purchaseDetailsList = new ObservableCollection<PurchaseDetailExtn>();            
        }

        public void Notify(int runningNo)
        {
            throw new NotImplementedException();
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
                return RMSEntitiesHelper.Instance.RMSEntities.Companies.Local.Where(c => c.CategoryTypeId == Constants.COMPANIES_OTHERS);
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
                purchaseDetailExtn.CostPrice = productPrice.Price;
                purchaseDetailExtn.PriceId = productPrice.PriceId;
                purchaseDetailExtn.AvailableStock = productPrice.Quantity;                

                purchaseDetailExtn.PropertyChanged += (sender, e) =>
                {
                    var prop = e.PropertyName;

                    if(prop == Constants.FREE_ISSUE)
                    {
                        if (purchaseDetailExtn.Qty.HasValue && purchaseDetailExtn.FreeIssue.HasValue)
                        {
                            purchaseDetailExtn.CostPrice = TotalAmount.Value / (purchaseDetailExtn.Qty.Value + purchaseDetailExtn.FreeIssue.Value);
                        }
                    }

                    if (prop == Constants.AMOUNT)
                    {
                        TotalAmount = _purchaseDetailsList.Sum(a => a.Amount);
                        return;
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
        }

        private void Clear()
        {
            _purchaseDetailsList = new ObservableCollection<PurchaseDetailExtn>();
        }

        #endregion
    }
}
