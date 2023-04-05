using Data;
using Model.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebAPI.Infraestructure;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiBaseController
    {
        [Route("GetUserData")]
        [Autorizar(AllowAnyProfile = true)]
        public object Get()
        {
            List<VistasModel> vistas = new List<VistasModel>();
            UsuariosModel usuario = new UsuariosModel();

            var status = SessionData.Get();

            if (status.Estado == EstadoSesion.NoIniciada)
            {
                return new
                {
                    usuario = usuario,
                    vistas = vistas
                };
            }

            using (UsuariosRepo ur = new UsuariosRepo())
            {
                usuario = ur.Get(Convert.ToInt32(status.idUsuario));
            }

            using (PerfilesRepo pr = new PerfilesRepo())
            {
                vistas = pr.GetPermisos(Convert.ToInt32(status.idPerfil)).Where(v => v.Permiso).ToList();
            }

            return new
            {
                usuario = usuario,
                vistas = vistas
            };
        }
        [Route("LogIn")]
        [HttpPost]
        public OperationResult Post(Credentials credentials)
        {
            if (ValidateModel(credentials))
            {
                Authentication auth = new Authentication();
                var result = auth.LogIn(credentials);

                if (result.IsSuccessful)
                {
                    using (var dbc = new TiendaDBEntities())
                    {
                        var logger = new Data.Common.Logger(dbc);
                        logger.LogHttpRequest(result.idUsuario, null);
                    }
                }

                return new OperationResult(result.IsSuccessful, result.Message, result.UserValidated);
            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }
        [Route("MyProfile")]
        [Autorizar(AllowAnyProfile = true)]
        [HttpGet]
        public object MyProfile(string userName)
        {
            using (UsuariosRepo ur = new UsuariosRepo())
            {
                var usuario = ur.GetFirst(u => u.NombreUsuario == userName);
                return usuario;
            };
        }

        [Route("LogOff")]
        public bool Put()
        {
            SessionData.Clear();
            return true;
        }
    }
}