using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LucaLeone.WebCatalog.API.DTO;
using LucaLeone.WebCatalog.API.Entities;

namespace LucaLeone.WebCatalog.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
        }
    }
}
