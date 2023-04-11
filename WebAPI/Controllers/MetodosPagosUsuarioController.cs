using Data;
using Data.Common;
using Microsoft.Ajax.Utilities;
using Model;
using Model.Usuarios;
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
    /// <summary>
    /// API para manejar los métodos de pago de los usuarios.
    /// </summary>
    [RoutePrefix("api/MetodosPagos")]
    public class MetodosPagosController : ApiBaseController
    {
        MetodosPagosRepo metodosPagosRepo = new MetodosPagosRepo();

        /// <summary>
        /// Obtiene todas los métodos de pago existentes.
        /// </summary>
        /// <returns></returns>
        // GET api/MetodosPagos
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public List<MetodosPagoUsuariosModel> Get()
        {
            return metodosPagosRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene un método de pago específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/MetodosPagos/5
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public MetodosPagoUsuariosModel Get(int id)
        {
            return metodosPagosRepo.Get(x => x.idMetodo == id).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los métodos de pago del usuario en línea o usuario en específico.
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        // GET api/MetodosPagos/5
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        [Route("GetMetodosUsuario")]
        public List<MetodosPagoUsuariosModel> GetMetodosUsuario(int? idUsuario)
        {
            if (idUsuario == 0 || idUsuario == null)
            {
                idUsuario = Infraestructure.OnlineUser.GetUserId();
            }

            var metodosPagos = metodosPagosRepo.Get(x => x.idUsuario == idUsuario).ToList();

            foreach (var metodoPago in metodosPagos)
            {
                metodoPago.Numero = Cipher.Decrypt(metodoPago.Numero, Properties.Settings.Default.JwtSecret);
                metodoPago.CVV = Cipher.Decrypt(metodoPago.CVV, Properties.Settings.Default.JwtSecret);
            }

            return metodosPagos;
        }

        /// <summary>
        /// Crea un nuevo método de pago.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/MetodosPagos
        [HttpPost]
        [Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody] MetodosPagoUsuariosModel model)
        {
            if (ValidateModel(model))
            {
                model.Numero = Cipher.Encrypt(model.Numero, Properties.Settings.Default.JwtSecret);
                model.CVV = Cipher.Encrypt(model.Numero, Properties.Settings.Default.JwtSecret);
                var created = metodosPagosRepo.Add(model);
                return new OperationResult(true, "Se ha guardado esta dirección satisfactoriamente", created);
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Actualiza un método de pago.
        /// </summary>
        /// <param name="idMetodo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/MetodosPagos/5
        [HttpPut]
        [Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idMetodo, [FromBody]MetodosPagoUsuariosModel model)
        {
            model.idUsuario = (model.idUsuario == 0) ? Infraestructure.OnlineUser.GetUserId() : model.idUsuario;

            if (ValidateModel(model))
            {
                model.Numero = Cipher.Encrypt(model.Numero, Properties.Settings.Default.JwtSecret);
                model.CVV = Cipher.Encrypt(model.Numero, Properties.Settings.Default.JwtSecret);
                metodosPagosRepo.Edit(model, idMetodo);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Elimina un método de pago.
        /// </summary>
        /// <param name="idMetodo"></param>
        /// <returns></returns>
        // DELETE api/MetodosPagos/5
        [HttpDelete]
        [Autorizar(AllowAnyProfile = true)]
        public OperationResult Delete(int idMetodo)
        {
            try
            {
                metodosPagosRepo.Delete(idMetodo);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
            }
            catch
            {
                return new OperationResult(false, "No se ha podido eliminar este método de pago, intentelo nuevamente.");
            }
        }
    }
}