using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Infraestructure
{
    public class SessionManager
    {
        public void SaveUser(int idUsuario)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["user"] = idUsuario;
            }
            else
            {
                throw new InvalidOperationException("HttpContext.Current es null.");
            }

        }

        public int GetOnlineUserId()
        {
            var a = HttpContext.Current.Session["user"];
            int onlineUserID = (int)(HttpContext.Current.Session["user"] ?? 0);
            return onlineUserID;
        }

        public void CleanSession()
        {
            HttpContext.Current.Session.Clear();
        }
    }
}