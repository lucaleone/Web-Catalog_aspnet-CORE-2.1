using JetBrains.Annotations;
using LucaLeone.WebCatalog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LucaLeone.WebCatalog.Data
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
