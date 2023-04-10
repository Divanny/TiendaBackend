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
    public class CarritosRepo : Repository<Carritos, CarritosModel>
    {
        ProductosRepo productosRepo = new ProductosRepo();
        public CarritosRepo(DbContext dbContext = null) : base
        (
            dbContext ?? new TiendaDBEntities(),
            new ObjectsMapper<CarritosModel, Carritos>(p => new Carritos()
            {
                idCarrito = p.idCarrito,
                idUsuario = p.idUsuario,
                FechaCreacion = p.FechaCreacion,
                EstaTerminado = p.EstaTerminado,
            }),
            (DB, filter) => (from c in DB.Set<Carritos>().Where(filter)
                             join cp in DB.Set<CarritosProductos>() on c.idCarrito equals cp.idCarrito
                             select new CarritosModel()
                             {
                                 idCarrito = c.idCarrito,
                                 idUsuario = c.idUsuario,
                                 FechaCreacion = c.FechaCreacion,
                                 EstaTerminado = c.EstaTerminado,
                                 Productos = ProductosRepo.Get(x => cp.idProducto.Contains(x.idProducto))
                             })
        )
        {

        }
        public CarritosModel Get(int id)
        {
            var model = base.Get(c => c.idCarrito == id).FirstOrDefault();

            return model;
        }
        public IEnumerable<ProductosModel> GetProductos(int? idCarrito)
        {
            int id = idCarrito ?? 0;
            var poseeSet = dbContext.Set<CarritosProductos>().Where(p => p.idCarrito == id);
            return productosRepo.Get(x => poseeSet.idProducto.Contains(x.idProducto));
        }
    }
}
