using log4net;
using RetailManagementSystem.Command;
using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Accounts
{
    class CustomerBillPaymentsViewModel : DocumentViewModel
    {
        private readonly bool _showRestrictedCustomer;
        private readonly int _categoryId;
        private static readonly ILog _log = LogManager.GetLogger(typeof(CustomerBillPaymentsViewModel));

        public ObservableCollection<CustomerPaymentDetails> CustomerPaymentDetailsList { get; set; }

        private IEnumerable<BankDetail> bankList;
        private int? selectedBankId;

        public Customer SelectedCustomer { get; set; }
        public decimal? AllocationAmount { get; set; }
        //public decimal? TotalPendingAmount { get; set; }
        public decimal? OldPendingAmount { get; set; }
        public decimal? ChequeAllocationAmount { get; set; }
        public int? ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public DateTime PaymentDate { get; set; }

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                using (var rmsEntities = new RMSEntities())
                {
                    return rmsEntities.Customers.ToList().Where(c => c.CustomerTypeId == _categoryId).OrderBy(a => a.Name);
                }
            }
            private set
            {

            }
        }

        public IEnumerable<CodeMaster> PaymentModes { get; set; }

        public IEnumerable<BankDetail> BankList
        {
            get => bankList;
            set
            {
                bankList = value;


            }
        }
        public IEnumerable<BankBranchDetail> BankBranchList { get; private set; }

        IEnumerable<BankBranchDetail> BankBranchListInternal { get; set; }

        public int? SelectedBankId
        {
            get => selectedBankId;
            set 
            { 
                selectedBankId = value;
                BankBranchList = BankBranchListInternal.Where(b => b.BankId == selectedBankId);
            }
        }
        public int? SelectedBankBranchId { get; set; }

        public CodeMaster SelectedPaymentMode { get; set; }
        public decimal TotalBillSumBalanceAmount { get; set; }

        public CustomerBillPaymentsViewModel(bool showRestrictedCustomer)
        {
            _showRestrictedCustomer = showRestrictedCustomer;
            if (_showRestrictedCustomer)
                _categoryId = Constants.CUSTOMERS_OTHERS;
            else
                _categoryId = Constants.CUSTOMERS_HOTEL;

            Title = "Customer Bill Payment";

            CustomerPaymentDetailsList = new ObservableCollection<CustomerPaymentDetails>();

            using (var rmsEntities = new RMSEntities())
            {
                var cnt = rmsEntities.CodeMasters.Local.Count;
                PaymentModes = rmsEntities.CodeMasters.Where(c => c.Code == "PMODE" && c.Id != 8).ToList();
                BankList = rmsEntities.BankDetails.ToList();
                BankBranchListInternal = rmsEntities.BankBranchDetails.ToList();
            }
            ChequeDate = DateTime.Now;
            PaymentDate = DateTime.Now;
        }

        #region GetBillsCommand
        RelayCommand<object> _getBillsCommand = null;
        public ICommand GetBillsCommand
        {
            get
            {
                if (_getBillsCommand == null)
                {
                    _getBillsCommand = new RelayCommand<object>((p) =>
                    {
                        CustomerPaymentDetailsList.Clear();
                        using (var rmsEntities = new RMSEntities())
                        {
                            var mySQLparam = new MySql.Data.MySqlClient.MySqlParameter("@customerId", MySql.Data.MySqlClient.MySqlDbType.Int32)
                            {
                                Value = SelectedCustomer.Id
                            };
                            var custPayDetails = rmsEntities.Database.SqlQuery<CustomerPaymentDetails>
                                                                                ("CALL GetCustomerPaymentDetails(@customerId)", mySQLparam);

                            //TotalPendingAmount = SelectedCustomer.BalanceDue;

                            var i = 0;
                            TotalBillSumBalanceAmount = 0;
                            foreach (var item in custPayDetails)
                            {
                                item.SerialNo = ++i;
                                TotalBillSumBalanceAmount += item.BalanceAmount;
                                CustomerPaymentDetailsList.Add(item);
                                item.PropertyChanged += (s, e) =>
                                {
                                    switch (e.PropertyName)
                                    {
                                        case "AmountPaid":
                                        case "CurrentAmountPaid":
                                            item.BalanceAmount = item.TotalAmount - (item.AmountPaid + item.CurrentAmountPaid);
                                            break;
                                    }
                                };
                            }

                            OldPendingAmount = SelectedCustomer.OldBalanceDue;
                            TotalBillSumBalanceAmount += OldPendingAmount ?? 0;
                        }
                    }, (p) =>
                    {
                        return SelectedCustomer != null;
                    });
                }

                return _getBillsCommand;
            }
        }

        #endregion

        #region SaveCommand
        RelayCommand<object> _saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand<object>((p) => OnSave(), (p) => CanSave());
                }

                return _saveCommand;
            }
        }

        private bool CanSave()
        {
            return SelectedCustomer != null;
        }

        protected void OnSave()
        {
            try
            {
                using (var rmsEntities = new RMSEntities())
                {
                    foreach (var item in CustomerPaymentDetailsList)
                    {
                        if (item.CurrentAmountPaid == 0) continue;

                        if (!ValidateCustomerPaymentDetails()) return;

                        //First time payment for the bill
                        if (item.AmountPaid == 0)
                        {
                            var paymentDetails = rmsEntities.PaymentDetails.FirstOrDefault(s => s.BillId == item.BillId);
                            if (paymentDetails != null)
                            {
                                paymentDetails.AmountPaid = item.CurrentAmountPaid;
                                var payment = item.PaymentMode;
                                paymentDetails.PaymentMode = payment.Id;
                                paymentDetails.PaymentDate = PaymentDate;
                                paymentDetails.UpdatedBy = Entitlements.EntitlementInformation.UserInternalId;
                                if (payment.Description == "Cheque")
                                {
                                    AddChequePayment(item, paymentDetails);
                                }
                            }
                            continue;
                        }
                        //Partial amount already paid. Add new payment for the same bill
                        var newPaymentDetail = new PaymentDetail()
                        {
                            AmountPaid = item.CurrentAmountPaid,
                            BillId = item.BillId,
                            CustomerId = SelectedCustomer.Id,
                            PaymentMode = item.PaymentMode.Id,
                            PaymentDate = PaymentDate,
                            UpdatedBy = Entitlements.EntitlementInformation.UserInternalId
                        };
                        if (item.PaymentMode.Description == "Cheque")
                        {
                            AddChequePayment(item, newPaymentDetail);
                        }

                        rmsEntities.PaymentDetails.Add(newPaymentDetail);
                    }
                    //Reduce the customer balance
                    var customer = rmsEntities.Customers.FirstOrDefault(c => c.Id == SelectedCustomer.Id);
                    var totalAmountPaid = CustomerPaymentDetailsList.Sum(c => c.CurrentAmountPaid);
                    customer.BalanceDue -= totalAmountPaid;
                    rmsEntities.SaveChanges();
                    Clear();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error on saving payments for customer:" + SelectedCustomer.Name, ex);
                Utility.ShowErrorBox(ex.Message);
            }
        }

        private bool ValidateCustomerPaymentDetails()
        {
            foreach (var item in CustomerPaymentDetailsList)
            {
                if (item.BalanceAmount < 0)
                {
                    Utility.ShowErrorBox("Balance Amount can't be less than zero");
                    return false;
                }
                if (item.CurrentAmountPaid > 0 && item.PaymentMode == null)
                {
                    Utility.ShowErrorBox("Choose payment mode as Cash/Cheque");
                    return false;
                }
                if (item.PaymentMode != null && item.PaymentMode.Description == "Cheque")
                {
                    if (!item.ChequeNo.HasValue)
                    {
                        Utility.ShowErrorBox("Cheque No can't be blank");
                        return false;
                    }
                    if (!item.ChequeDate.HasValue)
                    {
                        Utility.ShowErrorBox("Cheque date can't be blank");
                        return false;
                    }
                }
            }
            return true;
        }

        private void AddChequePayment(CustomerPaymentDetails item, PaymentDetail paymentDetails)
        {
            var chqPaymentDetails =
                                    new ChequePaymentDetail()
                                    {
                                        ChequeDate = item.ChequeDate,
                                        ChequeNo = item.ChequeNo,
                                        IsChequeRealised = false,
                                        PaymentDate = PaymentDate,
                                        Amount = item.CurrentAmountPaid,
                                        UpdatedBy = Entitlements.EntitlementInformation.UserInternalId,
                                        BankId = selectedBankId,
                                        BankBranchId = SelectedBankBranchId
                                    };
            paymentDetails.ChequePaymentDetails.Add(chqPaymentDetails);
        }

        #endregion

        #region AllocateCommand
        RelayCommand<object> _allocateCommand = null;
        public ICommand AllocateCommand
        {
            get
            {
                if (_allocateCommand == null)
                {
                    _allocateCommand = new RelayCommand<object>((p) => OnAllocate(), (p) => CanAllocate(p));
                }

                return _allocateCommand;
            }
        }

        private bool CanAllocate(object p)
        {
            if (!AllocationAmount.HasValue) return false;
            var sender = p as System.Windows.DependencyObject;
            return IsValid(sender);
        }

        protected void OnAllocate()
        {
            try
            {
                var payMode = PaymentModes.FirstOrDefault(p => p.Description == "Cash");
                var decreasingAllocationAmount = AllocationAmount.Value;
                foreach (var item in CustomerPaymentDetailsList)
                {
                    if (decreasingAllocationAmount <= 0) return;
                    if ((item.TotalAmount - item.AmountPaid) != 0)
                    {
                        var balanceAmtToBePaid = item.TotalAmount - item.AmountPaid;
                        item.PaymentMode = payMode;
                        if (balanceAmtToBePaid < decreasingAllocationAmount)
                        {
                            item.CurrentAmountPaid = balanceAmtToBePaid;
                            decreasingAllocationAmount -= balanceAmtToBePaid;
                        }
                        else
                        {
                            item.CurrentAmountPaid = decreasingAllocationAmount;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error on allocation:" + SelectedCustomer.Name, ex);
                Utility.ShowErrorBox(ex.Message);
            }
        }

        #endregion

        #region AllocateChequeCommand
        RelayCommand<object> _allocateChequeCommand = null;
        public ICommand AllocateChequeCommand
        {
            get
            {
                if (_allocateChequeCommand == null)
                {
                    _allocateChequeCommand = new RelayCommand<object>((p) => OnAllocateCheque(), (p) => CanAllocateCheque(p));
                }

                return _allocateChequeCommand;
            }
        }

        private bool CanAllocateCheque(object p)
        {
            if (!ChequeAllocationAmount.HasValue) return false;
            var sender = p as System.Windows.DependencyObject;
            return IsValid(sender);
        }

        protected void OnAllocateCheque()
        {
            try
            {
                var payMode = PaymentModes.FirstOrDefault(p => p.Description == "Cheque");
                var decreasingAllocationAmount = ChequeAllocationAmount.Value;
                foreach (var item in CustomerPaymentDetailsList)
                {
                    if (!item.IsSelected || decreasingAllocationAmount <= 0) continue;
                    if ((item.TotalAmount - item.AmountPaid) != 0)
                    {
                        item.ChequeNo = ChequeNo;
                        item.ChequeDate = ChequeDate;
                        item.PaymentMode = payMode;
                        var balanceAmtToBePaid = item.TotalAmount - item.AmountPaid;
                        if (balanceAmtToBePaid < decreasingAllocationAmount)
                        {
                            item.CurrentAmountPaid = balanceAmtToBePaid;
                            decreasingAllocationAmount -= balanceAmtToBePaid;
                        }
                        else
                        {
                            item.CurrentAmountPaid = decreasingAllocationAmount;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error on allocation:" + SelectedCustomer.Name, ex);
                Utility.ShowErrorBox(ex.Message);
            }
        }
        #endregion

        #region DirectPaymentCommand
        RelayCommand<object> _directPaymentCommand = null;
        
        public ICommand DirectPaymentCommand
        {
            get
            {
                if (_directPaymentCommand == null)
                {
                    _directPaymentCommand = new RelayCommand<object>((p) => OnDirectPayment(), (p) => CanDirectPayment(p));
                }
                return _directPaymentCommand;
            }
        }

        private bool CanDirectPayment(object p)
        {
            if (!AllocationAmount.HasValue || !OldPendingAmount.HasValue) return false;
            var sender = p as System.Windows.DependencyObject;
            return IsValid(sender);
        }

        protected void OnDirectPayment()
        {
            try
            {
                var message = Utility.ShowMessageBoxWithOptions("Do you want to pay the old balance?", System.Windows.MessageBoxButton.YesNo);
                if (message == System.Windows.MessageBoxResult.No) return;
                using (var RMSEntities = new RMSEntities())
                {
                    RMSEntities.DirectPaymentDetails.Add
                        (
                            new DirectPaymentDetail()
                            {
                                CustomerId = SelectedCustomer.Id,
                                PaidAmount = AllocationAmount,
                                PaymentDate = PaymentDate,
                                UpdatedBy = Entitlements.EntitlementInformation.UserInternalId
                            }
                        );

                    var cust = RMSEntities.Customers.FirstOrDefault(c => c.Id == SelectedCustomer.Id);
                    cust.OldBalanceDue -= AllocationAmount;
                    RMSEntities.SaveChanges();
                }
                Clear();
            }
            catch (Exception ex)
            {
                _log.Error("Error on OnDirectPayment:" + SelectedCustomer.Name, ex);
                Utility.ShowErrorBox(ex.Message);
            }
        }
        #endregion

        private bool IsValid(System.Windows.DependencyObject obj)
        {
            // The dependency object is valid if it has no errors and all
            // of its children (that are dependency objects) are error-free.
            return !System.Windows.Controls.Validation.GetHasError(obj) &&
            System.Windows.LogicalTreeHelper.GetChildren(obj)
            .OfType<System.Windows.DependencyObject>()
            .All(IsValid);
        }

        override internal void Clear()
        {
            CustomerPaymentDetailsList.Clear();
            SelectedCustomer = null;
            CustomersList = null;
            AllocationAmount = null;
            ChequeAllocationAmount = null;
            ChequeNo = null;
            ChequeDate = DateTime.Now;
            PaymentDate = DateTime.Now;
            //TotalPendingAmount = null;
            OldPendingAmount = null;
            TotalBillSumBalanceAmount = 0;
        }
    }
}
