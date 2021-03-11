using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Base
{
    class SalesViewModelbase : CommonBusinessViewModel
    {
        protected bool _showRestrictedCustomer;
        protected Customer _selectedCustomer;
        List<Customer> _customerList;

        public int SelectedCustomerId { get; set; }

        public Customer SelectedCustomer
        {
            get
            {
                //if(_selectedCustomer == null)
                //{
                //    _selectedCustomer = _rmsEntities.Customers.FirstOrDefault();
                //    RaisePropertyChanged("SelectedCustomer");
                //   return _selectedCustomer;
                //}
                return _selectedCustomer;
            }
            set
            {
                if (_selectedCustomer == value) return;

                _selectedCustomer = value;
                //CheckIfWithinCreditLimimt();
                RaisePropertyChanged(nameof(SelectedCustomer));
            }
        }

        public SalesViewModelbase(bool showRestrictedCustomer)
        {
            _showRestrictedCustomer = showRestrictedCustomer;
            if (_showRestrictedCustomer)
                _categoryId = Constants.CUSTOMERS_OTHERS;
            else
                _categoryId = Constants.CUSTOMERS_HOTEL;
        }

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                using (var rmsEntities = new RMSEntities())
                {
                    var defaultCustomerConfigName = ConfigurationManager.AppSettings["DefaultCustomer"];
                    //_customerList = new List<Customer>();
                    var cnt = rmsEntities.Customers.ToList().Count;
                    _customerList = new List<Customer>(cnt);
                    
                    if (_showRestrictedCustomer)
                    {
                        foreach (var item in rmsEntities.Customers)
                        {
                            if (item.CustomerTypeId == Constants.CUSTOMERS_OTHERS)
                            {
                                _customerList.Add(item);
                            }
                        }
                    }
                    //customerList =  _rmsEntities.Customers.Local.Where(c => c.CustomerTypeId == Constants.CUSTOMERS_OTHERS);
                    else
                    {
                        foreach (var item in rmsEntities.Customers)
                        {
                            if (item.CustomerTypeId == Constants.CUSTOMERS_HOTEL)
                            {
                                _customerList.Add(item);
                            }
                        }
                    }
                    //customerList  = _rmsEntities.Customers.Local.Where(c => c.CustomerTypeId != Constants.CUSTOMERS_OTHERS);

                    var defaultCustomerByConfig = _customerList.FirstOrDefault(c => c.Name.ToUpper() == defaultCustomerConfigName.ToUpper());
                    if (defaultCustomerByConfig != null)
                    {
                        DefaultCustomer = defaultCustomerByConfig;
                    }
                    return _customerList.OrderBy(a => a.Name);
                }
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
                    _saveCommand = new RelayCommand<object>(async (p) => await OnSave(p).ConfigureAwait(false),CanExecuteSaveCommand);
                }

                return _saveCommand;
            }
        }

        protected virtual bool CanExecuteSaveCommand(object parameter)
        {
            return true;
        }

        protected virtual async Task OnSave(object parameter)
        {
            
        }
        #endregion

        protected static void SetStockTransaction(RMSEntities rmsEntities, SaleDetail saleDetail, Stock stockNewItem, DateTime combinedDateTime)
        {
            var stockTrans = rmsEntities.StockTransactions.Where(s => s.StockId == stockNewItem.Id).OrderByDescending(s => s.Id).FirstOrDefault();
            var stockAdjustCheck = RMSEntitiesHelper.CheckStockAdjustment(rmsEntities, stockNewItem.Id);

            //stock transaction not available for this product. Add them 
            if (stockTrans == null)
            {
                var firstStockTrans = new StockTransaction()
                {
                    OpeningBalance = stockNewItem.Quantity, //Opening balance will be the one from stock table 
                    Outward = saleDetail.Qty,
                    ClosingBalance = stockNewItem.Quantity - saleDetail.Qty,
                    StockId = stockNewItem.Id,
                    AddedOn = combinedDateTime
                };
                stockNewItem.StockTransactions.Add(firstStockTrans);
            }
            //stock transaction available. Check if it is for the current date else get the latest date and mark the opening balance
            else
            {
                var systemDBDate = RMSEntitiesHelper.Instance.GetSystemDBDate();
                var dateDiff = DateTime.Compare(stockTrans.AddedOn.Value.Date, systemDBDate.Date);
                if (dateDiff == 0 && stockAdjustCheck != null && !stockAdjustCheck.StockTransId.HasValue)
                {
                    stockTrans.Outward = saleDetail.Qty.Value + (stockTrans.Outward ?? 0);
                    stockTrans.ClosingBalance -= saleDetail.Qty;
                }
                else
                {
                    var newStockTrans = new StockTransaction()
                    {
                        OpeningBalance = stockTrans.ClosingBalance,
                        Outward = saleDetail.Qty,
                        ClosingBalance = stockTrans.ClosingBalance - saleDetail.Qty,
                        StockId = stockNewItem.Id,
                        AddedOn = combinedDateTime
                    };
                    rmsEntities.StockTransactions.Add(newStockTrans);
                }
            }
        }

        protected static Stock GetStockDetails(RMSEntities rmsEntities, int productId, int priceId, DateTime dateToCompare)
        {
            var query = "select * from stocks where ProductId = " + productId +
                        " and PriceId =" + priceId +
                        " and date(ExpiryDate) = '" + dateToCompare.ToString("yyyy-MM-dd") + "'";

            return rmsEntities.Database.SqlQuery<Stock>(query).FirstOrDefault();
        }

        protected void SaveChequeDetailsAndPayments(RMSEntities rmsEntities, Sale saleBill)
        {
            var chqPayment = new PaymentDetail
            {
                AmountPaid = ChqAmount,
                CustomerId = SelectedCustomer.Id,
                PaymentDate = ChqDate,
                PaymentMode = 2,
                Sale = saleBill
            };

            rmsEntities.ChequePaymentDetails.Add(
                new ChequePaymentDetail()
                {
                    ChequeNo = ChqNo,
                    ChequeDate = ChqDate,
                    BankId = SelectedChqBank.Value,
                    BankBranchId = SelectedChqBranch.Value,
                    IsChequeRealised = false,
                    Amount = ChqAmount,
                    PaymentDetail = chqPayment
                });


            if (AmountPaid > 0)
            {
                saleBill.AmountPaid = AmountPaid;
                rmsEntities.PaymentDetails.Add
                (
                    new PaymentDetail
                    {
                        AmountPaid = AmountPaid,
                        CustomerId = SelectedCustomer.Id,
                        PaymentDate = _transcationDate,
                        PaymentMode = 0,
                        Sale = saleBill
                    }
                );
            }

            var outstandingBalance = saleBill.TotalAmount - ChqAmount.Value - AmountPaid;
            var customer = rmsEntities.Customers.FirstOrDefault(c => c.Id == _selectedCustomer.Id);
            customer.BalanceDue = customer.BalanceDue.HasValue ? customer.BalanceDue.Value + outstandingBalance : outstandingBalance;
        }   
    }
}
