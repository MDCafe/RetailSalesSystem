using RetailManagementSystem.UserControls;
using System;
using System.Collections.Generic;

namespace RetailManagementSystem.ViewModel.Notification
{
    internal class NotificationViewModel<T>
    {
        public IEnumerable<T> DataList { get; set; }
        private string[,] _properties;

        public NotificationViewModel(IEnumerable<T> dataList, string[,] properties)
        {
            DataList = dataList;
            _properties = properties;
        }

        public void ExecuteShowWindow()
        {
            App.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
               () =>
               {
                   var notify = new NotificationWindow();
                   notify.DataContext = this;
                   notify.GenerateListView(_properties);
                   notify.Show();
               }));
        }
    }
}
