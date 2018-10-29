using System;
using System.Collections.Generic;
using System.Text;

namespace LucaLeone.WebCatalog.API.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
