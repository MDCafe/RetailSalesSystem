using log4net;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.View.Reports.Sales;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.ViewModel.Entitlements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Sales
{
    public delegate void SetFocusOnClear();
    class POSSalesEntryViewModel : SalesViewModelbase, IDisposable
    {
        protected static readonly ILog _log = LogManager.GetLogger(typeof(POSSalesEntryViewModel));

        private readonly List<ProductEmptyMapping> ProductEmptyMappingValues;
        private readonly Queue<decimal> messageQueue;
        readonly Timer qtyTimer;
        readonly Timer emptyTimer;
        private int selectedIndex;
        public event SetFocusOnClear SetFocusOnClearEvent;
        public const string BARCODE = "BarcodeNo";
        public const string EMPTYBOTTLEQTY = "EmptyBottleQty";
        public const string QUANTITY = "Quantity";
        public const string EMPTY = "Empty";
        public const string DELETE_ROW = "DeleteRow";


        #region Getters and Setters

        public Window ViewWindow { get; set; }

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

        public int SelectedIndex { get => selectedIndex; set { selectedIndex = value; RaisePropertyChanged(nameof(selectedIndex)); } }

        private DataGridCellInfo _cellInfo;
        private string SelectedColumnHeader;

        //public DataGrid salesDataGrid;

       // public POSSalesDetailExtn SalesDetailSelectedItem { get => salesDetailSelectedItem; set { salesDetailSelectedItem = value; RaisePropertyChanged(nameof(SalesDetailSelectedItem)); } }

        public DataGridCellInfo CellInfo
        {
            get { return _cellInfo; }
            set
            {
                _cellInfo = value;
                if (!_cellInfo.IsValid) return;
                RaisePropertyChanged(nameof(CellInfo));

                SelectedColumnHeader = _cellInfo.Column != null ? _cellInfo.Column.Header.ToString() : "";
                //dataGridColumn = _cellInfo.Column != null ? _cellInfo.Column : null;
                //OnPropertyChanged("CellInfo");
                //MessageBox.Show(string.Format("Column: {0}",
                //                _cellInfo.Column.DisplayIndex != null ? _cellInfo.Column.DisplayIndex.ToString() : "Index out of range!"));
            }
        }

        public Visibility IsChequeControlsVisible
        {
            get => isChequeControlsVisible;

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

        public string LoggedInUserName { get; set; }

        #endregion

        public POSSalesEntryViewModel(bool showRestrictedCustomer) : base(showRestrictedCustomer)
        {
            SaleDetailList = new ObservableCollection<POSSalesDetailExtn>();
            this.SaleDetailList.CollectionChanged += SaleDetailListCollectionChanged;
            IsChequeControlsVisible = Visibility.Hidden;
            using (var rmsEntities = new RMSEntities())
            {
                //Category Id 23 = Empty Bottles. Don't pick them up
                ProductsWithoutBarCode = rmsEntities.Products
                                        .Where(p => (p.BarcodeNo == 0 || p.BarcodeNo == null) && p.IsActive == true && p.CategoryId !=23)
                                        .OrderBy(oo => oo.CategoryId).ToList();
                RaisePropertyChanged(nameof(ProductsWithoutBarCode));
                ProductEmptyMappingValues = rmsEntities.ProductEmptyMappings.ToList();
            }

            LoggedInUserName = "Logged in User: "  + EntitlementInformation.UserName;

            messageQueue = new Queue<decimal>();
            qtyTimer = new Timer(300)
            {
                AutoReset = false
            };
            emptyTimer = new Timer(300)
            {
                AutoReset = false
            };

            qtyTimer.Elapsed += (sender, evntArgs) =>
            {
                string numbers = "";
                var enumureator = messageQueue.GetEnumerator();
                while (enumureator.MoveNext())
                {
                    numbers += enumureator.Current;
                }
                messageQueue.Clear();
                if (string.IsNullOrEmpty(numbers)) return;
                SaleDetailList[SelectedIndex].Qty = Convert.ToDecimal(numbers);
                qtyTimer.Stop();
            };

            emptyTimer.Elapsed += (sender, evntArgs) =>
            {
                string numbers = "";
                var enumureator = messageQueue.GetEnumerator();
                while (enumureator.MoveNext())
                {
                    numbers += enumureator.Current;
                }
                messageQueue.Clear();
                if (string.IsNullOrEmpty(numbers)) return;
                SaleDetailList[SelectedIndex].EmptyBottleQty = Convert.ToInt32(numbers);
                emptyTimer.Stop();
            };
        }

        private void SaleDetailListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        var saleDetailExtn = e.NewItems[0] as POSSalesDetailExtn;

                        saleDetailExtn.SubscribeToAmountChange(() =>
                        {
                            TotalAmount = SaleDetailList.Sum(a => a.Amount);
                        });

                        saleDetailExtn.PropertyChanged += (s, evnt) =>
                        {
                            switch (evnt.PropertyName)
                            {
                                case BARCODE:
                                    var productInfo = this.ProductsPriceList.Where(p => p.BarCodeNo == saleDetailExtn.BarcodeNo).FirstOrDefault();
                                    if (productInfo == null) return;

                                    SetProductDetailsOnBarcode(saleDetailExtn, productInfo);
                                    break;

                                case EMPTYBOTTLEQTY:
                                    var emptyProduct = ProductEmptyMappingValues.FirstOrDefault(p => p.ProductId == saleDetailExtn.ProductId);
                                    if (emptyProduct == null) return;
                                    saleDetailExtn.CalculateAmount();

                                    //saleDetailExtn.Amount = saleDetailExtn.Amount.HasValue
                                    //                    ?  
                                    //                    ((saleDetailExtn.SellingPrice * saleDetailExtn.Qty) +  emptyProduct.EmptyProductValue)
                                    //                    - 
                                    //                    (saleDetailExtn.EmptyBottleQty * emptyProduct.EmptyProductValue)
                                    //                    : saleDetailExtn.Amount.Value;                            
                                    break;
                                default:
                                    break;
                            }
                        };
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        TotalAmount = SaleDetailList.Sum(a => a.Amount);
                        break;
                    }
            }
            }

        private void SetProductDetailsOnBarcode(POSSalesDetailExtn saleDetailExtn, ProductPrice productInfo)
        {
            saleDetailExtn.ProductId = productInfo.ProductId;
            saleDetailExtn.ProductName = productInfo.ProductName;
            saleDetailExtn.SellingPrice = productInfo.SellingPrice;
            saleDetailExtn.CostPrice = productInfo.Price;
            saleDetailExtn.PriceId = productInfo.PriceId;
            saleDetailExtn.UnitOfMeasure = productInfo.UnitOfMeasure;
            saleDetailExtn.ExpiryDate = DateTime.ParseExact(productInfo.ExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
            saleDetailExtn.Qty = 1;

            var emptyProduct = ProductEmptyMappingValues.FirstOrDefault(p => p.ProductId == saleDetailExtn.ProductId);
            if (emptyProduct == null) return;
            saleDetailExtn.Amount = saleDetailExtn.Amount.HasValue
                                ? saleDetailExtn.Amount.Value + emptyProduct.EmptyProductValue : saleDetailExtn.Amount.Value;

            saleDetailExtn.EmptyProductValue = emptyProduct.EmptyProductValue.Value;

        }

        override internal void Clear()
        {
            //base.Clear();
            IsChequeControlsVisible = Visibility.Hidden;

            //var defaultCustomerConfigName = ConfigurationManager.AppSettings["DefaultCustomer"];
            //SelectedCustomer = CustomersList.Where(c => c.Name == defaultCustomerConfigName).FirstOrDefault();
            //HardCode the values to improve performance
            SelectedCustomerId = 1; //Cash Customer
            SelectedPaymentId = '0';
            App.Current.Dispatcher.Invoke(() =>
            {                
                SaleDetailList = new ObservableCollection<POSSalesDetailExtn>();
                this.SaleDetailList.CollectionChanged += SaleDetailListCollectionChanged;
                var posDetailExtn = new POSSalesDetailExtn();
                SaleDetailList.Add(posDetailExtn);

                SetFocusOnClearEvent?.Invoke();

                //SelectedIndex = 0;
                //SalesDetailSelectedItem = SaleDetailList[SelectedIndex];
                //_cellInfo = new DataGridCellInfo(salesDataGrid.Items[0], salesDataGrid.Columns[0]);

                ////salesDataGrid.CurrentCell = new DataGridCellInfo(salesDataGrid.Items[0], salesDataGrid.Columns[0]);
                //salesDataGrid.CurrentCell = _cellInfo;

                //salesDataGrid.ScrollIntoView(salesDataGrid.Items[SelectedIndex], salesDataGrid.Columns[0]);
            });

            TotalAmount = null;
            SelectedChqBank = null;
            SelectedChqBranch = null;
            ChqNo = null;
            ChqDate = null;
            ChqAmount = null;
        }


        #region Commands


        RelayCommand<object> _showSummaryReportCommand = null;

        public ICommand ShowSummaryReportCommand
        {
            get
            {
                if (_showSummaryReportCommand == null)
                {
                    _showSummaryReportCommand = new RelayCommand<object>((prodId) =>
                    {
                        try
                        {
                            SalesSummary salesSummary = new SalesSummary();
                            salesSummary.ShowDialog();
                        }
                        catch (Exception ex)
                        {
                            Utility.ShowErrorBox(ex.Message);
                        }
                    });
                }
                return _showSummaryReportCommand;
            }
        }


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
                            Utility.ShowErrorBox(ViewWindow, ex.Message);
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

                            if(DELETE_ROW == calValue.ToString())
                            {
                                var saleItem = SaleDetailList[selectedIndex];
                                if (saleItem == null) return;
                                SaleDetailList.Remove(saleItem);
                                return;
                            }

                            if (SaleDetailList[SelectedIndex].Qty == null) return;
                            if (string.IsNullOrEmpty(SelectedColumnHeader)) return;

                            switch (SelectedColumnHeader)
                            {
                                case QUANTITY:
                                    {
                                        messageQueue.Enqueue(Convert.ToDecimal(calValue));
                                        //var stringNumber = SaleDetailList[SelectedIndex].Qty.ToString();
                                        //var result = stringNumber + calValue;
                                        //SaleDetailList[SelectedIndex].Qty = Convert.ToDecimal(calValue);
                                        if (messageQueue.Count > 1) return;
                                        qtyTimer.Start();
                                    };
                                    break;
                                case EMPTY:
                                    {
                                        //var stringNumber = SaleDetailList[SelectedIndex].EmptyBottleQty.ToString();
                                        ////var result = stringNumber + calValue;
                                        //SaleDetailList[SelectedIndex].EmptyBottleQty = Convert.ToInt32(calValue);
                                        messageQueue.Enqueue(Convert.ToDecimal(calValue));
                                        if (messageQueue.Count > 1) return;
                                        emptyTimer.Start();
                                    }
                                    break;
                            }                        
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
                                                    .Where(s => s.UserId == EntitlementInformation.UserInternalId)
                                                    .OrderByDescending(o => o.LoginDate).FirstOrDefault();
                                if (shiftDetails != null)
                                {
                                    shiftDetails.LogoutDate = serverDate;
                                }
                                rmsEntities.Entry(shiftDetails).State = System.Data.Entity.EntityState.Modified;
                                rmsEntities.SaveChanges();
                                Dispose();
                                w.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            var msg = "Error logging off..!!";
                            _log.Error(msg, ex);
                            Utilities.Utility.ShowErrorBox(ViewWindow, msg + ex.Message);
                        }
                    });
                }
                return _logOffCommand;
            }
        }



        #region SaveCommand

        protected override bool CanExecuteSaveCommand(object parameter)
        {
            return true;// _selectedCustomer != null && _selectedCustomer.Id != 0 && SaleDetailList.Count != 0 &&
                    //SelectedCustomerId != 0;
        }

        protected override async Task OnSave(object parameter)
        {
            //short paramValue = Convert.ToInt16(parameter);
            _log.DebugFormat("Saving method called");

            if (!Validate()) return;

            _log.DebugFormat("validation passed");

            try
            {
                using (var salesSaveTask = Task.Factory.StartNew(() =>
                {                                 
                    using (var con = new EntityConnection(GetEntityConnectionString().ToString()))
                    {
                        con.Open();
                        using (var dbTransaction = con.BeginTransaction())
                        {
                            try
                            {
                                using (var rmsEntitiesSaveCtx = new RMSEntities(con,false))
                                {
                                    _log.DebugFormat("Connection opened");

                                    //Get the latest runningBill number with exclusive lock                       
                                    string sqlRunningNo = "select max(rollingno) + 1 from category cat where  cat.id = @p0  for Update";
                                    var nextRunningNo = rmsEntitiesSaveCtx.Database.SqlQuery<int>(sqlRunningNo, _categoryId).FirstOrDefault();
                                    _runningBillNo = nextRunningNo;

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
                                        if (saleDetailItem.ProductId == 0) continue;

                                        var calculatedQty = saleDetailItem.GetQty();

                                        if (!calculatedQty.HasValue || calculatedQty == 0)
                                        {
                                            Utility.ShowErrorBox("Quantity can't be null or zero");
                                            return;
                                        }
                                        
                                        var saleDetail = new SaleDetail
                                        {
                                            //Discount = saleDetailItem.Discount,
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
                                            SetStockTransaction(rmsEntitiesSaveCtx, saleDetail, actualStockEntity, serverDateTime);
                                            actualStockEntity.Quantity -= calculatedQty.Value;
                                            actualStockEntity.UpdatedBy = EntitlementInformation.UserInternalId;
                                        }
                                        //Check for Empty Bottles and save them
                                        if (saleDetailItem.EmptyBottleQty.HasValue)
                                            SaveEmptyBottles(con, saleDetailItem);
                                    }

                                    lclBillSales.TotalAmount = TotalAmount;

                                    if (_selectedPaymentId == '2')// Cheque Payment
                                    {
                                        SaveChequeDetailsAndPayments(rmsEntitiesSaveCtx, lclBillSales);
                                    }

                                    //Not Cash customer
                                    if (SelectedCustomerId != 1)
                                    {
                                        SaveOutstanding(rmsEntitiesSaveCtx, serverDateTime, lclBillSales);
                                    }

                                    rmsEntitiesSaveCtx.Sales.Add(lclBillSales);
                                    rmsEntitiesSaveCtx.SaveChanges();

                                    dbTransaction.Commit();

                                    _log.DebugFormat("Exit save :{0}", _runningBillNo);


                                    //if (paramValue == SaveOperations.SavePrint)
                                    //{
                                    //    var salesBillPrint = new UserControls.SalesBillPrint(rmsEntitiesSaveCtx);
                                    //    salesBillPrint.Print(SelectedCustomer.Name, SaleDetailList.ToList(), lclBillSales, TotalAmount.Value,
                                    //                         AmountPaid, BalanceAmount, _showRestrictedCustomer);
                                    //}

                                    Clear();
                                }
                            }
                        catch (Exception ex)
                        {
                            dbTransaction.Rollback();
                            _log.Error("Error while saving..!!", ex);
                            Utility.ShowErrorBox(ViewWindow, ex.Message + " Stack Trace : " + ex.StackTrace);
                        }
                        }
                    }
                    
                }).ContinueWith(
                    (t) =>
                    {                        
                    }))
                {
                    await salesSaveTask.ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error while saving..!!", ex);
                //if (paramValue == SaveOperations.SaveOnWindowClosing) return;
                Utility.ShowErrorBox(ViewWindow, "Error while saving..!!" + ex.Message);
            }            
        }

        private void SaveOutstanding(RMSEntities rmsEntitiesSaveCtx, DateTime serverDateTime, Sale lclBillSales)
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

        private EntityConnectionStringBuilder GetEntityConnectionString()
        {
           
            // Specify the provider name, server and database.
            string providerName = "MySql.Data.MySqlClient";

//#if DebugPOS
//            string serverName = "localhost";
//#else
            string serverName = "NES-Main";
//#endif
            string databaseName = "RMS";

            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder
            {

                // Set the properties for the data source.
                DataSource = serverName,
                InitialCatalog = databaseName,
                PersistSecurityInfo = true,
                UserID = "RMS",
                Password = "RMS!@#$"
            };

            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
            new EntityConnectionStringBuilder
            {

                //Set the provider name.
                Provider = providerName,

                // Set the provider-specific connection string.
                ProviderConnectionString = providerString,

                // Set the Metadata location.
                Metadata = @"res://*/RMSDataModel.csdl|
                        res://*/RMSDataModel.ssdl|
                        res://*/RMSDataModel.msl"
            };           

            return entityBuilder;            
        }

        private void SaveEmptyBottles(DbConnection connection, POSSalesDetailExtn pOSSalesDetailExtn)
        {
            using (var rmsEntities = new RMSEntities(connection,false))
            {                
                var emtpyProductMapping = rmsEntities.ProductEmptyMappings.FirstOrDefault(e => e.ProductId == pOSSalesDetailExtn.ProductId);
                if (emtpyProductMapping == null) return;             
                var stock = rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == emtpyProductMapping.EmptyProductId);
                if (stock != null && pOSSalesDetailExtn.EmptyBottleQty.HasValue && pOSSalesDetailExtn.EmptyBottleQty != 0)
                {
                    stock.Quantity += pOSSalesDetailExtn.EmptyBottleQty.Value;
                }
            }
            
        }
        private bool Validate()
        {
            if (SelectedPaymentId == 2)
            {
                if (!ChqAmount.HasValue || ChqAmount == 0)
                {
                    Utility.ShowErrorBox(ViewWindow, "Cheque Amount is required");
                    return false;
                }

                if (!ChqNo.HasValue || ChqAmount == 0)
                {
                    Utility.ShowErrorBox(ViewWindow, "Cheque No is required");
                    return false;
                }

            }

            if(SaleDetailList.Count == 0)
            {
                Utility.ShowErrorBox(ViewWindow, "Nothing to save");
                return false;
            }

            foreach (var item in SaleDetailList)
            {
                if(item.ProductId == 0)
                {
                    Utility.ShowErrorBox(ViewWindow, "No Product exists for the Barcode :" + item.BarcodeNo);
                    return false;
                }
            }
            return true;
        }

        public void Dispose()
        {
            qtyTimer.Close();
            emptyTimer.Close();
            qtyTimer.Dispose();
            emptyTimer.Dispose();            
        }

#endregion

#endregion
    }
}

            
            









