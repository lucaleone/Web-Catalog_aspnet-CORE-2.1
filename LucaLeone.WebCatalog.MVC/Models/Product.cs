using System;
using System.ComponentModel.DataAnnotations;

namespace LucaLeone.WebCatalog.MVC.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public decimal Price { get; set; }
    }
}