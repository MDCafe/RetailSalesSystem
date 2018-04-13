using RetailManagementSystem.ViewModel.Misc;
using System.Windows;
using System.Windows.Data;

namespace RetailManagementSystem.View.Misc
{
    public partial class PriceListView : Window
    {
        public PriceListView()
        {
            InitializeComponent();
            DataContext = new PriceListViewModel();
        }
    }
}
