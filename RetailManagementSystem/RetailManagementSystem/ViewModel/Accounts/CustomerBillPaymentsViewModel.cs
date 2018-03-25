﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.Model;
using log4net;
using System;

namespace RetailManagementSystem.ViewModel.Accounts
{
    class CustomerBillPaymentsViewModel : DocumentViewModel
    {
        private bool _showRestrictedCustomer;
        private int _categoryId;
        private Customer _selectedCustomer;
        private ObservableCollection<CustomerPaymentDetails> _customerPaymentDetailsList;
        private static readonly ILog _log = LogManager.GetLogger(typeof(CustomerBillPaymentsViewModel));
        private IEnumerable<CodeMaster> _paymentModes;
        public ObservableCollection<CustomerPaymentDetails> CustomerPaymentDetailsList
        {
            get { return _customerPaymentDetailsList; }
            set { _customerPaymentDetailsList = value; }
        }

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set { _selectedCustomer = value; }
        }

        public decimal? AllocationAmount { get; set; }

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                using (var rmsEntities = new RMSEntities())
                {
                    return rmsEntities.Customers.ToList().Where(c => c.CustomerTypeId == _categoryId).OrderBy(a => a.Name);
                }
            }
        }

        public IEnumerable<CodeMaster> PaymentModes
        {
            get { return _paymentModes; }
            set
            {
                _paymentModes = value;
            }
        }

        public CodeMaster SelectedPaymentMode { get; set; }

        public CustomerBillPaymentsViewModel(bool showRestrictedCustomer)
        {
            _showRestrictedCustomer = showRestrictedCustomer;
            if (_showRestrictedCustomer)
                _categoryId = Constants.CUSTOMERS_OTHERS;
            else
                _categoryId = Constants.CUSTOMERS_HOTEL;

            Title = "Customer Bill Payment";

            _customerPaymentDetailsList = new ObservableCollection<CustomerPaymentDetails>();

            using (var rmsEntities = new RMSEntities())
            {
                var cnt = rmsEntities.CodeMasters.Local.Count();


                //foreach (var item in rmsEntities.CodeMasters)
                //{

                //}
                _paymentModes = rmsEntities.CodeMasters.Where(c => c.Code == "PMODE" && c.Id !=8).ToList();
            }
        }

        #region GetBillsCommand
        RelayCommand<object> _getBillsCommand = null;
        public ICommand GetBillsCommand
        {
            get
            {
                if (_getBillsCommand == null)
                {
                    _getBillsCommand = new RelayCommand<object>((p) => OnGetBills(), (p) => CanGetBills());
                }

                return _getBillsCommand;
            }
        }

        private bool CanGetBills()
        {
            return _selectedCustomer != null;
        }

        protected void OnGetBills()
        {
            _customerPaymentDetailsList.Clear();
            using (var rmsEntities = new RMSEntities())
            {
                var mySQLparam = new MySql.Data.MySqlClient.MySqlParameter("@customerId", MySql.Data.MySqlClient.MySqlDbType.Int32);
                mySQLparam.Value = SelectedCustomer.Id;
                var custPayDetails = rmsEntities.Database.SqlQuery<CustomerPaymentDetails>
                                                                    ("CALL GetCustomerPaymentDetails(@customerId)", mySQLparam);
                var i = 0;
                foreach (var item in custPayDetails)
                {
                    item.SerialNo = ++i;
                    _customerPaymentDetailsList.Add(item);
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
            return _selectedCustomer != null;
        }

        protected void OnSave()
        {
            try
            { 
                using (var rmsEntities = new RMSEntities())
                {
                    foreach (var item in _customerPaymentDetailsList)
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
                            CustomerId = _selectedCustomer.Id,
                            PaymentMode = item.PaymentMode.Id
                        };
                        if (item.PaymentMode.Description == "Cheque")
                        {
                            AddChequePayment(item, newPaymentDetail);
                        }

                        rmsEntities.PaymentDetails.Add(newPaymentDetail);
                    }

                    rmsEntities.SaveChanges();
                    Clear();
                }
            }
            catch (System.Exception ex)
            {
                _log.Error("Error on saving payments for customer:" + _selectedCustomer.Name, ex);
                Utility.ShowErrorBox(ex.Message);
            }
        }

        private bool ValidateCustomerPaymentDetails()
        {
            foreach (var item in _customerPaymentDetailsList)
            {
                if (item.BalanceAmount < 0)
                {
                    Utility.ShowErrorBox("Balance Amount can't be less than zero");
                    return false;
                }
                if (item.PaymentMode == null)
                {
                    Utility.ShowErrorBox("Choose payment mode as Cash/Cheque");
                    return false;
                }

                if (item.PaymentMode.Description == "Cheque")
                {
                    if (!item.ChequeNo.HasValue)
                    {
                        Utility.ShowErrorBox("Cheque No can't be blank");
                        return false;
                    }
                    if (item.ChequeDate.HasValue)
                    {
                        Utility.ShowErrorBox("Cheque date can't be blank");
                        return false;
                    }
                }
            }
            return true;
        }

        private static void AddChequePayment(CustomerPaymentDetails item, PaymentDetail paymentDetails)
        {
            var chqPaymentDetails =
                                    new ChequePaymentDetail()
                                    {
                                        ChequeDate = item.ChequeDate,
                                        ChequeNo = item.ChequeNo,
                                        IsChequeRealised = false
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
                var decreasingAllocationAmount = AllocationAmount.Value; 
                foreach (var item in _customerPaymentDetailsList)
                {
                    if (decreasingAllocationAmount <= 0) return;
                    if((item.TotalAmount - item.AmountPaid) !=0)
                    {
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
            catch (System.Exception ex)
            {
                _log.Error("Error on allocation:" + _selectedCustomer.Name, ex);
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
            _customerPaymentDetailsList.Clear();
            SelectedCustomer = null;
            AllocationAmount = null;
        }
    }
}
