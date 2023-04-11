using Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Usuarios
{
    public class MetodosPagoUsuariosModel
    {
        public int idMetodo { get; set; }
        [Required]
        public int idUsuario { get; set; }
        public string NombreUsuario { get; set; }
        [Required]
        public string Tipo { get; set; }
        [Required]
        public string Numero { get; set; }
        public Nullable<System.DateTime> FechaExpiracion { get; set; }
        [Required]
        public string CVV { get; set; }
        [Required]
        public bool EsPrincipal { get; set; }
    }
}
