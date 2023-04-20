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
        Mailing mailing = new Mailing();
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
                idDireccion = p.idDireccion,
                MontoPagado = p.MontoPagado,
                FechaIngreso = p.FechaIngreso,
                FechaUltimoEstado = p.FechaUltimoEstado
            }),
            (DB, filter) => (from p in DB.Set<Pedidos>().Where(filter)
                             join m in DB.Set<MetodosPagoUsuarios>() on p.idMetodo equals m.idMetodo
                             join u in DB.Set<Usuarios>() on p.idUsuario equals u.idUsuario
                             join d in DB.Set<DireccionesUsuarios>() on p.idDireccion equals d.idDireccion
                             join e in DB.Set<EstadosPedidos>() on p.idEstado equals e.idEstado
                             select new PedidosModel()
                             {
                                 idPedido = p.idPedido,
                                 idUsuario = p.idUsuario,
                                 NombreUsuario = u.NombreUsuario,
                                 idCarrito = p.idCarrito,
                                 idEstado = p.idEstado,
                                 Estado = e.Nombre,
                                 idMetodo = p.idMetodo,
                                 Metodo = m.Tipo,
                                 idDireccion = p.idDireccion,
                                 Direccion = d.Direccion,
                                 MontoPagado = p.MontoPagado,
                                 FechaIngreso = p.FechaIngreso,
                                 FechaUltimoEstado = p.FechaUltimoEstado,
                             })
        )
        { }

        public byte[] GenerarFactura(int idPedido)
        {
            PedidosModel pedido = this.Get(x => x.idPedido == idPedido).First();

            string html = mailing.genFactura(pedido);

            using (MemoryStream ms = new MemoryStream())
            {
                Document documento = new Document();
                PdfWriter writer = PdfWriter.GetInstance(documento, ms);
                writer.CloseStream = false;
                documento.Open();

                XMLWorkerHelper.GetInstance().ParseXHtml(writer, documento, new StringReader(html));

                documento.Close();

                return ms.ToArray();
            }
        }
    }
}
