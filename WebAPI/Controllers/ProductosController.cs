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

namespace WebAPI.Controllers
{
    [RoutePrefix("api/Productos")]
    public class ProductosController : ApiBaseController
    {
        ProductosRepo productosRepo = new ProductosRepo();

        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public List<ProductosModel> Get()
        {
            List<ProductosModel> productos = productosRepo.Get().ToList();
            return productos;
        }

        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public ProductosModel Get(int idProducto)
        {
            ProductosModel producto = productosRepo.Get(x => x.idProducto == idProducto).FirstOrDefault();
            return producto;
        }

        [HttpPost]
        [Autorizar(AllowAnyProfile = true)]
        public OperationResult Post(ProductosModel model)
        {
            if (ValidateModel(model))
            {
                var created = productosRepo.Add(model);
                productosRepo.Log(created);
                return new OperationResult(true, "Se ha creado satisfactoriamente", created);
            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

        [HttpPut]
        [Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idProducto, ProductosModel model)
        {
            if (ValidateModel(model))
            {
                productosRepo.Edit(model, idProducto);
                productosRepo.Log(model);
                return new OperationResult(true, "Se ha creado satisfactoriamente", model);
            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

        [Route("GetCategorias")]
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public List<CategoriasModel> GetCategorias()
        {
            List<CategoriasModel> categorias = productosRepo.GetCategorias().ToList();
            return categorias;
        }

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