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
using Data.Common;

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
        [Autorizar(VistasEnum.GestionarCategorias)]
        [HttpPost]
        public OperationResult Post(CategoriasModel model)
        {
            try
            {
                if (ValidateModel(model))
                {
                    var ifExist = categoriasRepo.Get(x => x.Nombre == model.Nombre).FirstOrDefault();

                    if (ifExist != null)
                    {
                        return new OperationResult(false, "Esta categoría ya existe", Validation.Errors);
                    }

                    model.FechaIngreso = DateTime.Now;
                    var created = categoriasRepo.Add(model);
                    categoriasRepo.Log(created);
                    return new OperationResult(true, "Se ha creado satisfactoriamente", created);
                }
                else
                    return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
            }
            catch (Exception ex)
            {
                categoriasRepo.LogError(ex);
                return new OperationResult(false, "Error 500 - Internal server error");
            }
        }

        /// <summary>
        /// Actualiza la información de una categoría.
        /// </summary>
        /// <param name="idCategoria"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarCategorias)]
        [HttpPut]
        public OperationResult Put(int idCategoria, CategoriasModel model)
        {
            try
            {
                if (ValidateModel(model))
                {
                    var actualModel = categoriasRepo.Get(x => x.idCategoria == idCategoria).FirstOrDefault();
                    model.FechaIngreso = actualModel.FechaIngreso;
                    model.idCategoria = idCategoria;

                    categoriasRepo.Edit(model, idCategoria);
                    categoriasRepo.Log(model);
                    return new OperationResult(true, "Se ha actualizado satisfactoriamente", model);
                }
                else
                    return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
            }
            catch (Exception ex)
            {
                categoriasRepo.LogError(ex);
                return new OperationResult(false, "Error 500 - Internal server error");
            }
        }

        /// <summary>
        /// Elimina una categoría.
        /// </summary>
        /// <param name="idCategoria"></param>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarCategorias)]
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
                categoriasRepo.LogError(ex);
                return new OperationResult(false, "No se ha podido eliminar esta categoría");
            }
        }
    }
}