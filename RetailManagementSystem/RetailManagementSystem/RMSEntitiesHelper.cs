namespace RetailManagementSystem
{
    static class RMSEntitiesHelper
    {
        static RMSEntities _rmEntities;

        static RMSEntitiesHelper()
        {
            if (_rmEntities == null)
            {
                _rmEntities = new RMSEntities();
            }
        }

        public static RMSEntities RMSEntities
        {
            get { return _rmEntities; }
        }
    }
}
