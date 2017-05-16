using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.View.Behaviours
{
    class ClearOnFocusedBehavior : System.Windows.Interactivity.Behavior<System.Windows.Controls.TextBox>
    {
        private readonly RoutedEventHandler _onGotFocusHandler = (o, e) =>
        {
            ((System.Windows.Controls.TextBox)o).Text =
                string.Empty;
        };

        protected override void OnAttached()
        {
            AssociatedObject.GotFocus += _onGotFocusHandler;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.GotFocus -= _onGotFocusHandler;
        }
    }
}
