using Data.Common;
using Data;
using Model.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Model.Enum;
using System.Web.UI.WebControls;

namespace WebAPI.Infraestructure
{
    class Authentication
    {
        public LogInResult LogIn(Credentials credentials)
        {
            LogInResult logInResult;
            using (var dbc = new TiendaDBEntities())
            {
                UsuariosRepo ur = new UsuariosRepo(dbc);
                var usuarios = ur.Get().ToList();
                var usuario = ur.GetFirst(x => x.idUsuario == 1);

                if (usuario == null) return new LogInResult(false, "Usuario o contraseña invalidos");

                var PasswordHash = Cryptography.Encrypt(credentials.password);
                bool passwordMatch = Cryptography.CompareByteArrays(PasswordHash, usuario.PasswordHash);

                if (!passwordMatch) return new LogInResult(false, "La contraseña ingresada es incorrecta");

                if (usuario.idEstado == (int)EstadoUsuarioEnum.Suspendido) return new LogInResult(false, "<i class='fas fa-lock'></i> El usuario <strong>" + credentials.userName + "</strong> está suspendido temporalmente");
                
                if (usuario.idEstado == (int)EstadoUsuarioEnum.Baneado) return new LogInResult(false, "<i class='fas fa-lock'></i> El usuario <strong>" + credentials.userName + "</strong> está baneado indefinidamente");
                
                if (usuario.idEstado == (int)EstadoUsuarioEnum.Inactivo) return new LogInResult(false, "<i class='fas fa-lock'></i> El usuario <strong>" + credentials.userName + "</strong> está inactivo");

                logInResult = new LogInResult(true, "Exito al iniciar sesión", true, usuario.idUsuario);

                SessionData.Set(new UserSesionInfo() { idUsuario = usuario.idUsuario.ToString(), idPerfil = usuario.idPerfil.ToString() });
            }
            return logInResult;
        }
    }
}