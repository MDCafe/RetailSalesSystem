using RetailManagementSystem.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace RetailManagementSystem.View
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class ReportsViewer : UserControl
    {
        ReportViewModel _rptVM;

        public ReportsViewer()
        {
            InitializeComponent();
            this.DataContextChanged += Reports_DataContextChanged;
        }

        private void Reports_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _rptVM = this.DataContext as ReportViewModel;
            _rptVM.RptViewer = this.ReportViewer;
        }
    }
}
