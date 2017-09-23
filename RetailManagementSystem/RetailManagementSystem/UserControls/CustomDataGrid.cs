using System.Windows.Automation.Peers;

namespace RetailManagementSystem.UserControls
{
    public class CustomDataGrid : Microsoft.Windows.Controls.DataGrid
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return null;
        }
    }
}
