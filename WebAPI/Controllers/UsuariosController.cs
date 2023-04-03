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
    [RoutePrefix("api/Usuarios")]
    public class UsuariosController : ApiController
    {
        SessionManager session = new SessionManager();
        UsuariosRepo usuariosRepo = new UsuariosRepo();
        [HttpGet]
        public List<Usuarios> Get()
        {
            return usuariosRepo.GetAll();
        }

        // GET api/Usuarios/5
        [HttpGet]
        public Usuarios Get(int id)
        {
            return usuariosRepo.GetByID(id);
        }


        // POST api/Usuarios
        [HttpPost]
        public OperationResult Post(UsuariosModel model)
        {
            if (ModelState.IsValid)
            {
                Usuarios usuario = usuariosRepo.GetByUsername(model.NombreUsuario);

                if (usuario != null)
                {
                    return new OperationResult(false, "Este usuario ya está registrado");
                }

                model.PasswordHash = Cryptography.Encrypt(model.Password);
                model.FechaRegistro = DateTime.Now;
                model.UltimoIngreso = DateTime.Now;

                var objectResult = usuariosRepo.Add(model);
                if (objectResult != null)
                {
                    InsertarUsuario_Result created = objectResult.ElementAt(0);
                    session.SaveUser(created.idUsuario);
                    return new OperationResult(true, "Se ha registrado satisfactoriamente", created);
                }
                else
                {
                    return new OperationResult(false, "Error al registrar el usuario");
                }
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos");
            }
        }

        // PUT api/Usuarios/5
        public void Put(int id, [FromBody] string value)
        {

        }
        [HttpPut]
        [Route("LogIn")]
        public LogInResult LogIn([FromBody] UsuariosModel logIn)
        {
            Usuarios usuario = usuariosRepo.GetByUsername(logIn.NombreUsuario);

            if (usuario == null)
            {
                return new LogInResult(false, "Usuario no encontrado");
            }

            logIn.PasswordHash = Cryptography.Encrypt(logIn.Password);
            bool passwordMatch = Cryptography.CompareByteArrays(logIn.PasswordHash, usuario.PasswordHash);

            if ((logIn.NombreUsuario == usuario.NombreUsuario && passwordMatch) && usuario.Estado == (int)EstadoUsuarioEnum.Activo)
            {
                session.SaveUser(usuario.idUsuario);
                return new LogInResult(true, "Ha iniciado sesión satisfactoriamente");
            }
            else
            {
                if (usuario.Estado != (int)EstadoUsuarioEnum.Activo)
                {
                    return new LogInResult(false, "Esta usuario está inactivado");
                }

                return new LogInResult(false, "Usuario y/o contraseña inválido");
            }
        }

        [HttpPut]
        [Route("LogOut")]
        public LogInResult LogOut()
        {
            try
            {
                session.CleanSession();
                return new LogInResult(true, "Ha cerrado sesión satisfactoriamente");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return new LogInResult(false, "Error al cerrar sesión");
            }
        }

        [HttpGet]
        [Route("GetOnlineUser")]
        public Usuarios GetOnlineUser()
        {
            if (session.GetOnlineUserId() != 0)
            {
                Usuarios usuario = usuariosRepo.GetByID(session.GetOnlineUserId());
                return usuario;
            }
            return null;
        }
    }
}