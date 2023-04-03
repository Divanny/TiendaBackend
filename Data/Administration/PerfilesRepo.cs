using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Data.Common;
using Model.Administration;
using Model.Common;
using Model.Enum;

namespace Data
{
    public class PerfilesRepo: Repository<Perfile, PerfilesModel>
    {
        public PerfilesRepo(DbContext dbContext = null) : base
            (
                dbContext ?? new TiendaDBEntities1(),
                new ObjectsMapper<PerfilesModel, Perfile>(p => new Perfile() {
                    Descripcion = p.Descripcion,
                    idPerfil = p.idPerfil,
                    Nombre = p.Nombre,
                    PorDefecto = p.PorDefecto
                }),
                (DB, filter) =>  (from p in DB.Set<Perfile>().Where(filter)
                                 select new PerfilesModel()
                                 {
                                     Descripcion = p.Descripcion,
                                     idPerfil = p.idPerfil,
                                     Nombre = p.Nombre,
                                     CantPermisos = DB.Set<PerfilesVistas>().Count(a => a.idPerfil == p.idPerfil),
                                     PorDefecto = p.PorDefecto ?? false
                                 })
            )
        { 
            
        }
        public PerfilesModel Get(int Id)
        {
            var model = base.Get(p => p.idPerfil == Id).FirstOrDefault();

            return model;
        }
        public IEnumerable<VistasModel> GetPermisos(int? idPerfil)
        {
            int id = idPerfil ?? 0;
            var permisosSet = dbContext.Set<PerfilesVistas>().Where(p => p.idPerfil == id);
            return from a in dbContext.Set<Areas>()
                   join m in dbContext.Set<Modulos>() on a.idArea equals m.idArea
                   join v in dbContext.Set<Vistas>() on m.idModulo equals v.idModulo
                   orderby a.Orden
                   select new VistasModel()
                   {
                       idArea = a.idArea,
                       Area = a.Nombre,
                       DescArea = a.Descripcion,
                       IconoArea = a.IconClass,
                       OrdenArea = a.Orden,
                       idModulo = m.idModulo,
                       Modulo = m.Nombre,
                       DescModulo = m.Descripcion,
                       IconoModulo = m.IconClass,
                       OrdenModulo = m.Orden,
                       idVista = v.idVista,
                       Vista = v.Nombre,
                       DescVista = v.Descripcion,
                       IconoVista = v.Descripcion,
                       Url = v.Url,
                       Permiso = permisosSet.Any(a => a.idVista == v.idVista),
                       Principal = v.Principal,
                       SoloSupervisores = v.SoloSupervisores ?? false
                   };
        }
        public IEnumerable<UsuariosModel> GetUsuarios(int idPerfil)
        {
            return from u in dbContext.Set<Usuarios>().Where(u => u.idPerfil == idPerfil)
                   join uo in dbContext.Set<UnidadOrg>() on u.idUnidadOrg equals uo.idUnidadOrg
                   select new UsuarioModel() 
                   { 
                        idUnidadOrg = u.idUnidadOrg,
                        UnidadOrg = uo.Nombre,
                        Activo = u.Activo,
                        Apellidos = u.Apellidos,
                        Cdula = u.Cdula,
                        idPerfil = u.idPerfil,
                        idUsuario = u.idUsuario,
                        Nombres = u.Nombres,
                        NombreUsuario = u.NombreUsuario
                   };
        }
        public override Perfile Add(PerfilesModel model)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (model.PorDefecto)
                    {
                        dbContext.Set<Perfiles>().ToList().ForEach(p => p.PorDefecto = false);
                        SaveChanges();
                    }

                    Perfiles created = base.Add(model);

                    var permisosSet = dbContext.Set<PerfilesVistas>();
                    if (model.Vistas != null && model.Vistas.Count() > 0)
                    {
                        var newPermisos = model.Vistas.Where(v => v.Permiso);
                        if (newPermisos != null)
                        {
                            permisosSet.AddRange(newPermisos.Select(p => new PerfilesVistas() 
                            { 
                                idPerfil = created.idPerfil, idVista = p.idVista
                            }));
                            SaveChanges();
                        }
                    }

