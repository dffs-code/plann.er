using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journey.Communication.Responses
{
    public class ResponseShortUserJson
    {
        public Guid Id {  get; set; }

        public string Username { get; set; } =  string.Empty;
    }
}
