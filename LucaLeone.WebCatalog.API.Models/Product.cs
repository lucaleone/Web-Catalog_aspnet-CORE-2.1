using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FileHelpers;

namespace LucaLeone.WebCatalog.API.Models
{
    /// <summary>
    ///     Product describe a product item in the catalog
    /// </summary>
    [Serializable]
    [DelimitedRecord(",")]
    public class Product
    {
        /// <summary>
        ///     Base Product Constructor, initialize the Product Id
        /// </summary>
        private Product()
        {
            Id = Guid.NewGuid();
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

        /// <summary>Id of the product</summary>
        [Key]
        [Required]
        [FieldQuoted]
        public Guid Id { get; set; }

        /// <summary>Name of the product</summary>
        [Required]
        [MaxLength(200)]
        [FieldQuoted]
        public string Name { get; set; }

        /// <summary>Photo url of the product</summary>
        [DefaultValue("")]
        [Url]
        [FieldQuoted]
        public string Photo { get; set; }

        /// <summary>Price of the product</summary>
        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        [FieldQuoted]
        public decimal Price { get; set; }

        /// <summary>UTC time of the last update of the product</summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [FieldQuoted]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     Set product detail and automatically update the LastUpdated field
        /// </summary>
        /// <param name="name">Name of the product</param>
        /// <param name="photo">Photo url of the product</param>
        /// <param name="price">Price of the product</param>
        public void EditProduct(string name, string photo, decimal price)
        {
            bool changeDone = false;
            if (!Name.Equals(name))
            {
                Name = name;
                changeDone = true;
            }

            if (!Photo.Equals(photo))
            {
                Photo = photo;
                changeDone = true;
            }

            if (Price != price)
            {
                Price = price;
                changeDone = true;
            }

            if (changeDone)
                LastUpdated = DateTime.UtcNow;
        }
    }
}