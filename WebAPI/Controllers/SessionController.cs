using Data;
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
    [RoutePrefix("api/Session")]
    public class SessionController : ApiController
    {
        SessionManager session = new SessionManager();
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
    }

}