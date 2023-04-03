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
        public bool UserValidated { get; set; }
        public int idUsuario { get; set; }

        public LogInResult(bool success, string message)
        {
            IsSuccessful = success;
            Message = message;
        }
        public LogInResult(bool success, string message, bool userValidated)
        {
            this.IsSuccessful = success;
            this.Message = message;
            this.UserValidated = userValidated;
        }
        public LogInResult(bool success, string message, bool userValidated, int idUsuario)
        {
            this.IsSuccessful = success;
            this.Message = message;
            this.UserValidated = userValidated;
            this.idUsuario = idUsuario;
        }
    }

}
