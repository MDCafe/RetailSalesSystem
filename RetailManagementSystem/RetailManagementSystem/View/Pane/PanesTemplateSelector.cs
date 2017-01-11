using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using RetailManagementSystem.ViewModel;
using RetailManagementSystem.ViewModel.Base;

namespace RetailManagementSystem.View.Pane
{
  class PanesTemplateSelector : DataTemplateSelector
    {
        public PanesTemplateSelector()
        {
        
        }

        public DataTemplate DocumentViewTemplate
        {
            get;
            set;
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is DocumentViewModel)
                return DocumentViewTemplate;

            //if (item is StartPageViewModel)
            //  return StartPageViewTemplate;

            //if (item is FileStatsViewModel)
            //    return FileStatsViewTemplate;

            //if (item is RecentFilesViewModel)
            //  return RecentFilesViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
