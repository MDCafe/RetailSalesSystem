using System;

namespace RetailManagementSystem.Exceptions
{
    public class RMSException : Exception
    {
        public RMSException(string message) : base(message) { }

        public RMSException()
        {
        }

        public RMSException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
