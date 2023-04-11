using Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Usuarios
{
    public class DireccionesModel
    {
        public int idDireccion { get; set; }
        [Required]
        public int idUsuario { get; set; }
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "Debe seleccionar una dirección válida")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "Debe seleccionar una ciudad válida")]
        public string Ciudad { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un código postal válido")]
        public string CodigoPostal { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un pais válido")]
        public string Pais { get; set; }
        [Required]
        public bool EsPrincipal { get; set; }
    }
}
