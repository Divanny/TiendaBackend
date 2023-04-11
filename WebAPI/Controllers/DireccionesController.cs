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
    /// API para manejar las direcciones de entrega de los usuarios.
    /// </summary>
    [RoutePrefix("api/Direcciones")]
    public class DireccionesController : ApiBaseController
    {
        DireccionesRepo direccionesRepo = new DireccionesRepo();

        /// <summary>
        /// Obtiene todas las direcciones existentes.
        /// </summary>
        /// <returns></returns>
        // GET api/Direcciones
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public List<DireccionesModel> Get()
        {
            return direccionesRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene una dirección específica.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Direcciones/5
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public DireccionesModel Get(int id)
        {
            return direccionesRepo.Get(x => x.idDireccion == id).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las direcciones del usuario en línea o usuario en específico.
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        // GET api/Direcciones/5
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        [Route("GetDireccionesUsuario")]
        public List<DireccionesModel> GetDireccionesUsuario(int? idUsuario)
        {
            if (idUsuario == 0 || idUsuario == null)
            {
                idUsuario = Infraestructure.OnlineUser.GetUserId();
            }

            return direccionesRepo.Get(x => x.idUsuario == idUsuario).ToList();
        }

        /// <summary>
        /// Crea una nueva dirección.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Direcciones
        [HttpPost]
        [Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody]DireccionesModel model)
        {
            if (ValidateModel(model))
            {
                var created = direccionesRepo.Add(model);
                return new OperationResult(true, "Se ha guardado esta dirección satisfactoriamente", created);
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Actualiza una dirección.
        /// </summary>
        /// <param name="idDireccion"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Direcciones/5
        [HttpPut]
        [Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idDireccion, [FromBody]DireccionesModel model)
        {
            model.idUsuario = (model.idUsuario == 0) ? Infraestructure.OnlineUser.GetUserId() : model.idUsuario;

            if (ValidateModel(model))
            {
                direccionesRepo.Edit(model, idDireccion);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Elimina una dirección.
        /// </summary>
        /// <param name="idDireccion"></param>
        /// <returns></returns>
        // DELETE api/Direcciones/5
        [HttpDelete]
        [Autorizar(AllowAnyProfile = true)]
        public OperationResult Delete(int idDireccion)
        {
            try
            {
                direccionesRepo.Delete(idDireccion);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
            }
            catch
            {
                return new OperationResult(false, "No se ha podido eliminar esta dirección, intentelo nuevamente.");
            }
        }
    }
}