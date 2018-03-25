using System.Windows.Controls;
using System.Windows.Data;

namespace RetailManagementSystem.View.Accounts
{
    /// <summary>
    /// Interaction logic for CustomerBillPayments.xaml
    /// </summary>
    public partial class CustomerBillPayments : UserControl
    {
        public CustomerBillPayments()
        {
            InitializeComponent();
            this.DataContextChanged += CustomerBillPayments_DataContextChanged;
        }

        private void CustomerBillPayments_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var datacontext = DataContext as ViewModel.Accounts.CustomerBillPaymentsViewModel;
            ComboBoxColumn.ItemsSource = datacontext.PaymentModes;
            //ComboBoxColumn.SelectedItemBinding = new Binding("SelectedPaymentMode");
        }
    }
}
