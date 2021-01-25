using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;


namespace RetailManagementSystem.ViewModel.Sales
{
    class POSSalesEntryViewModel : SalesEntryViewModel
    {        
        public string PaymentMethod { get; set; }        

        public IEnumerable<Product> ProductsWithoutBarCode { get; set; }

        public new char SelectedPaymentId
        {
            get { return _selectedPaymentId; }
            set
            {                
                _selectedPaymentId = value;
                RaisePropertyChanged(nameof(SelectedPaymentId));
                PaymentMethod = SelectedPaymentId.ToString();

                if (PaymentMethod == "2")
                {
                    IsChequeControlsVisible = Visibility.Visible;
                    if (BankDetailList == null)
                    {
                        using (var en = new RMSEntities())
                        {
                            BankDetailList = en.BankDetails.OrderBy(b => b.Name).ToList();
                        }
                    }
                }
                else
                {
                    IsChequeControlsVisible = Visibility.Hidden;
                }
            }

        }

        public int SelectedIndex { get; set; }
        
        public Visibility IsChequeControlsVisible { get => isChequeControlsVisible;
            
            set 
            { 
                isChequeControlsVisible = value;
                if (value == Visibility.Visible)
                {
                    NegateIsChequeControlsVisible = Visibility.Hidden;
                    ChqAmount = TotalAmount;
                }
                else
                    NegateIsChequeControlsVisible = Visibility.Visible;
            } 
        }
        public Visibility NegateIsChequeControlsVisible { get; set; }
        private Visibility isChequeControlsVisible;

        public POSSalesEntryViewModel(SalesParams salesParams) : base(salesParams)
        {
            this.SaleDetailList.CollectionChanged += SaleDetailList_CollectionChanged;
            IsChequeControlsVisible = Visibility.Hidden;
            using (var rmsEntities = new RMSEntities())
            {                
                ProductsWithoutBarCode = rmsEntities.Products.Where(p => p.BarcodeNo == "0" && p.IsActive == true).OrderBy(o => o.Name).ToList();
                RaisePropertyChanged(nameof(ProductsWithoutBarCode));
            }
        }

        private void SaleDetailList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var saleDetailExtn = e.NewItems[0] as SaleDetailExtn;

                //saleDteailExtn.PropertyChanged

                saleDetailExtn.SubscribeToAmountChange(() =>
                {
                    TotalAmount = SaleDetailList.Sum(a => a.Amount);
                    //purchaseDetailExtn.CalculateCost(purchaseDetailExtn.FreeIssue);
                });

                saleDetailExtn.PropertyChanged += (s, evnt) =>
                {
                    if (evnt.PropertyName == "BarcodeNo")
                    {
                        var productInfo = this.ProductsPriceList.Where(p => p.BarCodeNo == saleDetailExtn.BarcodeNo.ToString()).FirstOrDefault();
                        if (productInfo == null) return;

                        SetProductDetailsOnBarcode(saleDetailExtn, productInfo);
                    }
                };
            }
        }

        private static void SetProductDetailsOnBarcode(SaleDetailExtn saleDetailExtn,ProductPrice productInfo)
        {                       
            saleDetailExtn.ProductId = productInfo.ProductId;
            saleDetailExtn.ProductName = productInfo.ProductName;
            saleDetailExtn.SellingPrice = productInfo.SellingPrice;
            saleDetailExtn.CostPrice = productInfo.Price;
            saleDetailExtn.PriceId = productInfo.PriceId;
            saleDetailExtn.UnitOfMeasure = productInfo.UnitOfMeasure;
            saleDetailExtn.ExpiryDate = DateTime.ParseExact(productInfo.ExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
            saleDetailExtn.Qty = 1;

        }

        override internal void Clear()
        {
            base.Clear();
            IsChequeControlsVisible = Visibility.Hidden;
        }


        #region Commands
        

        RelayCommand<object> _addItemCommand = null;

        public ICommand AddItemCommand
        {
            get
            {
                if (_addItemCommand == null)
                {
                    _addItemCommand = new RelayCommand<object>((prodId) =>
                    {
                        try
                        {
                            if (SelectedIndex == -1) return;

                            var intProdId = Convert.ToInt32(prodId);

                            var productInfo = this.ProductsPriceList.Where(p => p.ProductId == intProdId).FirstOrDefault();
                            if (productInfo == null) return;

                            var saleDetailExtn = new SaleDetailExtn();
                            saleDetailExtn.SubscribeToAmountChange(() =>
                            {
                                TotalAmount = SaleDetailList.Sum(a => a.Amount);
                            });

                            SetProductDetailsOnBarcode(saleDetailExtn, productInfo);
                            
                            SaleDetailList.Add(saleDetailExtn);
                            //Added as it's not firing for the first time
                            TotalAmount = SaleDetailList.Sum(a => a.Amount);
                        }
                        catch (Exception ex)
                        {
                            Utilities.Utility.ShowErrorBox(ex.Message);
                        }
                    });
                }
                return _addItemCommand;
            }
        }


        RelayCommand<object> _onCalButtonClickCommand = null;

        public ICommand CalButtonClickCommand
        {
            get
            {
                if (_onCalButtonClickCommand == null)
                {
                    _onCalButtonClickCommand = new RelayCommand<object>((calValue) =>
                    {
                        try
                        {
                            if (SelectedIndex == -1) return;
                            if (SaleDetailList[SelectedIndex] == null) return;
                            if (SaleDetailList[SelectedIndex].Qty == null) return;

                            SaleDetailList[SelectedIndex].Qty += Convert.ToInt32(calValue);
                        }
                        catch (Exception ex)
                        {
                            Utilities.Utility.ShowErrorBox(ex.Message);
                        }
                    });
                }
                return _onCalButtonClickCommand;
            }
        }

        RelayCommand<Window> _logOffCommand = null;
        public ICommand LogOffCommand
        {
            get
            {
                if (_logOffCommand == null)
                {
                    _logOffCommand = new RelayCommand<Window>((w) => 
                    {
                        try
                        {
                            var serverDate = RMSEntitiesHelper.GetServerDate();
                            using (var rmsEntities = new RMSEntities())
                            {
                                var shiftDetails = rmsEntities.ShiftDetails
                                                    .Where(s => s.UserId == Entitlements.EntitlementInformation.UserInternalId)
                                                    .OrderByDescending(o => o.LoginDate).FirstOrDefault();
                                if (shiftDetails != null)
                                {
                                    shiftDetails.LogoutDate = serverDate;
                                }
                                rmsEntities.Entry(shiftDetails).State = System.Data.Entity.EntityState.Modified;
                                rmsEntities.SaveChanges();
                                w.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            var msg = "Error logging off..!!";
                            _log.Error(msg, ex);
                            Utilities.Utility.ShowErrorBox(msg + ex.Message);
                        }
                    });
                }
                return _logOffCommand;
            }
        }

        #endregion 
    }
}

            
            









