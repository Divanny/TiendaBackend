using Model.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    public class UsuariosRepo
    {
        TiendaDBEntities dbContext = new TiendaDBEntities();
        public ObjectResult<InsertarUsuario_Result> Add(UsuariosModel model)
        {
            try
            {
                var created = dbContext.InsertarUsuario(model.NombreUsuario, model.CorreoElectronico, model.PasswordHash, model.Estado, model.FechaRegistro, model.UltimoIngreso, model.Nombres, model.Apellidos, model.Telefono);
                dbContext.SaveChanges();
                return created;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        public Usuarios GetByID(int id)
        {
            var usuario = dbContext.Set<Usuarios>().Where(x => x.idUsuario == id).FirstOrDefault();
            return usuario;
        }

        public Usuarios GetByUsername(string username)
        {
            var usuario = dbContext.Set<Usuarios>().Where(x => x.NombreUsuario == username).FirstOrDefault();
            return usuario;
        }

        public List<Usuarios> GetAll()
        {
            var usuarios = dbContext.Set<Usuarios>().ToList();
            return usuarios;
        }
    }
}
