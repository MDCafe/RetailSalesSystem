using RetailManagementSystem.ViewModel;
using System.Windows;

namespace RetailManagementSystem.View
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class ReportsViewerWindow : Window
    {
        ReportViewModel _rptVM;

        public ReportsViewerWindow()
        {
            InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                _rptVM = this.DataContext as ReportViewModel;
                _rptVM.RptViewer = this.ReportViewer;
            };
        }
    }
}
