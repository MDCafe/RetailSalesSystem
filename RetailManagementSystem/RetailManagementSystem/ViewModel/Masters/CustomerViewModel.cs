using System.Windows.Input;
using System.Linq;
using System;
using System.Collections.Generic;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System.Windows;
using RetailManagementSystem.Utilities;
using System.Data.Entity;

namespace RetailManagementSystem.ViewModel.Masters
{
    class CustomerViewModel : ViewModelBase
    {
        Customer _customer;
        bool _isEditMode;
        IEnumerable<Customer> _customersList;        

        public CustomerViewModel()
        {            
            _customer = new Customer();                        
            using (RMSEntities rmsEntities = new RMSEntities())
            {
                CustomerCategory = rmsEntities.Categories.Where(c => c.parentId == 1).ToList();
                var cnt = rmsEntities.Categories.Count();
            }
        }

        #region Public Variables

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                if (_customersList == null)
                {
                    using (RMSEntities rmsEntities = new RMSEntities())
                    {
                        _customersList = rmsEntities.Customers.ToList();
                    }
                }

                return _customersList;
            }

            private set
            {
                _customersList = value;
                RaisePropertyChanged("CustomersList");
            }
        }

        public Customer SelectedCustomer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                RaisePropertyChanged("SelectedCustomer");
            }
        }

        public Customer DblClickSelectedCustomer
        {
            get;set;
        }

        public string SearchText { get; set; }

        public IEnumerable<Category> CustomerCategory { get; set; }

        #endregion

        #region CloseWindow Command
        public RelayCommand<Window> _closeCommand { get; private set; }

        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand<Window>((w) => CloseWindow(w));
                }

                return _closeCommand;
            }
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
        #endregion

        #region Clear Command
        RelayCommand<object> _clearCommand = null;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand<object>(
                        (p) =>
                        {
                            //_customer.Id = -1;
                            //_customer.Name = "";
                            //_customer.Address = "";
                            //_customer.BalanceDue = 0;
                            //_customer.City = "";
                            //_customer.CreditDays = 0;
                            //_customer.CreditLimit = 0;
                            //_customer.CustomerTypeId = -1;
                            //_customer.Description = "";
                            //_customer.Email = "";
                            //_customer.LanNo = 0;
                            _customer = new Customer();
                            RaisePropertyChanged("SelectedCustomer");

                            _isEditMode = false;
                            DblClickSelectedCustomer = null;
                            using (RMSEntities rmsEntities = new RMSEntities())
                            {
                                CustomersList = rmsEntities.Customers.ToList();
                            }
                            SearchText = "";
                        }
                        );
                }
                return _clearCommand;
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
                    _saveCommand = new RelayCommand<object>((p) => OnSave(p), (p) => CanSave(p));
                }

                return _saveCommand;
            }
        }        

        public bool CanSave(object parameter)
        {
            return !string.IsNullOrWhiteSpace(SelectedCustomer.Name);                        
        }

        private void OnSave(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(SelectedCustomer.Name))
            {
                using (RMSEntities rmsEntities = new RMSEntities())
                {
                    if (_isEditMode)
                    {
                        //var cust = rmsEntities.Customers.FirstOrDefault(c => c.Id == _customer.Id);                        
                        //cust = _customer;
                        rmsEntities.Entry(_customer).State = EntityState.Modified;
                    }
                    else
                        rmsEntities.Customers.Add(_customer);

                    rmsEntities.SaveChanges();
                }
                ClearCommand.Execute(null);
                RaisePropertyChanged("CustomersList");
            }
            else
                Utility.ShowErrorBox("Customer Name can't be empty");
        }

        #endregion


        #region DeleteCommand
        RelayCommand<object> _deleteCommand = null;
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand<object>((p) => OnDelete(), (p) => CanDelete());
                }

                return _deleteCommand;
            }
        }

        public bool CanDelete()
        {
            return !string.IsNullOrWhiteSpace(SelectedCustomer.Name) && SelectedCustomer != null;
        }

        private void OnDelete()
        {
            var msgResult = Utility.ShowMessageBoxWithOptions("Do you want to delete the customer : " + _customer.Name);
            if(msgResult != MessageBoxResult.Yes)
            {
                return;
            }

            using (RMSEntities rmsEntities = new RMSEntities())
            {
                var cust = rmsEntities.Customers.FirstOrDefault(c => c.Id == _customer.Id);
                if (cust == null)
                {
                    Utility.ShowMessageBoxWithOptions("Customer : " + _customer.Name + " doesn't exist");
                    return;
                }

                rmsEntities.Customers.Remove(cust);
                rmsEntities.SaveChanges();
            }            
            ClearCommand.Execute(null);
            RaisePropertyChanged("CustomersList");         
        }

        #endregion

        #region DoubleClickCommand
        RelayCommand<object> _doubleClickCommand = null;
        public ICommand DoubleClickCommand
        {
            get
            {
                if (_doubleClickCommand == null)
                {
                    _doubleClickCommand = new RelayCommand<object>
                        (
                            p =>
                            {
                                _isEditMode = true;
                                SelectedCustomer = DblClickSelectedCustomer;                      
                            }
                        );
                }

                return _doubleClickCommand;
            }
        }


        #endregion

        #region SearchCommand
        RelayCommand<object> _searchCommand = null;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand<object>
                        (
                            p =>
                            {
                                if (string.IsNullOrWhiteSpace(SearchText)) return;
                                CustomersList = CustomersList.Where(c => c.Name.StartsWith(SearchText,StringComparison.InvariantCultureIgnoreCase));
                                
                            }
                        );
                }

                return _searchCommand;
            }
        }

       


        #endregion
    }
}
