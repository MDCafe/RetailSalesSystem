using RetailManagementSystem.Model;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RetailManagementSystem.View
{
    /// <summary>
    /// Interaction logic for SalesEntry.xaml
    /// </summary>
    public partial class SalesEntry : UserControl
    {

        SalesEntryViewModel _salesViewModel;
        //SaleDetailExtn _selRowSaleDetailExtn;

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
            if (productPrice == null) return;
            var selRowSaleDetailExtn = _salesViewModel.SaleDetailList.FirstOrDefault(s => s.ProductId == productPrice.ProductId);
            if (selRowSaleDetailExtn != null)
            {
                //selectedRowSaleDetail.Qty = productPrice.Quantity;
                selRowSaleDetailExtn.SellingPrice = productPrice.SellingPrice;
                selRowSaleDetailExtn.CostPrice = productPrice.Price;
                selRowSaleDetailExtn.PriceId = productPrice.PriceId;
                selRowSaleDetailExtn.AvailableStock = productPrice.Quantity;

                selRowSaleDetailExtn.PropertyChanged += (sender, e) =>
                {
                    var prop = e.PropertyName;
                    if (prop == Constants.AMOUNT) return;


                    var amount = selRowSaleDetailExtn.SellingPrice * selRowSaleDetailExtn.Qty;
                    var discountAmount = selRowSaleDetailExtn.DiscountPercentage != 0 ?
                                         amount - (amount * (selRowSaleDetailExtn.DiscountPercentage / 100)) :
                                         selRowSaleDetailExtn.DiscountAmount != 0 ?
                                         amount - selRowSaleDetailExtn.DiscountAmount :
                                         0;

                    if (discountAmount != 0)
                    {
                        selRowSaleDetailExtn.Amount = discountAmount;
                        selRowSaleDetailExtn.Discount = discountAmount;
                        return;
                    }

                    selRowSaleDetailExtn.Amount = amount;
                    selRowSaleDetailExtn.Discount = 0;
                };
                //_selRowSaleDetailExtn.PropertyChanged += (sender, e) =>
                //{


                //    //switch (prop)
                //    //{
                //    //    case Constants.QTY:
                //    //        //selRowSaleDetailExtn.Amount = amo;
                //    //        break;
                //    //    case Constants.SELLING_PRICE:
                //    //        selRowSaleDetailExtn.Amount = selRowSaleDetailExtn.SellingPrice * selRowSaleDetailExtn.Qty;
                //    //        break;
                //    //    case Constants.DISCOUNT_PERCENT:
                //    //        if(selRowSaleDetailExtn.DiscountPercentage != 0 )
                //    //        {
                //    //            var amount = selRowSaleDetailExtn.SellingPrice * selRowSaleDetailExtn.Qty;
                //    //            selRowSaleDetailExtn.Amount = amount - (amount * (selRowSaleDetailExtn.DiscountPercentage / 100));
                //    //            break;
                //    //        }
                //    //        selRowSaleDetailExtn.Amount = selRowSaleDetailExtn.SellingPrice * selRowSaleDetailExtn.Qty;
                //    //        break;
                //    //    case Constants.DISCOUNT_AMT:
                //    //        if (selRowSaleDetailExtn.DiscountAmount != 0)
                //    //        {
                //    //            selRowSaleDetailExtn.Amount = (selRowSaleDetailExtn.SellingPrice * selRowSaleDetailExtn.Qty) - selRowSaleDetailExtn.DiscountAmount;
                //    //            break;
                //    //        }
                //    //        selRowSaleDetailExtn.Amount = selRowSaleDetailExtn.SellingPrice * selRowSaleDetailExtn.Qty;
                //    //        break;
                //    //}                    
                //};                                                                  
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
