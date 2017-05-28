using Microsoft.Windows.Controls;
using RetailManagementSystem.ViewModel.Masters;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RetailManagementSystem.View.Masters
{
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer : Window
    {
        
        public Customer()
        {
            InitializeComponent();
            DataContext = new CustomerViewModel();
        }

        //private void SearchValue(string matchValue)
        //{
        //    var colBind = ((Microsoft.Windows.Controls.DataGridTextColumn)CustomersGrid.Columns[1]).Binding as Binding;

        //    Func<object, Binding, string> getValue = (srcObj, bind) =>
        //    {
        //        var cntrl = new UserControl();
        //        cntrl.DataContext = srcObj;
        //        cntrl.SetBinding(ContentProperty, bind);
        //        return cntrl.GetValue(ContentProperty);
        //    };

        //    foreach (var data in CustomersGrid.Items)
        //    {
        //        var value = getValue(data, colBind);
        //        if (value.ToString() == matchValue)
        //        {
        //            CustomersGrid.SelectedItem = data;
        //            CustomersGrid.ScrollIntoView(data);

        //            break;
        //        }
        //    }
        //}

        //private void button_Click(object sender, RoutedEventArgs e)
        //{
        //    SearchValue(txtSearch.Text);
        //}
    }
}
