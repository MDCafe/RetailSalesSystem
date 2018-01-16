﻿using RetailManagementSystem.ViewModel.Reports.Stock;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Stock
{
    public partial class StockReport : Window
    {
        StockReportViewModel _stockReportViewModel = new StockReportViewModel(false);

        public StockReport(bool showRestrictedCustomers)
        {
            InitializeComponent();
            this.DataContext = _stockReportViewModel;
        }
    }
}