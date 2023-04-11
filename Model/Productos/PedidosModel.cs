using Model.Productos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Productos
{
    public class PedidosModel
    {
        public int idPedido { get; set; }
        public int idUsuario { get; set; }
        public int idCarrito { get; set; }
        public int idEstado { get; set; }
        public string Estado { get; set; }
        public int idMetodo { get; set; }
        public string Metodo { get; set; }
        public decimal MontoPagado { get; set; }
        public System.DateTime FechaIngreso { get; set; }
        public Nullable<System.DateTime> FechaUltimoEstado { get; set; }
        public CarritosModel Carrito { get; set; }
    }
}
