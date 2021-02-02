using RetailManagementSystem.ViewModel.Sales;
using System.Windows;

namespace RetailManagementSystem.View.Sales
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class POSSalesEntry : Window
    {
        public POSSalesEntry()
        {
            InitializeComponent();
            var posSalesEntryViewModel = new POSSalesEntryViewModel(false);

            Loaded += handler;

            DataContext = posSalesEntryViewModel;

            void handler(object sender, RoutedEventArgs e)
            {
                CboCustomers.SelectedValue = posSalesEntryViewModel.DefaultCustomer.Id;
                CboCustomers.SelectedItem = posSalesEntryViewModel.DefaultCustomer;
                posSalesEntryViewModel.SelectedCustomer = posSalesEntryViewModel.DefaultCustomer;
            }
        }      
    }
}
