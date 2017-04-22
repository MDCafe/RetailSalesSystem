using RetailManagementSystem.ViewModel.Extensions;
using System.Windows.Controls;

namespace RetailManagementSystem.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SalesExtension : UserControl
    {
        SalesExtensionViewModel _salesExtensionViewModel;

        public SalesExtension()
        {
            InitializeComponent();
            _salesExtensionViewModel = new SalesExtensionViewModel();
            this.DataContext = _salesExtensionViewModel;
        }
    }
}
