//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pedidos
    {
        public int idPedido { get; set; }
        public int idUsuario { get; set; }
        public int idCarrito { get; set; }
        public int idEstado { get; set; }
        public int idMetodo { get; set; }
        public decimal MontoPagado { get; set; }
        public System.DateTime FechaIngreso { get; set; }
        public Nullable<System.DateTime> FechaUltimoEstado { get; set; }
    }
}