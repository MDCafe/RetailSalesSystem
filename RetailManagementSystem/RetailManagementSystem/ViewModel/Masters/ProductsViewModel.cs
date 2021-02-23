using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Masters
{
    class ProductsViewModel : ViewModelBase
    {
        Product _product;
        bool _isEditMode;
        IEnumerable<Product> _productsList;
        IEnumerable<Company> _companiesList;
        readonly RMSEntities _rmsEntities;
        ObservableCollection<PriceDetail> _priceDetailsList;

        public IEnumerable<Product> EmptyBottleList { get; set; }
        public int SelectedEmptyProductId { get; set; }
        public int? EmptyBottleValue { get; set; }

        public int PacketsPerCase { get; set; }

        public ProductsViewModel()
        {
            _product = new Product
            {
                SupportsMultiPrice = false
            };
            _rmsEntities = new RMSEntities();
            var cnt = _rmsEntities.Categories.Count();

            ProductCategories = _rmsEntities.Categories.Where(c => c.parentId == 3).ToList().OrderBy(p => p.name);
            UnitOfMeasures = _rmsEntities.MeasuringUnits.ToList().OrderBy(p => p.unit);
            _companiesList = _rmsEntities.Companies.ToList().OrderBy(c => c.Name);
            LoadEmptyBottlesList();

            _priceDetailsList = new ObservableCollection<PriceDetail>
            {
                new PriceDetail()
            };

            ProductActiveValues = new BooleanValue().BooleanValues;
        }

        private void LoadEmptyBottlesList()
        {
            EmptyBottleList = _rmsEntities.Products.Where(p => p.CategoryId == 23).ToList();//Empty Bottles
        }

        #region Public Variables

        public IEnumerable<Product> ProductsList
        {
            get
            {
                if (_productsList == null)
                    _productsList = _rmsEntities.Products.ToList();

                return _productsList;
            }

            private set
            {
                _productsList = value;
                RaisePropertyChanged(nameof(ProductsList));
            }
        }

        public IEnumerable<Company> Companies
        {
            get
            {
                if (_companiesList == null)
                    _companiesList = _rmsEntities.Companies.ToList();

                return _companiesList;
            }

            private set
            {
                _companiesList = value;
            }
        }

        public IEnumerable<MeasuringUnit> UnitOfMeasures { get; set; }


        public Product SelectedProduct
        {
            get { return _product; }
            set
            {
                _product = value;
                RaisePropertyChanged(nameof(SelectedProduct));
            }
        }

        public List<KeyValuePair<bool, string>> ProductActiveValues { get; set; }

        public Product DblClickSelectedProduct
        {
            get; set;
        }

        public string SearchText { get; set; }

        public IEnumerable<Category> ProductCategories { get; set; }


        public ObservableCollection<PriceDetail> PriceDetailList
        {
            get
            {
                return _priceDetailsList;
            }

            private set
            {
                _priceDetailsList = value;
                RaisePropertyChanged("PriceList");
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
                            _product = new Product
                            {
                                SupportsMultiPrice = false
                            };
                            RaisePropertyChanged(nameof(SelectedProduct));
                            _priceDetailsList.Clear();
                            _priceDetailsList.Add(new PriceDetail());
                            RaisePropertyChanged("PriceDetailsList");

                            _isEditMode = false;
                            DblClickSelectedProduct = null;
                            ProductsList = _rmsEntities.Products.ToList();
                            SearchText = "";
                            LoadEmptyBottlesList();
                            SelectedEmptyProductId = -1;
                            EmptyBottleValue = 0;
                            PacketsPerCase = 0;
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
                    _saveCommand = new RelayCommand<object>((p) => OnSave(), (p) =>
                    {
                        return !string.IsNullOrWhiteSpace(SelectedProduct.Name);
                    }
                    );
                }
                return _saveCommand;
            }
        }

        private void OnSave()
        {
            if (!Validate()) return;

            if (_isEditMode)
            {
                var cust = _rmsEntities.Products.FirstOrDefault(c => c.Id == _product.Id);
                cust = _product;
                _product.UpdatedBy = Entitlements.EntitlementInformation.UserInternalId;
                var priceDetailsToSave = _rmsEntities.PriceDetails.Where(pr => pr.ProductId == SelectedProduct.Id).ToList();
                foreach (var item in _priceDetailsList)
                {
                    var itemToUpdate = priceDetailsToSave.Find(a => a.PriceId == item.PriceId);
                    itemToUpdate.Price = item.Price;
                    itemToUpdate.SellingPrice = item.SellingPrice;
                    itemToUpdate.UpdatedBy = Entitlements.EntitlementInformation.UserInternalId;
                }

                var productCaseMapping = _rmsEntities.ProductCaseMappings.Where(p => p.ProductId == _product.Id).FirstOrDefault();                
                if (productCaseMapping != null)
                {
                    productCaseMapping.ItemPerCase = PacketsPerCase;
                }
                else if(PacketsPerCase !=0)
                {
                    CreateProductCaseMapping();
                }

#if DebugPOS || ReleasePOS
                //check if emtpy already exists
                var emtpyBottle = _rmsEntities.ProductEmptyMappings.FirstOrDefault(e => e.ProductId == SelectedProduct.Id);
                if (emtpyBottle != null)
                {
                    emtpyBottle.EmptyProductId = SelectedEmptyProductId;
                    emtpyBottle.EmptyProductValue = EmptyBottleValue;
                }
                else
                    //New empty bottle assigned
                    CreateEmptyBottles();
#endif
            }
            else
            {
                var priceDetailNew = _priceDetailsList[0];
                if (priceDetailNew.Price == 0 || priceDetailNew.SellingPrice == 0)
                {
                    Utility.ShowErrorBox("Please enter Cost price and Selling Price");
                    return;
                }
                _rmsEntities.Stocks.Add(new Stock()
                {
                    ExpiryDate = DateTime.Now.AddMonths(6),
                    PriceDetail = priceDetailNew,
                    Quantity = 0,
                    Product = _product,
                    UpdatedBy = Entitlements.EntitlementInformation.UserInternalId
                });
                _rmsEntities.Products.Add(_product);
                _rmsEntities.PriceDetails.Add(priceDetailNew);

                CreateProductCaseMapping();

#if DebugPOS || ReleasePOS
                    CreateEmptyBottles();
#endif
            }

            _product.Name = _product.Name.Trim();
            _rmsEntities.SaveChanges();
            ClearCommand.Execute(null);
            RaisePropertyChanged(nameof(ProductsList));

            void CreateProductCaseMapping()
            {
                _rmsEntities.ProductCaseMappings.Add(new ProductCaseMapping()
                {
                    ItemPerCase = PacketsPerCase,
                    Product = _product
                });
            }
        }



        private void CreateEmptyBottles()
        {
            //Empty bottles mapping available
            if (SelectedEmptyProductId != 0)
            {
                if (!EmptyBottleValue.HasValue)
                {
                    Utility.ShowErrorBox("Empty Bottle Value can't be empty");
                    return;
                }
                _rmsEntities.ProductEmptyMappings.Add(new ProductEmptyMapping()
                {
                    EmptyProductId = SelectedEmptyProductId,
                    Product = _product,
                    EmptyProductValue = EmptyBottleValue
                });
            }
        }


        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(SelectedProduct.Name))
            {
                Utility.ShowErrorBox("Product Name can't be empty");
                return false;
            }

            if (!_product.CategoryId.HasValue)
            {
                Utility.ShowErrorBox(" Category can't be empty");
                return false;
            }
            if (!_product.UnitOfMeasure.HasValue)
            {
                Utility.ShowErrorBox("Unit of measure can't be empty");
                return false;
            }
            return true;
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
            return !string.IsNullOrWhiteSpace(SelectedProduct.Name) && SelectedProduct != null;
        }

        private void OnDelete()
        {
            var msgResult = Utility.ShowMessageBoxWithOptions("Do you want to delete the Product : " + _product.Name);
            if (msgResult != MessageBoxResult.Yes)
            {
                return;
            }

            var cust = _rmsEntities.Products.FirstOrDefault(c => c.Id == _product.Id);
            if (cust == null)
            {
                Utility.ShowMessageBoxWithOptions("Product : " + _product.Name + " doesn't exist");
                return;
            }

            _rmsEntities.Products.Remove(cust);
            _rmsEntities.PriceDetails.RemoveRange(_priceDetailsList);
            foreach (var item in _priceDetailsList)
            {
                var stkToRemove = _rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == item.ProductId &&
                                                                     s.PriceId == item.PriceId);
                if (stkToRemove != null)
                    _rmsEntities.Stocks.Remove(stkToRemove);
            }

            _rmsEntities.SaveChanges();
            ClearCommand.Execute(null);
            RaisePropertyChanged(nameof(ProductsList));
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
                                SelectedProduct = DblClickSelectedProduct;
                                _priceDetailsList.Clear();
                                var priceDetails = _rmsEntities.PriceDetails.Where(pr => pr.ProductId == SelectedProduct.Id).ToList();
                                priceDetails.ForEach((prd) => _priceDetailsList.Add(prd));
                                var pcase = SelectedProduct.ProductCaseMappings.FirstOrDefault();
                                if(pcase !=null)
                                    PacketsPerCase = Convert.ToInt32(pcase.ItemPerCase.Value);

#if DebugPOS || ReleasePOS
                                var emtpyBottle = _rmsEntities.ProductEmptyMappings.FirstOrDefault(e => e.ProductId == SelectedProduct.Id);
                                if(emtpyBottle !=null)
                                {
                                    SelectedEmptyProductId = emtpyBottle.EmptyProductId.Value;
                                    EmptyBottleValue = emtpyBottle.EmptyProductValue;
                                }
#endif
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
                                _priceDetailsList.Clear();
                                ProductsList = ProductsList.Where(c => c.Name.StartsWith(SearchText, StringComparison.InvariantCultureIgnoreCase));
                            }
                        );
                }

                return _searchCommand;
            }
        }

#endregion
    }
}
