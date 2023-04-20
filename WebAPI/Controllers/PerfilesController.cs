using Data;
using Model.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using WebAPI.Infraestructure;
using Model.Enum;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar los perfiles o roles del sistema.
    /// </summary>
    [RoutePrefix("api/Perfiles")]
    public class PerfilesController : ApiBaseController
    {
        PerfilesRepo perfilesRepo = new PerfilesRepo();

        /// <summary>
        /// Obtiene un listado de todos los perfiles del sistema.
        /// </summary>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpGet]
        public List<PerfilesModel> Get()
        {
            List<PerfilesModel> perfiles = perfilesRepo.Get().ToList();
            foreach(var item in perfiles) 
            {
                item.Usuarios = GetUsuarios(item.idPerfil);
                item.Vistas = GetPermisos(item.idPerfil);
            }
            return perfiles;

        }

        /// <summary>
        /// Crea un nuevo perfil al sistema.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpPost]
        public OperationResult Post(PerfilesModel model)
        {
            if (ValidateModel(model))
            {
                var ifExist = perfilesRepo.Get(x => x.Nombre == model.Nombre).FirstOrDefault();

                if (ifExist != null)
                {
                    return new OperationResult(false, "Este perfil ya existe");
                }

                var created = perfilesRepo.Add(model);
                perfilesRepo.Log(created);
                return new OperationResult(true, "Se ha creado satisfactoriamente", created);
            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

        /// <summary>
        /// Actualiza la información de un perfil.
        /// </summary>
        /// <param name="idPerfil"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpPut]
        public OperationResult Put(int idPerfil, PerfilesModel model)
        {
            if (ValidateModel(model))
            {
                perfilesRepo.Edit(model);
                perfilesRepo.Log(model);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente", model);
            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

        /// <summary>
        /// Elimina un perfil.
        /// </summary>
        /// <param name="idPerfil"></param>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpDelete]
        public OperationResult Delete(int idPerfil)
        {
            try
            {
                perfilesRepo.Delete(idPerfil);
                perfilesRepo.Log(idPerfil);
                return new OperationResult(true, "Se ha eliminado satisfactoriamente");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, "No se ha podido eliminar este perfil");
            }
        }

        /// <summary>
        /// Obtiene un listado de usuarios de un perfil.
        /// </summary>
        /// <param name="idPerfil"></param>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpGet]
        [Route("GetUsuarios")]
        public List<UsuariosModel> GetUsuarios(int idPerfil)
        {
            return perfilesRepo.GetUsuarios(idPerfil).ToList();
        }


        /// <summary>
        /// Obtiene un listado de permisos de un perfil.
        /// </summary>
        /// <param name="idPerfil"></param>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpGet]
        [Route("GetPermisos")]
        public List<VistasModel> GetPermisos(int idPerfil)
        {
            return perfilesRepo.GetPermisos(idPerfil).ToList();
        }

        /// <summary>
        /// Obtiene un listado de todas las vistas del sistema.
        /// </summary>
        /// <returns></returns>
        [Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpGet]
        [Route("GetVistas")]
        public List<VistasModel> GetVistas()
        {
            return perfilesRepo.GetVistas().ToList();
        }
    }
}