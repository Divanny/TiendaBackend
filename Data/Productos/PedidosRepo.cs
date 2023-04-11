using Data.Common;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Model.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations.Model;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using iTextSharp.tool.xml;
using Model.Usuarios;
using Model.Productos;

namespace Data
{
    public class PedidosRepo : Repository<Pedidos, PedidosModel>
    {
        public PedidosRepo(DbContext dbContext = null) : base
        (
            dbContext ?? new TiendaDBEntities(),
            new ObjectsMapper<PedidosModel, Pedidos>(p => new Pedidos()
            {
                idPedido = p.idPedido,
                idUsuario = p.idUsuario,
                idCarrito = p.idCarrito,
                idEstado = p.idEstado,
                idMetodo = p.idMetodo,
                MontoPagado = p.MontoPagado,
                FechaIngreso = p.FechaIngreso,
                FechaUltimoEstado = p.FechaUltimoEstado
            }),
            (DB, filter) => (from p in DB.Set<Pedidos>().Where(filter)
                             join m in DB.Set<MetodosPagoUsuarios>() on p.idMetodo equals m.idMetodo
                             join e in DB.Set<EstadosPedidos>() on p.idEstado equals e.idEstado
                             select new PedidosModel()
                             {
                                 idPedido = p.idPedido,
                                 idUsuario = p.idUsuario,
                                 idCarrito = p.idCarrito,
                                 idEstado = p.idEstado,
                                 Estado = e.Nombre,
                                 idMetodo = p.idMetodo,
                                 Metodo = m.Tipo,
                                 MontoPagado = p.MontoPagado,
                                 FechaIngreso = p.FechaIngreso,
                                 FechaUltimoEstado = p.FechaUltimoEstado,
                             })
        )
        { }
    }
}
