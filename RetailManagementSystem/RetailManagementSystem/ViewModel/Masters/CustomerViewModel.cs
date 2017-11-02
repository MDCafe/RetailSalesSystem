using System.Windows.Input;
using System.Linq;
using System;
using System.Collections.Generic;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System.Windows;
using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Masters
{
    class CustomerViewModel : ViewModelBase
    {
        Customer _customer;
        bool _isEditMode;
        IEnumerable<Customer> _customersList;
        IEnumerable<Category> _customerCategory;
        RMSEntities _rmsEntities;

        public CustomerViewModel()
        {
            //var names = RMSEntitiesHelper.RMSEntities.Customers.SelectMany(c => c.Name);
            _customer = new Customer();
            _rmsEntities = new RMSEntities();
            var cnt = _rmsEntities.Categories.Count();

            _customerCategory = _rmsEntities.Categories.Where(c => c.parentId == 1).ToList();

        }

        #region Public Variables

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                if(_customersList == null)
                    _customersList = _rmsEntities.Customers.ToList();

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

        public IEnumerable<Category> CustomerCategory
        {
            get
            {
                return _customerCategory;
            }

            set
            {
                _customerCategory = value;
            }
        }

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
                            CustomersList = _rmsEntities.Customers.ToList();
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
                if (_isEditMode)
                {
                    var cust = _rmsEntities.Customers.FirstOrDefault(c => c.Id == _customer.Id);
                    cust = _customer;
                }
                else
                    _rmsEntities.Customers.Add(_customer);

                _rmsEntities.SaveChanges();
                ClearCommand.Execute(null);
                RaisePropertyChanged("CustomersList");
            }
            else
                Utilities.Utility.ShowErrorBox("Customer Name can't be empty");
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

            var cust = _rmsEntities.Customers.FirstOrDefault(c => c.Id == _customer.Id);
            if(cust == null)
            {
                Utility.ShowMessageBoxWithOptions("Customer : " + _customer.Name + " doesn't exist");
                return;
            }

            _rmsEntities.Customers.Remove(cust);
            _rmsEntities.SaveChanges();
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
