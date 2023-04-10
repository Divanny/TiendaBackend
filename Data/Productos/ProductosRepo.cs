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
    public class ProductosRepo : Repository<Productos, ProductosModel>
    {
        public ProductosRepo(DbContext dbContext = null) : base
        (
            dbContext ?? new TiendaDBEntities(),
            new ObjectsMapper<ProductosModel, Productos>(p => new Productos()
            {
                idProducto = p.idProducto,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Cantidad = p.CantidadStock,
                EstaActivo = p.EstaActivo
            }),
            (DB, filter) => (from p in DB.Set<Productos>().Where(filter)
                             join pc in DB.Set<ProductosCategorias>() on p.idProducto equals pc.idProducto
                             join c in DB.Set<Categorias>() on pc.idCategoria equals c.idCategoria
                             select new
                             {
                                 Producto = p,
                                 Categoria = c
                             })
                             .GroupBy(x => x.Producto)
                             .Select(g => new ProductosModel()
                             {
                                 idProducto = g.Key.idProducto,
                                 Nombre = g.Key.Nombre,
                                 Descripcion = g.Key.Descripcion,
                                 CantCategorias = DB.Set<ProductosCategorias>().Count(a => a.idProducto == g.Key.idProducto),
                                 CantidadStock = g.Key.Cantidad,
                                 Valoracion = (g.Key.SumaValoraciones > 0) ? (g.Key.SumaValoraciones / g.Key.CantidadValoraciones) : (0),
                                 FechaIngreso = g.Key.FechaIngreso,
                                 EstaActivo = g.Key.EstaActivo,
                                 Categorias = g.Select(x => new CategoriasModel()
                                 {
                                     idCategoria = x.Categoria.idCategoria,
                                     Nombre = x.Categoria.Nombre
                                 })
                             })
        )
        {

        }
        //public ProductosRepo(DbContext dbContext = null) : base
        //    (
        //        dbContext ?? new TiendaDBEntities(),
        //        new ObjectsMapper<ProductosModel, Productos>(p => new Productos()
        //        {
        //            idProducto = p.idProducto,
        //            Nombre = p.Nombre,
        //            Descripcion = p.Descripcion,
        //            Cantidad = p.Cantidad,
        //            EstaActivo = p.EstaActivo
        //        }),
        //        (DB, filter) => (from p in DB.Set<Productos>().Where(filter)
        //                         select new ProductosModel()
        //                         {
        //                             idProducto = p.idProducto,
        //                             Nombre = p.Nombre,
        //                             Descripcion = p.Descripcion,
        //                             CantCategorias = DB.Set<ProductosCategorias>().Count(a => a.idProducto == p.idProducto),
        //                             Cantidad = p.Cantidad,
        //                             FechaIngreso = p.FechaIngreso,
        //                             EstaActivo = p.EstaActivo,
        //                         })
        //    )
        //{

        //}
        public ProductosModel Get(int Id)
        {
            var model = base.Get(p => p.idProducto == Id).FirstOrDefault();

            return model;
        }
        public IEnumerable<CategoriasModel> GetCategoria(int? idProducto)
        {
            int id = idProducto ?? 0;
            var poseeSet = dbContext.Set<ProductosCategorias>().Where(p => p.idProducto == id);
            return from c in dbContext.Set<Categorias>()
                   select new CategoriasModel()
                   {
                       idCategoria = c.idCategoria,
                       Nombre = c.Nombre,
                       Descripcion = c.Descripcion,
                       PoseeCategoria = poseeSet.Any(a => a.idCategoria == c.idCategoria),
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
                        var newCategorias = model.Categorias.Where(v => v.PoseeCategoria);
                        if (newCategorias != null)
                        {
                            categoriasSet.AddRange(newCategorias.Select(p => new ProductosCategorias()
                            {
                                idProducto = created.idProducto,
                                idCategoria = p.idCategoria
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

                        var newCategorias = model.Categorias.Where(v => v.PoseeCategoria);
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
        public IEnumerable<CategoriasModel> GetCategorias()
        {
            return from c in dbContext.Set<Categorias>()
                   select new CategoriasModel()
                   {
                       idCategoria = c.idCategoria,
                       Nombre = c.Nombre,
                       Descripcion = c.Descripcion,
                       FechaIngreso = c.FechaIngreso,
                   };
        }
    }
}
