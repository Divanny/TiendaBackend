using Model.Productos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Productos
{
    public class CarritosModel
    {
        public int idCarrito { get; set; }
        public int idUsuario { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public bool EstaTerminado { get; set; }
        public List<ProductosModel> Productos { get; set; }
    }
}
