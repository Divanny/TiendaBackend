using System;
using System.Web;

namespace WebAPI.Infraestructure
{
    public class OnlineUser
    {
        public static int GetUserId()
        {
            var userID = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
            return userID;
        }
    }
}