using System.Windows.Input;
using System.Linq;
using System;
using System.Collections.Generic;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System.Windows;

namespace RetailManagementSystem.ViewModel.Masters
{
    class CustomerViewModel : ViewModelBase
    {
        Customer _customer;
        bool _isEditMode;

        public CustomerViewModel()
        {
            //var names = RMSEntitiesHelper.RMSEntities.Customers.SelectMany(c => c.Name);
            _customer = new Customer();
        }

        #region Public Variables

        public IEnumerable<Customer> CustomersList
        {
            get
            {
                return RMSEntitiesHelper.RMSEntities.Customers.ToList();
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
                    var cust = RMSEntitiesHelper.RMSEntities.Customers.FirstOrDefault(c => c.Id == _customer.Id);
                    cust = _customer;
                }
                else
                    RMSEntitiesHelper.RMSEntities.Customers.Add(_customer);

                RMSEntitiesHelper.RMSEntities.SaveChanges();
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
            var msgResult = Utilities.Utility.ShowMessageBoxWithOptions("Do you want to delete the customer : " + _customer.Name);
            if(msgResult != MessageBoxResult.Yes)
            {
                return;
            }

            var cust = RMSEntitiesHelper.RMSEntities.Customers.FirstOrDefault(c => c.Id == _customer.Id);
            RMSEntitiesHelper.RMSEntities.Customers.Remove(cust);
            RMSEntitiesHelper.RMSEntities.SaveChanges();
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
                                CustomersList.Where(c => c.Name == SearchText);
                                RaisePropertyChanged("CustomersList");
                            }
                        );
                }

                return _searchCommand;
            }
        }


        #endregion
    }
}
