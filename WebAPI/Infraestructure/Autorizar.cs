using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using Model.Enum;
using Data;

namespace WebAPI.Infraestructure
{
    /// <summary>
    /// Sirve para autorizar el acceso a las rutas de los controladores de la aplicacion 
    /// </summary>
    public class AutorizarAttribute : System.Web.Http.AuthorizeAttribute
    {
        public VistasEnum[] AppProfiles { get; set; }
        public bool AllowAnyProfile { get; set; }
        EstadoSesion sesionStatus;
        public AutorizarAttribute(params VistasEnum[] vistas)
        {
            AppProfiles = vistas;
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!IsAuthorized(actionContext))
                HandleUnauthorizedRequest(actionContext);
        }
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //Se obtiene el estado de sesion desde la cookie o el token
            var sesionInfo = SessionData.Get();
            sesionStatus = sesionInfo.Estado;
            //Se establese el id de usuario en el HttpContext
            HttpContext.Current.User = new Model.Principal() { Identity = new Model.Identity() { Name = sesionInfo.idUsuario } };

            //Si la sesion esta iniciada, se valida que el usuario tiene acceso a la vista (rol)
            if (sesionStatus == EstadoSesion.Iniciada)
            {
                //Si se admite el paso no importando el rol se retorna true
                if (AllowAnyProfile) return true;
                //Si no se han definido roles se arroja un exception
                if (AppProfiles == null || AppProfiles.Length == 0) throw new Exception("No se ha definido ninguna regla para autorizar, ya sea 'Profiles' o 'AllowAnyProfile'");
                //Se valida que el usuario tiene permiso para acceder a la vista (rol)
                using (PerfilesRepo PR = new PerfilesRepo())
                {
                    return PR.CanAccess(Convert.ToInt32(sesionInfo.idUsuario), AppProfiles.Select(a => (int)a).ToArray());
                }
            }
            else
                return false;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            //Si la sesion no esta iniciada se retorna 401 (no autorizado)
            if (sesionStatus == EstadoSesion.NoIniciada)
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            //Si la sesión está iniciada y aún así no se autorizó, significa que el recurso está prohibido 403
            else
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
        }
    }
}