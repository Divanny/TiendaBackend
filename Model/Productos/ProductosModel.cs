using Model.Common;
using Model.Productos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Model.Productos
{
    public class ProductosModel
    {
        public int idProducto { get; set; }
        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public int SumaValoraciones { get; set; }
        public int CantidadValoraciones { get; set; }
        public int Valoracion { get; set; }
        [Required]
        public decimal Precio { get; set; }
        [Required]
        public int CantidadStock { get; set; }
        public int CantidadEnCarrito { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? FechaIngreso { get; set; }
        public bool EstaActivo { get; set; }
        public IEnumerable<CategoriasModel> Categorias { get; set; }
        public int CantCategorias { get; set; }
        [FileSize(1048576, ErrorMessage = "El archivo no puede ser mayor de 1 MB.")]
        public HttpPostedFileBase Imagen { get; set; }
        public string FotoUrl { get; set; }
    }
}
