using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Common
{
    public class LogInResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        public LogInResult(bool isSuccessful, string message)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }
    }

}
