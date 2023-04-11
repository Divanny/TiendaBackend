using Data;
using Data.Common;
using Microsoft.Ajax.Utilities;
using Model.Common;
using Model.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using WebAPI.Infraestructure;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar la sesión actual del sistema.
    /// </summary>
    [RoutePrefix("api/Session")]
    public class SessionController : ApiController
    {
        /// <summary>
        /// Obtiene el estado de la sesión.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSessionState")]
        public IHttpActionResult GetSessionState()
        {
            var session = HttpContext.Current.Session;
            if (session == null)
            {
                return NotFound();
            }
            var sessionData = new
            {
                SessionID = session.SessionID,
                Timeout = session.Timeout,
                IsNewSession = session.IsNewSession,
                Count = session.Count,
                Keys = session.Keys
            };
            return Ok(sessionData);
        }

        /// <summary>
        /// Obtiene la información del usuario en línea.
        /// </summary>
        /// <returns></returns>
        [Autorizar(AllowAnyProfile = true)]
        [HttpGet]
        [Route("GetOnlineUser")]
        public UsuariosModel GetOnlineUser()
        {
            UsuariosRepo usuariosRepo = new UsuariosRepo();
            var model = usuariosRepo.Get(x => x.idUsuario == Infraestructure.OnlineUser.GetUserId()).FirstOrDefault();

            return model;
        }
    }

}