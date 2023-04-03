﻿using Data.Common;
using Model.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    public class UsuariosRepo : Repository<Usuarios, UsuariosModel>
    {
        public UsuariosRepo(DbContext dbContext = null) : base
        (
            dbContext ?? new TiendaDBEntities1(),
            new ObjectsMapper<UsuariosModel, Usuarios>(u => new Usuarios()
            {
                idUsuario = u.idUsuario,
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,
                NombreUsuario = u.NombreUsuario,
                CorreoElectronico = u.CorreoElectronico,
                PasswordHash = u.PasswordHash,
                idPerfil = u.idPerfil,
                idEstado = u.idEstado,
                FechaRegistro = u.FechaRegistro,
                UltimoIngreso = u.UltimoIngreso
            }),
            (DB, filter) => from u in DB.Set<Usuarios>().Where(filter)
                            join p in DB.Set<Perfile>() on u.idPerfil equals p.idPerfil
                            join e in DB.Set<EstadosUsuario>() on u.idEstado equals e.idEstado
                            select new UsuariosModel()
                            {
                                idUsuario = u.idUsuario,
                                Nombres = u.Nombres,
                                Apellidos = u.Apellidos,
                                NombreUsuario = u.NombreUsuario,
                                CorreoElectronico = u.CorreoElectronico,
                                PasswordHash = u.PasswordHash,
                                idPerfil = u.idPerfil,
                                Perfil = p.Nombre,
                                idEstado = u.idEstado,
                                Estado = e.Nombre,
                                FechaRegistro = u.FechaRegistro,
                                UltimoIngreso = u.UltimoIngreso
                            }
        )
        { }

        public UsuariosModel GetByUsername(string nombreUsuario)
        {
            return base.Get(x => x.NombreUsuario == nombreUsuario).FirstOrDefault();
        }

        public UsuariosModel Get(int id)
        {
            var result = base.Get(a => a.idUsuario == id).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;
        }
    }
}
