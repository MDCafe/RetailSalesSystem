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
    public partial class Products : Window
    {
        
        public Products()
        {
            InitializeComponent();
            DataContext = new ProductsViewModel();
        }
    }
}
