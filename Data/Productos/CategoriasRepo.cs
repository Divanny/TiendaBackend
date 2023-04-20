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
    public class CategoriasRepo: Repository<Categorias, CategoriasModel>
    {
        public CategoriasRepo(DbContext dbContext = null) : base
        (
            dbContext ?? new TiendaDBEntities(),
            new ObjectsMapper<CategoriasModel, Categorias>(p => new Categorias() {
                idCategoria = p.idCategoria,
                Descripcion = p.Descripcion,
                Nombre = p.Nombre,
                FechaIngreso = p.FechaIngreso ?? DateTime.Now
            }),
            (DB, filter) =>  (from p in DB.Set<Categorias>().Where(filter)
                                select new CategoriasModel()
                                {
                                    idCategoria = p.idCategoria,
                                    Descripcion = p.Descripcion,
                                    Nombre = p.Nombre,
                                    FechaIngreso = p.FechaIngreso
                                })
        )
        { 
            
        }

        public override void Delete(int id)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {
                    base.Delete(id);
                    var PCSet = dbContext.Set<ProductosCategorias>();
                    PCSet.RemoveRange(PCSet.Where(a => a.idCategoria == id));
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
    }
}
