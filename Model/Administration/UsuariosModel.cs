﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Model.Common
{
    public class UsuariosModel
    {
        public int idUsuario { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un nombre de usuario válido")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 carácteres")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un correo electrónico")]
        [MaxLength(100, ErrorMessage = "El nombre de usuario no puede exceder los 50 carácteres")]
        public string CorreoElectronico { get; set; }
        public string Password { get; set; }
        public byte[] PasswordHash { get; set; }
        public int idPerfil { get; set; }
        public string Perfil { get; set; }
        public int idEstado { get; set; }
        public string Estado { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public System.DateTime UltimoIngreso { get; set; }
        [Required(ErrorMessage = "Debe seleccionar su nombre")]
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        [FileSize(1048576, ErrorMessage = "El archivo no puede ser mayor de 1 MB.")]
        public HttpPostedFileBase FotoPerfil { get; set; }
        public string FotoUrl { get; set; }
    }
}
