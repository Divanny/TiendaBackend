using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Common
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public OperationResult(bool success, string message, object data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
        public OperationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}   
