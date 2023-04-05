using System.Web;

namespace Data.Common
{
    public class OnlineUser
    {
        public static int GetUserId()
        {
            var a = HttpContext.Current.User.Identity.Name;
            return 1;
        }
    }
}