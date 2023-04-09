using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Common
{
    public class Logger : IDisposable
    {
        private DbContext dbContext { get; set; }
        public Logger(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void LogHttpRequest(object data)
        {
            //LogActividad log = new LogActividad()
            //{
            //    URL = HttpContext.Current.Request.RawUrl.ToString(),
            //    idUsuario = OnlineUser.GetUserId(),
            //    Metodo = HttpContext.Current.Request.HttpMethod.ToString(),
            //    Fecha = DateTime.Now,
            //    Data = data == null ? String.Empty : Newtonsoft.Json.JsonConvert.SerializeObject(data)
            //};

            //dbContext.Set<LogActividad>().Add(log);
            //dbContext.SaveChanges();
        }
        public void LogHttpRequest(int idUsuario, object data)
        {
            //LogActividad log = new LogActividad()
            //{
            //    URL = HttpContext.Current.Request.RawUrl.ToString(),
            //    idUsuario = idUsuario,
            //    Metodo = HttpContext.Current.Request.HttpMethod.ToString(),
            //    Fecha = DateTime.Now,
            //    Data = data == null ? String.Empty : Newtonsoft.Json.JsonConvert.SerializeObject(data)
            //};
            //LogHttpRequest(log);
        }
        public void LogHttpRequest(LogActividad log)
        {
            dbContext.Set<LogActividad>().Add(log);
            dbContext.SaveChanges();
        }

        public void Dispose()
        {
            this.dbContext.Dispose();
        }
    }
}
