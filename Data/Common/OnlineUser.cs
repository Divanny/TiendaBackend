using System;
using System.Web;

namespace Data.Common
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