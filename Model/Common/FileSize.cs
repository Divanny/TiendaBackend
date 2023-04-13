using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Model.Common
{
    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSize;

        public FileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as HttpPostedFileBase;

            if (file == null)
            {
                return ValidationResult.Success;
            }

            if (file.ContentLength > _maxSize)
            {
                return new ValidationResult($"El archivo no puede ser mayor de {_maxSize} bytes.");
            }

            return ValidationResult.Success;

        }
    }
}
