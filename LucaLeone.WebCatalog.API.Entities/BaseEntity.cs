using System;
using System.ComponentModel.DataAnnotations;

namespace LucaLeone.WebCatalog.API.Entities
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Id of the product.
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime CreateDate { get; set; }


        /// <summary>UTC time of the last update of the product</summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime LastUpdated { get; set; }
    }
}
