﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TiendaDBEntities : DbContext
    {
        public TiendaDBEntities()
            : base("name=TiendaDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<EstadosUsuarios> EstadosUsuarios { get; set; }
        public virtual DbSet<LogActividad> LogActividad { get; set; }
        public virtual DbSet<Perfiles> Perfiles { get; set; }
        public virtual DbSet<PerfilesVistas> PerfilesVistas { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }
        public virtual DbSet<Vistas> Vistas { get; set; }
    }
}
