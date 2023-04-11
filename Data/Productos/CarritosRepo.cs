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
using System.Runtime.Remoting.Contexts;

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
                join p in DB.Set<Productos>() on cp.idProducto equals p.idProducto
                where cp.idCarrito == c.idCarrito /* OJO -- ARREGLAR ESTO, SOLO TRAE 1 PRODUCTO, TAMBIEN HACER QUE DIGA LA CANTIDAD DE ESE PRODUCTO Y PRECIO POR PRODUCTO */
                select new CarritosModel()
                {
                    idCarrito = c.idCarrito,
                    idUsuario = c.idUsuario,
                    FechaCreacion = c.FechaCreacion,
                    EstaTerminado = c.EstaTerminado,
                    Productos = DB.Set<Productos>().Where(x => cp.idProducto == x.idProducto).Select(x => new ProductosModel()
                    {
                        idProducto = x.idProducto,
                        Nombre = x.Nombre,
                        Descripcion = x.Descripcion,
                        CantidadStock = x.Cantidad,
                        Valoracion = (x.SumaValoraciones > 0) ? (x.SumaValoraciones / x.CantidadValoraciones) : (0),
                        FechaIngreso = x.FechaIngreso,
                        EstaActivo = x.EstaActivo,
                    }).ToList(),
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
            var idProductos = poseeSet.Select(p => p.idProducto).ToList();
            return productosRepo.Get(x => idProductos.Contains(x.idProducto));
        }
        public OperationResult InsertarProductos(int idCarrito, int idProducto, int cantidad, int precioPorProducto)
        {
            var carritosProductos = dbContext.Set<CarritosProductos>().Where(x => x.idCarrito == idCarrito).ToList();

            if (carritosProductos.Any(x => x.idProducto == idProducto))
            {
                CarritosProductos model = dbContext.Set<CarritosProductos>().Where(x => x.idCarrito == idCarrito && x.idProducto == idProducto).FirstOrDefault();

                model.Cantidad += cantidad;
                model.PrecioPorProducto = precioPorProducto;

                dbContext.Entry(model).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                CarritosProductos model = new CarritosProductos()
                {
                    idCarrito = idCarrito,
                    idProducto = idProducto,
                    Cantidad = cantidad,
                    PrecioPorProducto = precioPorProducto,
                    FechaIngreso = DateTime.Now
                };

                dbContext.Set<CarritosProductos>().Add(model);
                dbContext.SaveChanges();
            }

            return new OperationResult(true, "Se ha agregado satisfactoriamente");
        }
    }
}
