using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LucaLeone.WebCatalog.API.DTO
{
    public class GetCatalogDto
    {
        /// <summary>
        ///     [Optional] Number of the page, default 1.
        /// </summary>
        [FromQuery]
        [Range(1, int.MaxValue, ErrorMessage = "The page number must be equal or greater than 1.")]
        public int Page { get; set; } = 1;

        /// <summary>
        ///     [Optional]Max amount of products to return, default 10.
        /// </summary>
        [FromQuery]
        [Range(1, 50, ErrorMessage = "The minimum number of element but be between 1 and 50.")]
        public int MaxNumElem { get; set; } = 10;
    }
}
