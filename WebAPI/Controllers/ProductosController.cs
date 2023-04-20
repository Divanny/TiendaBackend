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
using Model.Productos;
using Model.Enum;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar los productos del sistema.
    /// </summary>
    [RoutePrefix("api/Productos")]
    public class ProductosController : ApiBaseController
    {
        ProductosRepo productosRepo = new ProductosRepo();

        /// <summary>
        /// Obtiene un listado de los productos.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public List<ProductosModel> Get()
        {
            List<ProductosModel> productos = productosRepo.Get(x => x.EstaActivo == true).ToList();
            return productos;
        }
        /// <summary>
        /// Obtiene un listado de todos los productos (incluyendo inactivos.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        [Route("GetAllProducts")]
        public List<ProductosModel> GetAllProducts()
        {
            List<ProductosModel> productos = productosRepo.Get().ToList();
            return productos;
        }

        /// <summary>
        /// Obtiene un producto en específico.
        /// </summary>
        /// <param name="idProducto"></param>
        /// <returns></returns>
        [HttpGet]
        [Autorizar(VistasEnum.GestionarProductos)]
        public ProductosModel Get(int idProducto)
        {
            ProductosModel producto = productosRepo.Get(x => x.idProducto == idProducto).FirstOrDefault();
            return producto;
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Autorizar(AllowAnyProfile = true)]
        public OperationResult Post(ProductosModel model)
        {
            if (ValidateModel(model))
            {
                model.FechaIngreso = DateTime.Now;
                model.SumaValoraciones = 5;
                model.CantidadValoraciones = 1;

                var created = productosRepo.Add(model);
                productosRepo.Log(created);
                return new OperationResult(true, "Se ha creado satisfactoriamente", created);
            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

        /// <summary>
        /// Actualiza la información de un producto.
        /// </summary>
        /// <param name="idProducto"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Autorizar(VistasEnum.GestionarProductos)]
        public OperationResult Put(int idProducto, ProductosModel model)
        {
            if (ValidateModel(model))
            {
                var actualModel = productosRepo.Get(x => x.idProducto == idProducto).FirstOrDefault();
                model.FechaIngreso = actualModel.FechaIngreso;
                model.idProducto = idProducto;
                model.SumaValoraciones = actualModel.SumaValoraciones;
                model.CantidadValoraciones = actualModel.CantidadValoraciones;
                productosRepo.Edit(model);
                productosRepo.Log(model);
                return new OperationResult(true, "Se ha creado satisfactoriamente", model);
            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

        /// <summary>
        /// Elimina un producto.
        /// </summary>
        /// <param name="idProducto"></param>
        /// <returns></returns>
        [HttpDelete]
        [Autorizar(VistasEnum.GestionarProductos)]
        public OperationResult Delete(int idProducto)
        {
            try
            {
                productosRepo.Delete(idProducto);
                return new OperationResult(true, "Se ha eliminado satisfactoriamente");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, "Error al eliminar el producto");
            }
        }

        /// <summary>
        /// Obtiene todas las categorías de productos.
        /// </summary>
        /// <returns></returns>
        [Route("GetCategorias")]
        [HttpGet]
        [Autorizar(VistasEnum.GestionarProductos)]
        public List<CategoriasModel> GetCategorias()
        {
            List<CategoriasModel> categorias = productosRepo.GetCategorias().ToList();
            return categorias;
        }

        /// <summary>
        /// Le da un ranking a un producto en específico. (Del 1 al 5).
        /// </summary>
        /// <param name="idProducto"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        [Route("Valorar")]
        [HttpPut]
        [Autorizar(AllowAnyProfile = true)]
        public OperationResult Valorar(int idProducto, int valor)
        {
            if (valor <= 0 || valor > 5)
            {
                return new OperationResult(false, "El valor excede el límite");
            }

            var model = productosRepo.Get(x => x.idProducto == idProducto).FirstOrDefault();
            model.SumaValoraciones += valor;
            model.CantidadValoraciones++;

            productosRepo.Edit(model, idProducto);

            return new OperationResult(true, "El producto ha sido valorado satisfactoriamente");
        }
    }
}