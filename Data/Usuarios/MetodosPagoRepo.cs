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

namespace Data
{
    public class MetodosPagosRepo : Repository<MetodosPagoUsuarios, MetodosPagoUsuariosModel>
    {
        public MetodosPagosRepo(DbContext dbContext = null) : base
        (
            dbContext ?? new TiendaDBEntities(),
            new ObjectsMapper<MetodosPagoUsuariosModel, MetodosPagoUsuarios>(m => new MetodosPagoUsuarios()
            {
                idMetodo = m.idMetodo,
                idUsuario = m.idUsuario,
                Tipo = m.Tipo,
                Numero = m.Numero,
                FechaExpiracion = m.FechaExpiracion,
                CVV = m.CVV,
                EsPrincipal = m.EsPrincipal,
            }),
            (DB, filter) => (from m in DB.Set<MetodosPagoUsuarios>().Where(filter)
                             join u in DB.Set<Usuarios>() on m.idUsuario equals u.idUsuario
                             select new MetodosPagoUsuariosModel()
                             {
                                 idMetodo = m.idMetodo,
                                 idUsuario = m.idUsuario,
                                 NombreUsuario = u.NombreUsuario,
                                 Tipo = m.Tipo,
                                 Numero = m.Numero,
                                 FechaExpiracion = m.FechaExpiracion,
                                 CVV = m.CVV,
                                 EsPrincipal = m.EsPrincipal,
                             })
        )
        { }
    }
}
