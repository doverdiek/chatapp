using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Models
{
    public class SignalRMessage
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
