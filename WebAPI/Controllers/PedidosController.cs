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
using static iTextSharp.text.pdf.AcroFields;
using System.Data.Entity;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;
using Data.Common;
using System.Net.Http.Headers;
using System.Net;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar los Pedidos o roles del sistema.
    /// </summary>
    [RoutePrefix("api/Pedidos")]
    public class PedidosController : ApiBaseController
    {
        TiendaDBEntities dbContext = new TiendaDBEntities();
        PedidosRepo pedidosRepo = new PedidosRepo();
        CarritosRepo carritosRepo = new CarritosRepo();
        /// <summary>
        /// Obtiene un listado de todos los pedidos.
        /// </summary>
        /// <returns></returns>
        [Autorizar(AllowAnyProfile = true)]
        [HttpGet]
        public List<PedidosModel> Get()
        {
            var pedidos = pedidosRepo.Get().ToList();

            foreach (var item in pedidos)
            {
                item.Carrito = carritosRepo.Get(x => x.idCarrito == item.idCarrito).FirstOrDefault();
            }

            return pedidos;
        }

        /// <summary>
        /// Obtiene un pedido, junto a sus detalles.
        /// </summary>
        /// <returns></returns>
        [Autorizar(AllowAnyProfile = true)]
        [HttpGet]
        public PedidosModel Get(int idPedido)
        {
            var pedido = pedidosRepo.Get(x => x.idPedido == idPedido).FirstOrDefault();
            pedido.Carrito = carritosRepo.Get(x => x.idCarrito == pedido.idCarrito).FirstOrDefault();

            return pedido;
        }

        /// <summary>
        /// Obtiene los pedidos de un usuario, junto a sus detalles.
        /// </summary>
        /// <returns></returns>
        [Autorizar(AllowAnyProfile = true)]
        [HttpGet]
        [Route("GetPedidosUsuarios")]
        public List<PedidosModel> GetPedidosUsuarios(int idUsuario)
        {
            var pedidos = pedidosRepo.Get(x => x.idUsuario == idUsuario).ToList();

            foreach (var item in pedidos)
            {
                item.Carrito = carritosRepo.Get(x => x.idCarrito == item.idCarrito).FirstOrDefault();
            }

            return pedidos;
        }

        /// <summary>
        /// Genera la factura como PDF de un pedido en específico (AUN NO ESTA FUNCIONANDO).
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Autorizar(AllowAnyProfile = true)]
        [HttpGet]
        [Route("GetFactura")]
        public HttpResponseMessage GetFactura(int idPedido)
        {
            var pedido = pedidosRepo.Get(x => x.idPedido == idPedido).FirstOrDefault();
            var facturaPdf = pedidosRepo.GenerarFactura(idPedido);

            // Devolver el reporte PDF como un archivo descargable
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(facturaPdf)
            };
            result.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"Factura de pedido #{pedido.idPedido}.pdf"
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/pdf");

            return result;
        }

        /// <summary>
        /// Paga el carrito actual del usuario.
        /// </summary>
        /// <returns></returns>
        [Autorizar(AllowAnyProfile = true)]
        [HttpPost]
        [Route("Pagar")]
        public OperationResult Pagar(int idUsuario, int idMetodoPago, int idDireccion)
        {
            if (idUsuario == 0)
            {
                idUsuario = Infraestructure.OnlineUser.GetUserId();
            }

            CarritosModel carrito = carritosRepo.Get(x => x.idUsuario == Infraestructure.OnlineUser.GetUserId() && x.EstaTerminado == false).FirstOrDefault();

            decimal montoTotal = 0;
            if (carrito != null)
            {
                var carritoProductos = dbContext.CarritosProductos.Where(cp => cp.idCarrito == carrito.idCarrito).ToList();
                foreach (var cp in carritoProductos)
                {
                    montoTotal += cp.PrecioPorProducto * cp.Cantidad;
                }
            }

            PedidosModel pedido = new PedidosModel()
            {
                idUsuario = idUsuario,
                idCarrito = carrito.idCarrito,
                idEstado = 1,
                idMetodo = idMetodoPago,
                MontoPagado = montoTotal,
                FechaIngreso = DateTime.Now,
                FechaUltimoEstado = DateTime.Now,
                idDireccion = idDireccion,
            };

            // LOGICA DE INTENTO DE PAGO

            // FIN DE LOGICA DE INTENTO DE PAGO, SI ES TRUE REGISTRA EL PEDIDO

            if (true)
            {

                using (var trx = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        carrito.EstaTerminado = true;
                        carritosRepo.Edit(carrito, carrito.idCarrito);

                        CarritosModel carritoNew = new CarritosModel()
                        {
                            idUsuario = idUsuario,
                            FechaCreacion = DateTime.Now,
                            EstaTerminado = false
                        };

                        var carritoCreated = carritosRepo.Add(carritoNew);

                        var created = pedidosRepo.Add(pedido);

                        Mailing mailing = new Mailing();
                        mailing.SendFacturaMail(created);

                        return new OperationResult(true, "Se ha procesado el pago, gracias por su compra.", created);
                    }
                    catch
                    {
                        return new OperationResult(false, "Error al procesar el pago, intentelo nuevamente.");
                    }
                }
            }
            else
            {
                return new OperationResult(false, "Error al procesar el pago, intentelo nuevamente.");
            }
        }

        /// <summary>
        /// Cambia el estado actual que tiene un pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idEstado"></param>
        /// <returns></returns>
        [Autorizar(AllowAnyProfile = true)]
        [HttpPut]
        [Route("CambiarEstado")]
        public OperationResult CambiarEstatus(int idPedido, int idEstado)
        {
            if (idPedido != 0 && idEstado != 0)
            {
                PedidosModel pedido = pedidosRepo.Get(x => x.idPedido == idPedido).FirstOrDefault();
                pedido.idEstado = idEstado;

                pedidosRepo.Edit(pedido, idPedido);

                return new OperationResult(true, "Se ha cambiado el estado del pedido satisfactoriamente");
            }
            else
            {
                return new OperationResult(false, "Error al cambiar el estado del pedido.");
            }
        }

        /// <summary>
        /// Obtiene los estados que puede tener un pedido. 
        /// </summary>
        /// <returns></returns>
        [Autorizar(AllowAnyProfile = true)]
        [HttpPut]
        [Route("GetEstadosProductos")]
        public List<EstadosPedidos> GetEstadosProductos()
        {
            return dbContext.Set<EstadosPedidos>().ToList();
        }
    }
}