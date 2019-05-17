using RetailManagementSystem.ViewModel.Misc;
using System.Windows;

namespace RetailManagementSystem.View.Misc
{
    public partial class PriceListView : Window
    {
        public PriceListView()
        {
            InitializeComponent();
            DataContext = new PriceListViewModel();
            //this.txtProductName.KeyDown += TxtProductName_KeyDown;
        }

        //private void TxtProductName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == System.Windows.Input.Key.Enter)
        //    {
        //        if (txtProductName.Text == "4792172004080")
        //        {
        //            MessageBox.Show(txtProductName.Text);
        //        }
        //    }
        //}
    }
}
