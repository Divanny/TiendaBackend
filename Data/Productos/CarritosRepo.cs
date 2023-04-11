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
                 select new
                 {
                     Carrito = c,
                     Producto = p,
                     CarritosProductos = cp
                 })
                             .GroupBy(x => x.Carrito)
                             .Select(g => new CarritosModel()
                             {
                                 idCarrito = g.Key.idCarrito,
                                 idUsuario = g.Key.idUsuario,
                                 FechaCreacion = g.Key.FechaCreacion,
                                 EstaTerminado = g.Key.EstaTerminado,
                                 Productos = g.Select(x => new ProductosModel()
                                 {
                                     idProducto = x.Producto.idProducto,
                                     Nombre = x.Producto.Nombre,
                                     Descripcion = x.Producto.Descripcion,
                                     CantidadStock = x.Producto.Cantidad,
                                     Valoracion = (x.Producto.SumaValoraciones > 0) ? (x.Producto.SumaValoraciones / x.Producto.CantidadValoraciones) : (0),
                                     FechaIngreso = x.Producto.FechaIngreso,
                                     Precio = x.CarritosProductos.PrecioPorProducto, // Precio que tiene CarritosProductos.PrecioPorProducto
                                     CantidadEnCarrito = x.CarritosProductos.Cantidad, // Cantidad que tiene CarritosProductos.Cantidad
                                     EstaActivo = x.Producto.EstaActivo,
                                 })
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
        public OperationResult RemoverProductos(int idCarrito, int idProducto, int cantidad)
        {
            var carritosProductos = dbContext.Set<CarritosProductos>().Where(x => x.idCarrito == idCarrito).ToList();

            if (carritosProductos.Any(x => x.idProducto == idProducto))
            {
                CarritosProductos model = dbContext.Set<CarritosProductos>().Where(x => x.idCarrito == idCarrito && x.idProducto == idProducto).FirstOrDefault();
                ProductosModel producto = productosRepo.Get(idProducto);

                if ((model.Cantidad - cantidad) >= 0)
                {
                    model.Cantidad -= cantidad;
                }
                else
                {
                    return new OperationResult(false, $"No existe esta cantidad de {producto.Nombre} en el carrito.");
                }

                dbContext.Entry(model).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                return new OperationResult(false, "Este producto no existe en el carrito");
            }

            return new OperationResult(true, "Se ha removido satisfactoriamente");
        }
    }
}
