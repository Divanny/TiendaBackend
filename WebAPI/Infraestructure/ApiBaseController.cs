using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace WebAPI.Infraestructure
{
    public class ApiBaseController : ApiController
    {
        public ModelValidation Validation { get; set; }
        public ApiBaseController()
        {
            Validation = new ModelValidation();
        }
        [System.Web.Http.NonAction]
        public bool ValidateModel(object thing)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            var vc = new ValidationContext(thing, null);
            var success = Validator.TryValidateObject(thing, vc, errors, true);

            Validation = new ModelValidation(success);
            foreach (ValidationResult err in errors)
            {
                Validation.Errors.Add(err.MemberNames.First(), err.ErrorMessage);
            }

            if (success)
                return true;
            else
                return false;
        }
        protected override void Dispose(bool disposing)
        {
            var curr = System.Web.HttpContext.Current;
            if (disposing)
            {
                base.Dispose(disposing);
            }
        }
    }
}