using log4net;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.ViewModel.Entitlements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Sales
{
    class POSSalesEntryViewModel : SalesViewModelbase
    {
        protected static readonly ILog _log = LogManager.GetLogger(typeof(POSSalesEntryViewModel));

        private readonly List<ProductEmptyMapping> ProductEmptyMappingValues;


        #region Getters and Setters

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

        public ObservableCollection<POSSalesDetailExtn> SaleDetailList { get; private set; }
        
        public decimal? TotalAmount { get; private set; }

        #endregion

        public POSSalesEntryViewModel(bool showRestrictedCustomer) : base(showRestrictedCustomer)
        {
            SaleDetailList = new ObservableCollection<POSSalesDetailExtn>();
            this.SaleDetailList.CollectionChanged += SaleDetailList_CollectionChanged;
            IsChequeControlsVisible = Visibility.Hidden;
            using (var rmsEntities = new RMSEntities())
            {                
                ProductsWithoutBarCode = rmsEntities.Products.Where(p => p.BarcodeNo == "0" && p.IsActive == true).OrderBy(o => o.Name).ToList();
                RaisePropertyChanged(nameof(ProductsWithoutBarCode));
                ProductEmptyMappingValues = rmsEntities.ProductEmptyMappings.ToList();
            }
        }

        private void SaleDetailList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var saleDetailExtn = e.NewItems[0] as SaleDetailExtn;

                saleDetailExtn.SubscribeToAmountChange(() =>
                {
                    TotalAmount = SaleDetailList.Sum(a => a.Amount);
                    //purchaseDetailExtn.CalculateCost(purchaseDetailExtn.FreeIssue);
                });

                saleDetailExtn.PropertyChanged += (s, evnt) =>
                {
                    switch (evnt.PropertyName)
                    {
                        case "BarcodeNo":
                            var productInfo = this.ProductsPriceList.Where(p => p.BarCodeNo == saleDetailExtn.BarcodeNo.ToString()).FirstOrDefault();
                            if (productInfo == null) return;

                            SetProductDetailsOnBarcode(saleDetailExtn, productInfo);
                            break;

                        case "EmptyBottleQty":
                            var emptyProduct = ProductEmptyMappingValues.FirstOrDefault(p => p.ProductId == saleDetailExtn.ProductId);
                            if (emptyProduct == null) return;
                            saleDetailExtn.Amount = saleDetailExtn.Amount.HasValue
                                                ? saleDetailExtn.Amount.Value - emptyProduct.EmptyProductValue : saleDetailExtn.Amount.Value;
                            break;
                        default:
                            break;
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

            var defaultCustomerConfigName = ConfigurationManager.AppSettings["DefaultCustomer"];
            SelectedCustomer = CustomersList.Where(c => c.Name == defaultCustomerConfigName).FirstOrDefault();
            SelectedCustomerId = SelectedCustomer.Id;
            SelectedPaymentId = '0';            
            App.Current.Dispatcher.Invoke(() =>
            {
                SaleDetailList.Clear();
            });
            
            TotalAmount = null;
            SelectedChqBank = null;
            SelectedChqBranch = null;
            ChqNo = null;
            ChqDate = null;
            ChqAmount = null;            
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

                            var saleDetailExtn = new POSSalesDetailExtn();
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



        #region SaveCommand

        protected override bool CanExecuteSaveCommand(object parameter)
        {
            return _selectedCustomer != null && _selectedCustomer.Id != 0 && SaleDetailList.Count != 0 &&
                    SaleDetailList[0].ProductId != 0 && SelectedCustomerId != 0;
        }

        protected override async Task OnSave(object parameter)
        {
            short paramValue = Convert.ToInt16(parameter);

            if (!Validate()) return;

            PanelLoading = true;

            try
            {
                using (var salesSaveTask = Task.Factory.StartNew(() =>
                {
                    using (var rmsEntitiesSaveCtx = new RMSEntities())
                    {
                        using (DbContextTransaction dbTransaction = rmsEntitiesSaveCtx.Database.BeginTransaction())
                        {
                            try
                            {
                                //Get the latest runningBill number with exclusive lock                       
                                string sqlRunningNo = "select max(rollingno) + 1 from category cat where  cat.id = @p0  for Update";
                                var nextRunningNo = rmsEntitiesSaveCtx.Database.SqlQuery<int>(sqlRunningNo, _categoryId).FirstOrDefault();

                                _runningBillNo = nextRunningNo;
                                //_billSales.RunningBillNo = nextRunningNo;

                                var _category = rmsEntitiesSaveCtx.Categories.FirstOrDefault(c => c.Id == _categoryId);
                                _category.RollingNo = nextRunningNo;                                

                                _log.DebugFormat("Enter save :{0}", _runningBillNo);
                                var serverDateTime = RMSEntitiesHelper.GetServerDate();

                                //Create new sales to avoid DB context Error
                                var lclBillSales = new Sale
                                {
                                    CustomerId = _selectedCustomer.Id,                                    
                                    PaymentMode = SelectedPaymentId.ToString(),
                                    AddedOn = serverDateTime,
                                    ModifiedOn = serverDateTime,
                                    RunningBillNo = _runningBillNo,
                                    UpdatedBy = EntitlementInformation.UserInternalId
                                };                                

                                foreach (var saleDetailItem in SaleDetailList)
                                {
                                    var calculatedQty = saleDetailItem.GetQty();

                                    if (saleDetailItem.ProductId == 0) continue;
                                    var saleDetail = new SaleDetail
                                    {
                                        Discount = saleDetailItem.Discount,
                                        PriceId = saleDetailItem.PriceId,
                                        ProductId = saleDetailItem.ProductId,
                                        Qty = calculatedQty,
                                        SellingPrice = saleDetailItem.SellingPrice,
                                        CostPrice = saleDetailItem.CostPrice,
                                        BillId = lclBillSales.BillId,
                                        AddedOn = serverDateTime,
                                        ModifiedOn = serverDateTime,
                                        UpdatedBy = EntitlementInformation.UserInternalId
                                    };
                                    lclBillSales.SaleDetails.Add(saleDetail);

                                    var expiryDate = saleDetailItem.ExpiryDate;
                                    
                                    var stock = GetStockDetails(rmsEntitiesSaveCtx, saleDetailItem.ProductId, saleDetailItem.PriceId, expiryDate.Value);
                                    if (stock != null)
                                    {
                                        var actualStockEntity = rmsEntitiesSaveCtx.Stocks.First(s => s.Id == stock.Id);
                                        actualStockEntity.Quantity -= calculatedQty.Value;
                                        actualStockEntity.UpdatedBy = EntitlementInformation.UserInternalId;
                                        SetStockTransaction(rmsEntitiesSaveCtx, saleDetail, actualStockEntity, serverDateTime);
                                    }
                                    //Check for Empty Bottles and save them
                                    if (saleDetailItem.EmptyBottleQty.HasValue)                                    
                                        SaveEmptyBottles(rmsEntitiesSaveCtx, saleDetailItem);
                                }

                                lclBillSales.TotalAmount = _totalAmount;                                
                               
                                if (_selectedPaymentId == '2')// Cheque Payment
                                {
                                    SaveChequeDetailsAndPayments(rmsEntitiesSaveCtx, lclBillSales);
                                }

                                //Not Cash customer
                                if (SelectedCustomerId != 1)
                                {
                                    var outstandingBalance = _totalAmount.Value - AmountPaid;
                                    if (outstandingBalance > 0 && SelectedPaymentId != 0)
                                    {
                                        lclBillSales.AmountPaid = _amountPaid;
                                        var custPaymentDetail = new PaymentDetail
                                        {
                                            AmountPaid = AmountPaid,
                                            CustomerId = SelectedCustomer.Id,
                                            Sale = lclBillSales,
                                            UpdatedBy = EntitlementInformation.UserInternalId,
                                            PaymentMode = 0,
                                            PaymentDate = serverDateTime
                                        };

                                        rmsEntitiesSaveCtx.PaymentDetails.Add
                                        (
                                            custPaymentDetail
                                        );

                                        //_billSales.PaymentDetails.Add(custPaymentDetail);
                                        var customer = rmsEntitiesSaveCtx.Customers.FirstOrDefault(c => c.Id == _selectedCustomer.Id);
                                        customer.BalanceDue = customer.BalanceDue.HasValue ? customer.BalanceDue.Value + outstandingBalance : outstandingBalance;
                                    }
                                }

                                
                                rmsEntitiesSaveCtx.Sales.Add(lclBillSales);
                                rmsEntitiesSaveCtx.SaveChanges();

                                dbTransaction.Commit();

                                _log.DebugFormat("Exit save :{0}", _runningBillNo);

                                if (paramValue == SaveOperations.SavePrint)
                                {
                                    var salesBillPrint = new UserControls.SalesBillPrint(rmsEntitiesSaveCtx);
                                    salesBillPrint.Print(SelectedCustomer.Name, SaleDetailList.ToList(), lclBillSales, TotalAmount.Value,
                                                         AmountPaid, BalanceAmount, _showRestrictedCustomer);
                                }

                                Clear();
                            }
                            catch (Exception ex)
                            {
                                dbTransaction.Rollback();
                                _log.Error("Error while saving..!!", ex);
                                Utility.ShowErrorBox(ex.StackTrace);
                            }
                        }
                    }
                }).ContinueWith(
                    (t) =>
                    {
                        PanelLoading = false;
                        //if (!_autoResetEvent.SafeWaitHandle.IsClosed)
                        //{
                        //    _autoResetEvent.Set();
                        //}
                    }))
                {
                    await salesSaveTask;
                }
            }
            catch (Exception ex)
            {
                if (paramValue == SaveOperations.SaveOnWindowClosing) return;
                Utility.ShowErrorBox("Error while saving..!!" + ex.Message);
            }
            finally
            {
                PanelLoading = false;                
            }
        }

        private void SaveEmptyBottles(RMSEntities rmsEntitiesSaveCtx,POSSalesDetailExtn pOSSalesDetailExtn)
        {
            var emtpyProductMapping = rmsEntitiesSaveCtx.ProductEmptyMappings.FirstOrDefault(e => e.ProductId == pOSSalesDetailExtn.ProductId);
            if (emtpyProductMapping == null) return;
            var stock = rmsEntitiesSaveCtx.Stocks.FirstOrDefault(s => s.ProductId == emtpyProductMapping.EmptyProductId);
            if(stock !=null)
            {
                stock.Quantity += pOSSalesDetailExtn.EmptyBottleQty.Value;
            }
        }

        private bool Validate()
        {
            if(SelectedPaymentId == 2)
            {
                if(!ChqAmount.HasValue || ChqAmount == 0)
                {
                    Utility.ShowErrorBox("Cheque Amount is required");
                    return false;
                }
                
                if (!ChqNo.HasValue || ChqAmount == 0)
                {
                    Utility.ShowErrorBox("Cheque No is required");
                    return false;
                }

            }
            return true;
        }

        #endregion

        #endregion
    }
}

            
            









