namespace RetailManagementSystem
{
    using Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    internal class RMSEntitiesHelper
    {
        RMSEntities _rmsEntities;
        static RMSEntitiesHelper _rMSEntitiesHelper;
        static object _syncRoot = new object();

        List<INotifier> _notifierList = new List<INotifier>();   
        
        private RMSEntitiesHelper()
        {
            _rmsEntities = new RMSEntities();
        }

        public static RMSEntitiesHelper Instance
        {
            get
            {
                Monitor.Enter(_syncRoot);
                if (_rMSEntitiesHelper == null)
                {
                    Monitor.Exit(_syncRoot);
                    return _rMSEntitiesHelper = new RMSEntitiesHelper();
                }
                Monitor.Exit(_syncRoot);
                return _rMSEntitiesHelper;
            }            
        }

        public RMSEntities RMSEntities
        {
            get
            {                
                    return _rmsEntities;
            }
        }

        public void AddNotifier(INotifier notifier)
        {
            Monitor.Enter(_notifierList);
            _notifierList.Add(notifier);
            Monitor.Exit(_notifierList);
        }

        public void RemoveNotifier(INotifier notifier)
        {
            Monitor.Enter(_notifierList);
            _notifierList.Remove(notifier);
            Monitor.Exit(_notifierList);
        }

        public void SelectRunningBillNo(int categoryId)
        {
            string sqlRunningNo = "select max(rollingno) + 1 from category cat where  cat.id = @p0";
            var salesNo = _rmsEntities.Database.SqlQuery<int>(sqlRunningNo, categoryId).FirstOrDefault();

            foreach (var notifyList in _notifierList)
            {
                notifyList.Notify(salesNo);
            }
        }
    }
}
