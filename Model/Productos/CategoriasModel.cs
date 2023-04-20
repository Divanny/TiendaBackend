using System;
using System.ComponentModel.DataAnnotations;

namespace Model.Productos
{
    public class CategoriasModel
    {
        public int idCategoria { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public bool PoseeCategoria { get; set; }
    }
}