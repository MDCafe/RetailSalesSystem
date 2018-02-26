using System.Windows.Input;
using System.Linq;
using System;
using System.Windows;
using System.Collections.Generic;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using RetailManagementSystem.Utilities;

namespace RetailManagementSystem.ViewModel.Masters
{
    class CompanyViewModel : ViewModelBase
    {
        Company _company;
        bool _isEditMode;
        IEnumerable<Company> _companyList;
        IEnumerable<Category> _companyCategory;
        RMSEntities _rmsEntities;

        public CompanyViewModel()
        {
             _company = new Company();
            _rmsEntities = new RMSEntities();
            var cnt = _rmsEntities.Categories.Count();

            _companyCategory = _rmsEntities.Categories.Where(c => c.parentId == 2).ToList();
        }

        #region Public Variables

        public IEnumerable<Company> CompanyList
        {
            get
            {
                if(_companyList == null)
                    _companyList = _rmsEntities.Companies.ToList();

                return _companyList;
            }

            private set
            {
                _companyList = value;
                RaisePropertyChanged("CompanyList");
            }
        }

        public Company SelectedCompany
        {
            get { return _company; }
            set
            {
                _company = value;
                RaisePropertyChanged("SelectedCompany");
            }
        }

        public Company DblClickSelectedCompany
        {
            get;set;
        }

        public string SearchText { get; set; }

        public IEnumerable<Category> CompnayCategory
        {
            get
            {
                return _companyCategory;
            }

            set
            {
                _companyCategory = value;
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
                            _company = new Company();
                            RaisePropertyChanged("SelectedCompany");


                            _isEditMode = false;
                            DblClickSelectedCompany = null;
                            CompanyList = _rmsEntities.Companies.ToList();
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
            return !string.IsNullOrWhiteSpace(SelectedCompany.Name);                        
        }

        private void OnSave(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(SelectedCompany.Name))
            {
                if (_isEditMode)
                {
                    var cust = _rmsEntities.Companies.FirstOrDefault(c => c.Id == _company.Id);
                    cust = _company;
                }
                else
                    _rmsEntities.Companies.Add(_company);

                _rmsEntities.SaveChanges();
                ClearCommand.Execute(null);
                RaisePropertyChanged("CompanyList");
            }
            else
                Utility.ShowErrorBox("Company Name can't be empty");
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
            return !string.IsNullOrWhiteSpace(SelectedCompany.Name) && SelectedCompany != null;
        }

        private void OnDelete()
        {
            var msgResult = Utility.ShowMessageBoxWithOptions("Do you want to delete the company : " + _company.Name);
            if(msgResult != MessageBoxResult.Yes)
            {
                return;
            }

            var cmp = _rmsEntities.Companies.FirstOrDefault(c => c.Id == _company.Id);
            if(cmp == null)
            {
                Utility.ShowMessageBoxWithOptions("Company : " + _company.Name + " doesn't exist");
                return;
            }

            _rmsEntities.Companies.Remove(cmp);
            _rmsEntities.SaveChanges();
            ClearCommand.Execute(null);
            RaisePropertyChanged("CompnayList");         
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
                                SelectedCompany = DblClickSelectedCompany;                      
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
                                CompanyList = CompanyList.Where(c => c.Name.StartsWith(SearchText,StringComparison.InvariantCultureIgnoreCase));
                                
                            }
                        );
                }

                return _searchCommand;
            }
        }

       


        #endregion
    }
}
