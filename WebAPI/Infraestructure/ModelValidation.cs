using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Infraestructure
{
    public class ModelValidation
    {
        public bool IsValid { get; set; }
        public Dictionary<string, string> Errors { get; set; }
        public ModelValidation()
        {
            Errors = new Dictionary<string, string>();
        }
        public ModelValidation(bool IsValid)
        {
            this.IsValid = IsValid;
            Errors = new Dictionary<string, string>();
        }
        public ModelValidation(bool IsValid, Dictionary<string, string> errors)
        {
            this.IsValid = IsValid;
            Errors = errors;
        }
    }
}