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
    [RoutePrefix("api/Carritos")]
    public class CarritosController : ApiBaseController
    {
        CarritosRepo carritosRepo = new CarritosRepo();

        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public List<CarritosModel> Get()
        {
            List<CarritosModel> carritos = carritosRepo.Get().ToList();
            return carritos;
        }

        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public CarritosModel Get(int idCarrito)
        {
            CarritosModel carritos = carritosRepo.Get(x => x.idCarrito == idCarrito).FirstOrDefault();
            return carritos;
        }

        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        [Route("GetActualCarrito")]
        public CarritosModel GetActualCarrito()
        {
            CarritosModel carrito = carritosRepo.Get(x => x.idUsuario == OnlineUser.GetUserId() && x.EstaTerminado == false).FirstOrDefault();
            return carrito;
        }

        [HttpPost]
        [Autorizar(AllowAnyProfile = true)]
        [Route("InsertarProducto")]
        public OperationResult InsertarProducto(int idProducto, int cantidad, int precioPorProducto)
        {
            CarritosModel carrito = GetActualCarrito();
            var result = carritosRepo.InsertarProductos(carrito.idCarrito, idProducto, cantidad, precioPorProducto);
            return result;
        }
    }
}