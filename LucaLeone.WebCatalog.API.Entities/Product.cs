using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucaLeone.WebCatalog.API.Entities
{
    /// <summary>
    ///     Product describe a product item in the catalog
    /// </summary>
    [Serializable]
    public class Product : BaseEntity
    {
        /// <summary>
        ///     Base Product Constructor, initialize the Product Id
        /// </summary>
        private Product()
        {
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        ///     Calls the base Product Constructor and set the fields, then update LastUpdated
        /// </summary>
        public Product(string name, string photo, decimal price) : this()
        {
            Name = name;
            Photo = photo;
            Price = price;
        }

        /// <summary>Name of the product</summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>Photo url of the product</summary>
        [DefaultValue("")]
        [Url]
        public string Photo { get; set; }

        /// <summary>Price of the product</summary>
        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

    }
}