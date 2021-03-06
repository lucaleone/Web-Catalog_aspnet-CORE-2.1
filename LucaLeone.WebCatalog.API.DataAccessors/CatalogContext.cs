﻿using LucaLeone.WebCatalog.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace LucaLeone.WebCatalog.API.DataAccessors
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
/* Tools –> NuGet Package Manager –> Package Manager Console
   init DB:
        Add-Migration InitialCreate
   New migration:
        Add-Migration fixClass
   Apply new Migration:
        Update-Database –Verbose
   Remove:
        Remove-Migration
*/