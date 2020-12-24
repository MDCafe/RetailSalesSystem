using RetailManagementSystem.ViewModel.Base;
using System.Windows;
using System.Windows.Controls;

namespace RetailManagementSystem.View.Pane
{
    class PanesStyleSelector : StyleSelector
    {


        public Style DocumentStyle
        {
            get;
            set;
        }

        public override Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if (item is DocumentViewModel)
                return DocumentStyle;

            return base.SelectStyle(item, container);
        }
    }
}
