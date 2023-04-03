using Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Administration
{
    public class PerfilesModel
    {
        public int idPerfil { get; set; }
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [StringLength(250)]
        public string Descripcion { get; set; }
        public int CantPermisos { get; set; }
        public bool PorDefecto { get; set; }
        public IEnumerable<VistasModel> Vistas { get; set; }
        public IEnumerable<UsuariosModel> Usuarios { get; set; }
    }
}
