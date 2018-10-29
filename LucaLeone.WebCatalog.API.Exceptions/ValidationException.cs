using System;
using System.Collections.Generic;

namespace LucaLeone.WebCatalog.API.Exceptions
{    
    public class ValidationException : Exception
    {
        public IEnumerable<string> Errors { get; private set; }

        public ValidationException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}
