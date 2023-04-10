using Data;
using Data.Common;
using Microsoft.Ajax.Utilities;
using Model;
using Model.Common;
using Model.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPI.Infraestructure;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/Usuarios")]
    public class UsuariosController : ApiBaseController
    {
        UsuariosRepo usuariosRepo = new UsuariosRepo();

        // GET api/Usuarios
        [HttpGet]
        [Autorizar(VistasEnum.GestionarUsuarios)]
        public List<UsuariosModel> Get()
        {
            return usuariosRepo.Get().ToList();
        }

        // GET api/Usuarios/5
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public UsuariosModel Get(int id)
        {
            return usuariosRepo.Get(x => x.idUsuario == id).FirstOrDefault();
        }

        // POST api/Usuarios
        [HttpPost]
        [Autorizar(VistasEnum.GestionarUsuarios)]
        public OperationResult Post([FromBody]UsuariosModel model)
        {
            if (ValidateModel(model))
            {
                if (model.Password == null || model.Password == "")
                {
                    return new OperationResult(false, "Se debe colocar una contraseña válida", Validation.Errors);
                }

                UsuariosModel usuario = usuariosRepo.GetByUsername(model.NombreUsuario);
                if (usuario != null)
                {
                    return new OperationResult(false, "Este usuario ya está registrado");
                }

                model.PasswordHash  = Cryptography.Encrypt(model.Password);
                model.idEstado = (int)EstadoUsuarioEnum.Activo;
                model.FechaRegistro = DateTime.Now;
                model.UltimoIngreso = DateTime.Now;

                if (model.idPerfil == 0)
                {
                    model.idPerfil = (int)PerfilesEnum.Cliente;
                }

                var created = usuariosRepo.Add(model);
                usuariosRepo.SaveChanges();
                return new OperationResult(true, "Se creado este usuario satisfactoriamente", created);
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        // POST api/Usuarios/Registrar
        [HttpPost]
        [Route("Registrar")]
        public OperationResult Registrar([FromBody] UsuariosModel model)
        {
            if (ValidateModel(model))
            {
                UsuariosModel usuario = usuariosRepo.GetByUsername(model.NombreUsuario);
                if (usuario != null)
                {
                    return new OperationResult(false, "Este usuario ya está registrado");
                }

                if (model.Password == null || model.Password == "")
                {
                    return new OperationResult(false, "Se debe colocar una contraseña válida", Validation.Errors);
                }

                Utilities utilities = new Utilities();
                var PasswordValidation = utilities.ValidarContraseña(model.Password);

                if (!PasswordValidation.Success)
                {
                    return PasswordValidation;
                }

                model.PasswordHash = Cryptography.Encrypt(model.Password);
                model.idEstado = (int)EstadoUsuarioEnum.Activo;
                model.FechaRegistro = DateTime.Now;
                model.UltimoIngreso = DateTime.Now;

                if (model.idPerfil == 0)
                {
                    model.idPerfil = (int)PerfilesEnum.Cliente;
                }

                usuariosRepo.Add(model);
                usuariosRepo.SaveChanges();

                Credentials credentials = new Credentials()
                {
                    userName = model.NombreUsuario,
                    password = model.Password
                };

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

                if (result.IsSuccessful && result.UserValidated)
                {
                    result.Message = ("Se ha registrado existosamente, bienvenido.");
                }

                return new OperationResult(result.IsSuccessful, result.Message, result.UserValidated, result.Token);
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        // PUT api/Usuarios/5
        [Autorizar(AllowAnyProfile = true)]
        [HttpPut]
        public OperationResult Put(int idUsuario, [FromBody] UsuariosModel model)
        {
            if (ValidateModel(model))
            {
                UsuariosModel usuario = usuariosRepo.Get(x => x.idUsuario == idUsuario).FirstOrDefault();

                if (usuario == null)
                {
                    return new OperationResult(false, "Este usuario no existe.");
                }

                var usuarioExists = usuariosRepo.Get(x => x.NombreUsuario == model.NombreUsuario).FirstOrDefault();

                if (usuarioExists != null) 
                { 
                    if (usuarioExists.idUsuario != idUsuario)
                    {
                        return new OperationResult(false, "Este usuario ya está registrado");
                    }                
                }


                if (model.Password != null && model.Password != "")
                {
                    Utilities utilities = new Utilities();
                    var PasswordValidation = utilities.ValidarContraseña(model.Password);

                    if (!PasswordValidation.Success)
                    {
                        return PasswordValidation;
                    }

                    model.PasswordHash = Cryptography.Encrypt(model.Password);
                }
                else
                {
                    model.PasswordHash = usuario.PasswordHash;
                }

                model.UltimoIngreso = DateTime.Now;

                usuariosRepo.Edit(model, idUsuario);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        // GET api/Usuarios/GetEstadosUsuarios
        [Route("GetEstadosUsuarios")]
        [HttpGet]
        [Autorizar(VistasEnum.GestionarUsuarios)]
        public object GetEstadosUsuarios()
        {
            return usuariosRepo.GetEstadosUsuarios();
        }

        // GET api/Usuarios/GetReporteUsuarios
        [Route("GetReporteUsuarios")]
        [HttpGet]
        //[Autorizar(VistasEnum.GestionarUsuarios)]
        public HttpResponseMessage GetReporteUsuarios()
        {
            var reportePdf = usuariosRepo.GenerarReporteUsuarios();

            // Devolver el reporte PDF como un archivo descargable
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(reportePdf)
            };
            result.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"Reporte de Usuarios — {DateTime.Now}.pdf"
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/pdf");

            return result;
        }
    }
}