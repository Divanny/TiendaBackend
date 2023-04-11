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
    public class DireccionesRepo : Repository<DireccionesUsuarios, DireccionesModel>
    {
        public DireccionesRepo(DbContext dbContext = null) : base
        (
            dbContext ?? new TiendaDBEntities(),
            new ObjectsMapper<DireccionesModel, DireccionesUsuarios>(d => new DireccionesUsuarios()
            {
                idDireccion = d.idDireccion,
                idUsuario = d.idUsuario,
                Direccion = d.Direccion,
                Ciudad = d.Ciudad,
                CodigoPostal = d.CodigoPostal,
                Pais = d.Pais,
                EsPrincipal = d.EsPrincipal,
            }),
            (DB, filter) => (from d in DB.Set<DireccionesUsuarios>().Where(filter)
                             join u in DB.Set<Usuarios>() on d.idUsuario equals u.idUsuario
                             select new DireccionesModel()
                             {
                                 idDireccion = d.idDireccion,
                                 idUsuario = d.idUsuario,
                                 NombreUsuario = u.NombreUsuario,
                                 Direccion = d.Direccion,
                                 Ciudad = d.Ciudad,
                                 CodigoPostal = d.CodigoPostal,
                                 Pais = d.Pais,
                                 EsPrincipal = d.EsPrincipal,
                             })
        )
        { }
    }
}
