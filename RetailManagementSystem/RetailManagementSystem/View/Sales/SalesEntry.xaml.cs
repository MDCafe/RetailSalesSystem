using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel;
using RetailManagementSystem.ViewModel.Extensions;
using System;
using System.Linq;
using System.Windows.Controls;

namespace RetailManagementSystem.View.Sales
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

            DataContextChanged += (sender, eventArgs) =>
            {
                _salesViewModel = this.DataContext as SalesEntryViewModel;
                this.custComboBoxCol.ItemsSource = _salesViewModel.ProductsPriceList;
                _salesViewModel.Extensions = SalesExtn.DataContext as IExtensions;
            };

            custComboBoxCol.comboBox.PreviewTextInput += ComboBox_PreviewTextInput;
            custComboBoxCol.OnComboLoadedEvent += (txt) =>
            {
                custComboBoxCol._cboTextBox.PreviewKeyUp += (s, e) =>
                {
                    if (e.Key == System.Windows.Input.Key.Back && string.IsNullOrWhiteSpace(custComboBoxCol.comboBox.Text))
                    {
                        custComboBoxCol.comboBox.ItemsSource = _salesViewModel.ProductsPriceList;
                    }
                };
            };
        }        

        private void ComboBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {             
            ComboBox cmb = (ComboBox)sender;

            cmb.IsDropDownOpen = true;

            if (!string.IsNullOrEmpty(cmb.Text))
            {
                string fullText = cmb.Text.Insert(custComboBoxCol._cboTextBox.CaretIndex, e.Text);
                cmb.ItemsSource = _salesViewModel.ProductsPriceList.Where(s => s.ProductName.StartsWith(fullText, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            //else if (!string.IsNullOrEmpty(e.Text))
            if (!string.IsNullOrEmpty(e.Text))
            {
                cmb.ItemsSource = _salesViewModel.ProductsPriceList.Where(s => s.ProductName.StartsWith(cmb.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            else
            {
                cmb.ItemsSource = _salesViewModel.ProductsPriceList;
            }        
        }               

        private void custComboBoxCol_ComboBoxSelectedEvent(object selectedItem)
        {
            var productPrice = selectedItem as ProductPrice;
            _salesViewModel.SetProductDetails(productPrice);                                                                                     
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
