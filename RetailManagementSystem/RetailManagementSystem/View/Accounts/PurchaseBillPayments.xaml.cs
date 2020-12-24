using System.Windows.Controls;

namespace RetailManagementSystem.View.Accounts
{
    /// <summary>
    /// Interaction logic for CustomerBillPayments.xaml
    /// </summary>
    public partial class PurchaseBillPayments : UserControl
    {
        public PurchaseBillPayments()
        {
            InitializeComponent();
            DataContextChanged += ((s, e) =>
            {
                var datacontext = DataContext as ViewModel.Accounts.PurchaseBillPaymentsViewModel;
                ComboBoxColumn.ItemsSource = datacontext.PaymentModes;
                //ComboBoxColumn.SelectedItemBinding = new Binding("SelectedPaymentMode");
            });
        }
    }
}
