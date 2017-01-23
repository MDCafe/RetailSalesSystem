using RetailManagementSystem.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RetailManagementSystem.View
{
    /// <summary>
    /// Interaction logic for SalesEntry.xaml
    /// </summary>
    public partial class SalesEntry : UserControl
    {

        SalesEntryViewModel _salesViewModel;

        public SalesEntry()
        {
            InitializeComponent();            
            this.DataContextChanged += SalesEntry_DataContextChanged;
        }

        private void SalesEntry_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _salesViewModel = this.DataContext  as SalesEntryViewModel;
            this.custComboBoxCol.ItemsSource = _salesViewModel.ProductsPriceList;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //var test = this.Cbo.ItemsSource;
        }

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var productPrice = selectedItem as ProductPrice;

            var selectedRowSaleDetail = _salesViewModel.SaleDetailList.FirstOrDefault(s => s.ProductId == productPrice.ProductId);
            if (selectedRowSaleDetail != null)
            {
                //selectedRowSaleDetail.Qty = productPrice.Quantity;
                selectedRowSaleDetail.SellingPrice = productPrice.SellingPrice;
                selectedRowSaleDetail.CostPrice = productPrice.Price;
                selectedRowSaleDetail.PriceId = productPrice.PriceId;
                
                
                //selectedRowSaleDetail.Amount = productPrice.SellingPrice * selectedRowSaleDetail.Qty;                
                
            }
        }
    }

    //public class DataEntry
    //{
    //    public ObservableCollection<Orders> _cvsOrders = new ObservableCollection<Orders>();
    //    public ObservableCollection<Orders> _productCollection = new ObservableCollection<Orders>();
    //    public ObservableCollection<Customers> CustomersCollection = new ObservableCollection<Customers>();

    //    public ObservableCollection<Orders> ComboOrders
    //    {
    //        get { return new ObservableCollection<Orders>(_cvsOrders.ToList()); }
    //    }

    //    public ObservableCollection<Orders> CvsOrders
    //    {
    //        get { return _cvsOrders; }
    //    }

    //    public ObservableCollection<Orders> ProductCollection
    //    {
    //        get { return _productCollection; }
    //    }

    //    public DataEntry()
    //    {
    //        _cvsOrders.Add(new Orders() { ProductId = 1, ProductDescription = "Marie Tikiri" });
    //        _cvsOrders.Add(new Orders() { ProductId = 3, ProductDescription = "Oil" });

    //        //CustomersCollection.Add(new Customers() { CustomerId = 1, CustomerDescription = "Grand Hotel" });
    //        //CustomersCollection.Add(new Customers() { CustomerId = 2, CustomerDescription = "St. Andrews" });

    //        _productCollection.Add(new Orders() { ProductId = 1, ProductDescription = "Marie Tikiri" });
    //        _productCollection.Add(new Orders() { ProductId = 2, ProductDescription = "Onion Biscuit" });
    //        _productCollection.Add(new Orders() { ProductId = 3, ProductDescription = "Oil" });
    //        _productCollection.Add(new Orders() { ProductId = 4, ProductDescription = "Sugar" });
    //        _productCollection.Add(new Orders() { ProductId = 5, ProductDescription = "Dhal" });
    //        //CollectionView vw = new CollectionView(_productCollection);
    //        //vw.Filter.
    //    }

    //    public class Orders
    //    {
    //        public int ProductId { get; set; }
    //        public string ProductDescription { get; set; }
    //    }

    //    public class Customers
    //    {
    //        public int CustomerId { get; set; }
    //        public string CustomerDescription { get; set; }
    //    }
    //}

}
