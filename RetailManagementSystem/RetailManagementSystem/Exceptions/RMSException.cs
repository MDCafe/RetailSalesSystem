using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Exceptions
{
    class RMSException : ApplicationException
    {
        public RMSException(string message) : base(message) { }
    }
}
