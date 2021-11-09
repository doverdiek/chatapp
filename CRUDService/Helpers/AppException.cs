using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDService.Helpers
{
    public class AppException : Exception
    {
        public AppException()
        {
        }

        public AppException(string message) : base(message)
        {
        }

        public AppException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
