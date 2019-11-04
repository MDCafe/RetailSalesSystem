using System;

namespace RetailManagementSystem.Exceptions
{
    public class RMSException : ApplicationException
    {
        public RMSException(string message) : base(message) { }
    }
}
