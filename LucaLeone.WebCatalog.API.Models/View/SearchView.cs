using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LucaLeone.WebCatalog.API.Models.View
{
    public class SearchView : IValidatableObject
    {
        /// <summary>
        ///     [Optional] Name of the Product to search.
        /// </summary>
        [FromQuery(Name = "name")]
        public string Name { get; set; } = "";

        /// <summary>
        ///     [Optional] Min price of the product in €.
        /// </summary>
        [FromQuery(Name = "minPrice")]
        [Range(0, int.MaxValue, ErrorMessage = "The minimum price must be positive.")]
        public int MinPrice { get; set; } = 0;

        /// <summary>
        ///     [Optional] Max price of the product in €.
        /// </summary>
        [FromQuery(Name = "maxPrice")]
        [Range(1, int.MaxValue, ErrorMessage = "The maximum price must be greater than 1.")]
        public int? MaxPrice { get; set; }  = null;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateProperty(this.MinPrice,
                new ValidationContext(this, null, null) { MemberName = nameof(MinPrice) },
                results);
            Validator.TryValidateProperty(this.MaxPrice,
                new ValidationContext(this, null, null) { MemberName = nameof(MaxPrice) },
                results);
            if (MaxPrice.HasValue && MinPrice > MaxPrice.Value)
                results.Add(new ValidationResult($"{nameof(MaxPrice)} must be greater than {nameof(MinPrice)}"));

            return results;
        }
    }
}
