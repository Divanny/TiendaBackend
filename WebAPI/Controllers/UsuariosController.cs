﻿using Data;
using Microsoft.Ajax.Utilities;
using Model;
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
    public class UsuariosController : ApiBaseController
    {
        UsuariosRepo usuariosRepo = new UsuariosRepo();
        [HttpGet]
        public List<UsuariosModel> Get()
        {
            return usuariosRepo.Get().ToList();
        }

        // GET api/Usuarios/5
        [HttpGet]
        public UsuariosModel Get(int id)
        {
            return usuariosRepo.Get(x => x.idUsuario == id).FirstOrDefault();
        }


        // POST api/Usuarios
        [HttpPost]
        public OperationResult Post([FromBody]UsuariosModel model)
        {
            if (ValidateModel(model))
            {
                UsuariosModel usuario = usuariosRepo.GetByUsername(model.NombreUsuario);
                if (usuario != null)
                {
                    return new OperationResult(false, "Este usuario ya está registrado");
                }

                model.PasswordHash  = Cryptography.Encrypt(model.Password);
                model.idEstado = (int)EstadoUsuarioEnum.Activo;
                model.FechaRegistro = DateTime.Now;
                model.UltimoIngreso = DateTime.Now;

                if (model.idPerfil == null)
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

                return new OperationResult(result.IsSuccessful, result.Message, result.UserValidated);
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        // PUT api/Usuarios/5
        public void Put(int id, [FromBody] string value)
        {

        }
    }
}