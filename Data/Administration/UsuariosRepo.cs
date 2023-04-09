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

namespace Data
{
    public class UsuariosRepo : Repository<Usuarios, UsuariosModel>
    {
        public UsuariosRepo(DbContext dbContext = null) : base
        (
            dbContext ?? new TiendaDBEntities(),
            new ObjectsMapper<UsuariosModel, Usuarios>(u => new Usuarios()
            {
                idUsuario = u.idUsuario,
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,
                NombreUsuario = u.NombreUsuario,
                CorreoElectronico = u.CorreoElectronico,
                PasswordHash = u.PasswordHash,
                idPerfil = u.idPerfil,
                idEstado = u.idEstado,
                FechaRegistro = u.FechaRegistro,
                UltimoIngreso = u.UltimoIngreso
            }),
            (DB, filter) => (from u in DB.Set<Usuarios>().Where(filter)
                             join p in DB.Set<Perfiles>() on u.idPerfil equals p.idPerfil
                             join e in DB.Set<EstadosUsuarios>() on u.idEstado equals e.idEstado
                             select new UsuariosModel()
                             {
                                 idUsuario = u.idUsuario,
                                 Nombres = u.Nombres,
                                 Apellidos = u.Apellidos,
                                 NombreUsuario = u.NombreUsuario,
                                 CorreoElectronico = u.CorreoElectronico,
                                 PasswordHash = u.PasswordHash,
                                 idPerfil = u.idPerfil,
                                 Perfil = p.Nombre,
                                 idEstado = u.idEstado,
                                 Estado = e.Nombre,
                                 FechaRegistro = u.FechaRegistro,
                                 UltimoIngreso = u.UltimoIngreso
                             })
        )
        { }

        public UsuariosModel GetByUsername(string nombreUsuario)
        {
            return this.Get(x => x.NombreUsuario == nombreUsuario).FirstOrDefault();
        }

        public UsuariosModel Get(int id)
        {
            var result = base.Get(a => a.idUsuario == id).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;
        }

        public List<EstadosUsuarios> GetEstadosUsuarios()
        {
            return dbContext.Set<EstadosUsuarios>().ToList();
        }

        public byte[] GenerarReporteUsuarios()
        {
            // Obtener los usuarios del sistema
            var usuarios = base.Get().ToList();

            // Generar el HTML del reporte manualmente
            string html = @"
            <html>
            <head>
                <style>
                    table {
                        width: 100%;
                        border-collapse: collapse;
                    }

                    th, td {
                        padding: 6px;
                        text-align: left;
                        border-bottom: 1px solid #ddd;
                        font-size: 8pt;
                    }

                    th {
                        background-color: #f2f2f2;
                        font-weight: bold;
                    }

                    h1 {
                        font-size: 14pt;
                        margin-bottom: 20px;
                    }

                    @media screen {
                        body {
                            font-family: Arial, sans-serif;
                            font-size: 10pt;
                            margin: 0;
                        }

                        table {
                            page-break-inside: avoid;
                        }
                    }

                    @media print {
                        body {
                            font-family: Arial, sans-serif;
                            font-size: 8pt;
                            margin: 0;
                        }

                        table {
                            page-break-inside: avoid;
                        }
                    }
                </style>
            </head>
            <body>"+
                $@"<h1>Reporte de Usuarios — {DateTime.Now}</h1>

                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Username</th>
                            <th>Correo electrónico</th>
                            <th>Nombre</th>
                            <th>Perfil</th>
                            <th>Estado</th>
                            <th>Fecha de registro</th>
                        </tr>
                    </thead>
                    <tbody>";

                    foreach (var usuario in usuarios)
                    {
                        html += $"<tr>" +
                            $"<td>{usuario.idUsuario}</td>" +
                            $"<td>{usuario.NombreUsuario}</td>" +
                            $"<td>{usuario.CorreoElectronico}</td>" +
                            $"<td>{usuario.Nombres} {usuario.Apellidos}</td>" +
                            $"<td>{usuario.Perfil}</td>" +
                            $"<td>{usuario.Estado}</td>" +
                            $"<td>{usuario.FechaRegistro}</td>" +
                            $"</tr>";
                    }

                    html += @"
                    </tbody>
                </table>
            </body>
            </html>";

            // Convertir el HTML a PDF con iTextSharp
            using (MemoryStream ms = new MemoryStream())
            {
                Document documento = new Document();
                PdfWriter writer = PdfWriter.GetInstance(documento, ms);
                writer.CloseStream = false;
                documento.Open();

                // Parsear el HTML con XMLWorker
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, documento, new StringReader(html));

                documento.Close();

                return ms.ToArray();
            }
        }

    }
}
