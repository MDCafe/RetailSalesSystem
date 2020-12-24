using RetailManagementSystem.Model;
using RetailManagementSystem.ViewModel.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace RetailManagementSystem.ViewModel.Misc
{
    class PriceListViewModel : ViewModelBase
    {
        readonly CollectionViewSource _cvsProductsPriceList;
        readonly ObservableCollection<ProductPrice> _productsPriceList;
        string _productName;
        // System.Windows.Data.CollectionView cv;

        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                _cvsProductsPriceList.View.Refresh();
                //_productsPriceList.Refresh();
                //RaisePropertyChanged("ProductsPriceList");
            }
        }

        public ICollectionView ProductsPriceList
        {
            get { return _cvsProductsPriceList.View; }
        }

        public PriceListViewModel()
        {
            _productsPriceList = RMSEntitiesHelper.GetProductPriceList();
            _cvsProductsPriceList = new CollectionViewSource
            {
                Source = _productsPriceList
            };
            _cvsProductsPriceList.Filter += (s, e) =>
             {
                 if (!(e.Item is ProductPrice productPrice) || ProductName == null || string.IsNullOrWhiteSpace(ProductName)) e.Accepted = true;
                 else
                 {
                     e.Accepted = productPrice.ProductName.ToUpper().Contains(ProductName.ToUpper());
                 }
             };

        }

        //private bool FilterCollection(object obj)
        //{
        //    //if (string.IsNullOrWhiteSpace(ProductName)) return false;
        //    //var productPrice = obj as ProductPrice;
        //    //return productPrice.ProductName.Contains(ProductName);
        //}
    }
}
