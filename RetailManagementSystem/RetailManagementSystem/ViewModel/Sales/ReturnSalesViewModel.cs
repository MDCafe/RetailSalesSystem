using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Sales
{
    class ReturnSalesViewModel : DocumentViewModel
    {
        ObservableCollection<SaleDetailExtn> _returnSalesDetailsList;


        public ReturnSalesViewModel()
        {
            _returnSalesDetailsList = new ObservableCollection<SaleDetailExtn>();
            var cnt = RMSEntitiesHelper.Instance.RMSEntities.Products.ToList();
        }


        public ObservableCollection<SaleDetailExtn> ReturnSaleDetailList
        {
            get { return _returnSalesDetailsList; }            
        }

        public IEnumerable<Product> ProductsList
        {
            get { return RMSEntitiesHelper.Instance.RMSEntities.Products.Local; }
        }

        public void SetProductDetails(Product product, int selectedIndex)
        {
            if (product == null) return;
            var saleItem = _returnSalesDetailsList.FirstOrDefault(s => s.ProductId == product.Id);
            var selRowSaleDetailExtn = ReturnSaleDetailList[selectedIndex];
            //if (saleItem != null)
            //{
            //    Utility.ShowWarningBox("Item is already added");
            //    selRowSaleDetailExtn.ProductId = 0;
            //    return;
            //}
            SetSaleDetailExtn(product, selRowSaleDetailExtn, selectedIndex);
        }


        private void SetSaleDetailExtn(Product productPrice, SaleDetailExtn SaleDetailExtn, int selectedIndex)
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
        override public ICommand SaveCommand
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
            
        } 

        #endregion
    }
}
