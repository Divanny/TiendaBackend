using Data;
using Model.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using WebAPI.Infraestructure;
using Model.Enum;
using Model.Productos;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar las categorias o roles del sistema.
    /// </summary>
    [RoutePrefix("api/Categorias")]
    public class CategoriasController : ApiBaseController
    {
        CategoriasRepo categoriasRepo = new CategoriasRepo();

        /// <summary>
        /// Obtiene un listado de todos las categorias del sistema.
        /// </summary>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarProductos)]
        [HttpGet]
        public List<CategoriasModel> Get()
        {
            return categoriasRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene una categoria en específico.
        /// </summary>
        /// <param name="idCategoria"></param>
        /// <returns></returns>onarPerfiles)]
        [Autorizar(VistasEnum.GestionarProductos)]
        [HttpGet]
        public CategoriasModel Get(int idCategoria)
        {
            return categoriasRepo.Get(x => x.idCategoria == idCategoria).FirstOrDefault();
        }

        /// <summary>
        /// Crea un nuevo categoria al sistema.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarProductos)]
        [HttpPost]
        public OperationResult Post(CategoriasModel model)
        {
            if (ValidateModel(model))
            {
                var ifExist = categoriasRepo.Get(x => x.Nombre == model.Nombre).FirstOrDefault();

                if (ifExist != null)
                {
                    return new OperationResult(false, "Esta categoría ya existe", Validation.Errors);
                }

                var created = categoriasRepo.Add(model);
                categoriasRepo.Log(created);
                return new OperationResult(true, "Se ha creado satisfactoriamente", created);
            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

        /// <summary>
        /// Actualiza la información de una categoría.
        /// </summary>
        /// <param name="idCategoria"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarProductos)]
        [HttpPut]
        public OperationResult Put(int idCategoria, CategoriasModel model)
        {
            if (ValidateModel(model))
            {
                categoriasRepo.Edit(model, idCategoria);
                categoriasRepo.Log(model);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente", model);
            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

        /// <summary>
        /// Elimina una categoría.
        /// </summary>
        /// <param name="idCategoria"></param>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarProductos)]
        [HttpDelete]
        public OperationResult Delete(int idCategoria)
        {
            try
            {
                categoriasRepo.Delete(idCategoria);
                categoriasRepo.Log(idCategoria);
                return new OperationResult(true, "Se ha eliminado satisfactoriamente");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, "No se ha podido eliminar este perfil");
            }
        }
    }
}