                    if (model.Usuarios != null && model.Usuarios.Count() > 0)
                    {
                        var usuariosIds = model.Usuarios.Select(u => u.idUsuario);
                        var usuariosDarPerfil = dbContext.Set<Usuarios>().Where(u => usuariosIds.Contains(u.idUsuario)).ToList();
                        usuariosDarPerfil.ForEach(u => u.idPerfil = created.idPerfil);
                        SaveChanges();
                    }
                    
                    trx.Commit();
                    return created;
                }
                catch (Exception E)
                {
                    trx.Rollback();
                    throw E;
                }
            }
        }
        public override void Edit(PerfilesModel model)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (model.PorDefecto)
                    {
                        dbContext.Set<Perfiles>().ToList().ForEach(p => p.PorDefecto = false);
                        SaveChanges();
                    }

                    base.Edit(model, model.idPerfil);

                    var permisosSet = dbContext.Set<PerfilesVistas>();
                    if (model.Vistas != null && model.Vistas.Count() > 0)
                    {
                        permisosSet.RemoveRange(permisosSet.Where(p => p.idPerfil == model.idPerfil));

                        var newPermisos = model.Vistas.Where(v => v.Permiso);
                        if (newPermisos != null)
                        {
                            permisosSet.AddRange(newPermisos.Select(p => new PerfilesVistas()
                            {
                                idPerfil = model.idPerfil,
                                idVista = p.idVista
                            }));
                        }
                        SaveChanges();
                    }

                    if (model.Usuarios != null && model.Usuarios.Count() > 0)
                    {
                        var usuariosIds = model.Usuarios.Select(u => u.idUsuario);
                        var usuariosDarPerfil = dbContext.Set<Usuarios>().Where(u => usuariosIds.Contains(u.idUsuario)).ToList();
                        usuariosDarPerfil.ForEach(u => u.idPerfil = model.idPerfil);
                        SaveChanges();
                    }

                    trx.Commit();
                }
                catch (Exception E)
                {
                    trx.Rollback();
                    throw E;
                }
            }
        }
        public override void Delete(int id)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {
                    base.Delete(id);
                    var PVSet = dbContext.Set<PerfilesVistas>();
                    PVSet.RemoveRange(PVSet.Where(a => a.idPerfil == id));
                    SaveChanges();

                    trx.Commit();
                }
                catch (Exception ex)
                {
                    trx.Rollback();
                    throw ex;
                }
            }
        }
        public int[] VistasIdsCanAccess(int idUsuario)
        {
            var vistasPermitidas = (from u in dbContext.Set<Usuarios>().Where(u1 => u1.idUsuario == idUsuario)
                                    join pv in dbContext.Set<PerfilesVistas>() on u.idPerfil equals pv.idPerfil
                                    select pv.idVista).ToArray();
            return vistasPermitidas;
        }
        public bool CanAccess(int idUsuario, int idVista)
        {
            var PVSet = from u in dbContext.Set<Usuarios>().Where(u => u.idUsuario == idUsuario && u.idEstado == (int)EstadoUsuarioEnum.Activo)
                        join pv in dbContext.Set<PerfilesVistas>().Where(a => a.idVista == idVista) on u.idPerfil equals pv.idPerfil
                        select pv;

            return PVSet.Any();
        }
        public bool CanAccess(int idUsuario, int[] idVistas)
        {
            var PVSet = from u in dbContext.Set<Usuarios>().Where(u => u.idUsuario == idUsuario && u.idEstado == (int)EstadoUsuarioEnum.Activo)
                        join pv in dbContext.Set<PerfilesVistas>().Where(a => idVistas.Contains(a.idVista)) on u.idPerfil equals pv.idPerfil
                        select pv;

            return PVSet.Any();
        }
        public bool CanDelete(int id)
        {
            return !dbContext.Set<Usuarios>().Any(a => a.idPerfil == id);
        }
        public IEnumerable<TipoPerfil> GetTiposPerfil()
        {
            return dbContext.Set<TipoPerfil>();
        }
    }
}
