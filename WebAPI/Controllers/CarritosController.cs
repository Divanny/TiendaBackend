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
    /// <summary>
    /// API para manejar los carritos de los usuarios.
    /// </summary>
    [RoutePrefix("api/Carritos")]
    public class CarritosController : ApiBaseController
    {
        TiendaDBEntities dbContext = new TiendaDBEntities();
        CarritosRepo carritosRepo = new CarritosRepo();

        /// <summary>
        /// Obtiene un listado de todos los carritos.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public List<CarritosModel> Get()
        {
            List<CarritosModel> carritos = carritosRepo.Get().ToList();
            return carritos;
        }

        /// <summary>
        /// Obtiene la información de un carrito en específico.
        /// </summary>
        /// <param name="idCarrito"></param>
        /// <returns></returns>
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public CarritosModel Get(int idCarrito)
        {
            CarritosModel carritos = carritosRepo.Get(x => x.idCarrito == idCarrito).FirstOrDefault();
            return carritos;
        }

        /// <summary>
        /// Obtiene el carrito actual del usuario.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        [Route("GetActualCarrito")]
        public CarritosModel GetActualCarrito()
        {
            CarritosModel carrito = carritosRepo.Get(x => x.idUsuario == OnlineUser.GetUserId() && x.EstaTerminado == false).FirstOrDefault();
            return carrito;
        }

        /// <summary>
        /// Inserta un producto al carrito actual del usuario. (Parámetro precioPorProducto, puede ser 0 y tomará el precio actual del producto)
        /// </summary>
        /// <param name="idProducto"></param>
        /// <param name="cantidad"></param>
        /// <param name="precioPorProducto"></param>
        /// <returns></returns>
        [HttpPost]
        [Autorizar(AllowAnyProfile = true)]
        [Route("InsertarProducto")]
        public OperationResult InsertarProducto(int idProducto, int cantidad, int precioPorProducto)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CarritosModel carrito = GetActualCarrito();

                    if (carrito == null)
                    {
                        CarritosModel carritoNew = new CarritosModel()
                        {
                            idUsuario = OnlineUser.GetUserId(),
                            FechaCreacion = DateTime.Now,
                            EstaTerminado = false
                        };

                        var carritoCreated = carritosRepo.Add(carritoNew);
                        carrito = GetActualCarrito();
                    }

                    if (precioPorProducto == 0)
                    {
                        ProductosRepo productosRepo = new ProductosRepo();
                        var producto = productosRepo.Get(idProducto);
                        precioPorProducto = (int)producto.Precio; // OJO CAMBIAR A DECIMAL (precioPorProducto)
                    }

                    cantidad = (cantidad == 0) ? 1 : cantidad;

                    var result = carritosRepo.InsertarProductos(carrito.idCarrito, idProducto, cantidad, precioPorProducto);
                    trx.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    trx.Rollback();
                    return new OperationResult(false, "Error 500 - Internal server error");
                }
            }
        }

        /// <summary>
        /// Elimina un producto del carrito actual del usuario.
        /// </summary>
        /// <param name="idProducto"></param>
        /// <param name="cantidad"></param>
        /// <returns></returns>
        [HttpPost]
        [Autorizar(AllowAnyProfile = true)]
        [Route("RemoverProductos")]
        public OperationResult RemoverProductos(int idProducto, int cantidad)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CarritosModel carrito = GetActualCarrito();

                    if (carrito == null)
                    {
                        CarritosModel carritoNew = new CarritosModel()
                        {
                            idUsuario = OnlineUser.GetUserId(),
                            FechaCreacion = DateTime.Now,
                            EstaTerminado = false
                        };

                        var carritoCreated = carritosRepo.Add(carritoNew);
                        carrito = GetActualCarrito();
                    }

                    var result = carritosRepo.RemoverProductos(carrito.idCarrito, idProducto, cantidad);
                    return result;
                }
                catch (Exception ex)
                {
                    trx.Rollback();
                    return new OperationResult(false, "Error 500 - Internal server error");
                }
            }
        }
    }
}