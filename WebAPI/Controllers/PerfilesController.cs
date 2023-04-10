using Data;
using Model.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebAPI.Infraestructure;
using Model.Enum;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/Perfiles")]
    public class PerfilesController : ApiBaseController
    {
        PerfilesRepo perfilesRepo = new PerfilesRepo();

        [Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpGet]
        public List<PerfilesModel> Get()
        {
            return perfilesRepo.Get().ToList();
        }

        [Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpPost]
        public OperationResult Post(PerfilesModel model)
        {
            if (ValidateModel(model))
            {
                var ifExist = perfilesRepo.Get(x => x.Nombre == model.Nombre);

                if (ifExist != null) {
                    return new OperationResult(false, "Este perfil ya existe", Validation.Errors);
                }

                var created = perfilesRepo.Add(model);
                perfilesRepo.Log(created);
                return new OperationResult(true, "Se ha creado satisfactoriamente", created);
            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

        [Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpPut]
        public OperationResult Put(int idPerfil, PerfilesModel model)
        {
            if (ValidateModel(model))
            {
                perfilesRepo.Edit(model, idPerfil);
                perfilesRepo.Log(model);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente", model);
            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

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
            catch(Exception ex) 
            { 
                return new OperationResult(false, "No se ha podido eliminar este perfil");
            }
        }

        [Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpGet]
        [Route("GetVistas")]
        public List<VistasModel> GetVistas()
        {
            return perfilesRepo.GetVistas().ToList();
        }
    }
}