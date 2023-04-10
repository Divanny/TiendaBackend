using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Data.Common;
using Model.Common;
using Model.Enum;
using Model.Productos;

namespace Data
{
    public class ProductosRepo: Repository<Productos, ProductosModel>
    {
        public ProductosRepo(DbContext dbContext = null) : base
            (
                dbContext ?? new TiendaDBEntities(),
                new ObjectsMapper<ProductosModel, Productos>(p => new Productos() {
                    idProducto = p.idProducto,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,

                    EstaActivo = p.EstaActivo
                }),
                (DB, filter) =>  (from p in DB.Set<Productos>().Where(filter)
                                 select new ProductosModel()
                                 {
                                     idProducto = p.idProducto,
                                     Nombre = p.Nombre,
                                     Descripcion = p.Descripcion,
                                     Categorias = DB.Set<ProductosCategorias>().Count(a => a.idProducto == p.idProducto),
                                     Cantidad = p.Cantidad,
                                     FechaIngreso = p.FechaIngreso,
                                     EstaActivo = p.EstaActivo
                                 })
            )
        { 
            
        }
        public ProductosModel Get(int Id)
        {
            var model = base.Get(p => p.idProducto == Id).FirstOrDefault();

            return model;
        }
        public IEnumerable<VistasModel> GetPermisos(int? idPerfil)
        {
            int id = idPerfil ?? 0;
            var permisosSet = dbContext.Set<PerfilesVistas>().Where(p => p.idPerfil == id);
            return from v in dbContext.Set<Vistas>()
                   select new VistasModel()
                   {
                       idVista = v.idVista,
                       Vista = v.Nombre,
                       DescVista = v.Descripcion,
                       URL = v.URL,
                       Permiso = permisosSet.Any(a => a.idVista == v.idVista),
                       Principal = v.Principal
                   };
        }
        public override Productos Add(ProductosModel model)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Productos created = base.Add(model);

                    var categoriasSet = dbContext.Set<ProductosCategorias>();
                    if (model.Categorias != null && model.Categorias.Count() > 0)
                    {
                        var newCategorias = model.Categorias.Where(v => v.Permiso);
                        if (newCategorias != null)
                        {
                            newCategorias.AddRange(newCategorias.Select(p => new ProductosCategorias() 
                            { 
                                idProducto = created.idProducto, idCategoria = p.idCategoria
                            }));
                            SaveChanges();
                        }
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
        public override void Edit(ProductosModel model)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {
                    base.Edit(model, model.idProducto);

                    var categoriasSet = dbContext.Set<ProductosCategorias>();
                    if (model.Categorias != null && model.Categorias.Count() > 0)
                    {
                        categoriasSet.RemoveRange(categoriasSet.Where(p => p.idProducto == model.idProducto));

                        var newCategorias = model.Categorias.Where(v => v.Permiso);
                        if (newCategorias != null)
                        {
                            categoriasSet.AddRange(newCategorias.Select(p => new ProductosCategorias()
                            {
                                idProducto = model.idProducto,
                                idCategoria = p.idCategoria
                            }));
                        }
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
                    var PCSet = dbContext.Set<ProductosCategorias>();
                    PCSet.RemoveRange(PCSet.Where(a => a.idProducto == id));
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
        public IEnumerable<VistasModel> GetVistas()
        {
            return from v in dbContext.Set<Vistas>()
                   select new VistasModel()
                   {
                       idVista = v.idVista,
                       Vista = v.Nombre,
                       DescVista = v.Descripcion,
                       URL = v.URL,
                       Principal = v.Principal
                   };
        }
    }
}
