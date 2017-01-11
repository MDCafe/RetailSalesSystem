using System.Windows;
using System.Windows.Controls;
using RetailManagementSystem.ViewModel;
using RetailManagementSystem.ViewModel.Base;

namespace RetailManagementSystem.View.Pane
{
  class PanesStyleSelector : StyleSelector
  {
    

    public Style DocumentStyle
    {
      get;
      set;
    }

    public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
    {
      if (item is DocumentViewModel)
        return DocumentStyle;

     return base.SelectStyle(item, container);
    }
  }
}
