using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Infraestructure
{
    public class UserSesionInfo
    {
        public EstadoSesion Estado { get; set; }
        public string idUsuario { get; set; }
        public string idPerfil { get; set; }
    }
    public enum EstadoSesion
    {
        NoIniciada = 0,
        Iniciada = 1,
        NoAutorizado = 2
    }
    /// <summary>
    /// Maneja los datos del usuario en sesion
    /// </summary>
    public static class SessionData
    {
        private static int Duration = Properties.Settings.Default.SessionDuration;
        private const string key = "t1$3nd4";
        public static void Set(UserSesionInfo info)
        {
            //Backend use cookie
            string value = String.Format("{0}|{1}|{2}", info.idUsuario, info.idPerfil, RandomString(8));
            HttpCookie cookie = new HttpCookie("SID", Cipher.Encrypt(value, key)) { HttpOnly = true, Expires = DateTime.Now.AddMinutes(Duration), Path = HttpContext.Current.Request.ApplicationPath };
            HttpContext.Current.Response.Cookies.Add(cookie);

            //Frontend use cookie
            string value2 = DateTime.Now.AddMinutes(Duration).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
            HttpCookie cookie2 = new HttpCookie("SE", value2) { HttpOnly = false, Expires = DateTime.Now.AddMinutes(Duration), Path = HttpContext.Current.Request.ApplicationPath };
            HttpContext.Current.Response.Cookies.Add(cookie2);
        }
        public static UserSesionInfo Get()
        {
            var cookie = HttpContext.Current.Request.Cookies.Get("SID");
            string value;

            UserSesionInfo info = new UserSesionInfo();
            if (cookie == null || cookie.Value == "")
            {
                info.Estado = EstadoSesion.NoIniciada;
                return info;
            }

            try
            {
                value = Cipher.Decrypt(cookie.Value, key);
                info.Estado = EstadoSesion.Iniciada;
                info.idUsuario = value.Split('|')[0];
                info.idPerfil = value.Split('|')[1];

                UpdateSesionEnd(cookie.Value);
            }
            catch (Exception ex)
            {
                info.Estado = EstadoSesion.NoIniciada; return info;
            }
            return info;
        }
        private static void UpdateSesionEnd(string cookieValue)
        {
            //Backend use cookie
            HttpCookie cookie = new HttpCookie("SID", cookieValue) { HttpOnly = true, Expires = DateTime.Now.AddMinutes(Duration), Path = HttpContext.Current.Request.ApplicationPath };
            HttpContext.Current.Response.Cookies.Add(cookie);

            //Frontend use cookie
            string value2 = DateTime.Now.AddMinutes(Duration).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
            HttpCookie cookie2 = new HttpCookie("SE", value2) { HttpOnly = false, Expires = DateTime.Now.AddMinutes(Duration), Path = HttpContext.Current.Request.ApplicationPath };
            HttpContext.Current.Response.Cookies.Add(cookie2);
        }
        public static void Clear()
        {
            HttpCookie cookie = new HttpCookie("SID", "") { HttpOnly = true, Expires = new DateTime(1970, 1, 1, 0, 0, 0), Path = HttpContext.Current.Request.ApplicationPath };
            HttpContext.Current.Response.Cookies.Add(cookie);

            HttpCookie cookie2 = new HttpCookie("SE", "") { HttpOnly = true, Expires = new DateTime(1970, 1, 1, 0, 0, 0), Path = HttpContext.Current.Request.ApplicationPath };
            HttpContext.Current.Response.Cookies.Add(cookie2);
        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}