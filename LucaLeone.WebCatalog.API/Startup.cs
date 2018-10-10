using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LucaLeone.WebCatalog.Data;
using LucaLeone.WebCatalog.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace LucaLeone.WebCatalog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<CatalogContext>(opt => opt.UseInMemoryDatabase("CatalogContext"));
            services.AddDbContext<CatalogContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CatalogDbConnection")));
            services.AddScoped<ICatalogService, CatalogService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "CatalogAPI",
                        Description =
                            @"RESTful service for managing product catalog. Users can add(POST), edit(PUT), remove(DELETE), view(GET) and search(GET) product, export csv(GET) the catalog.",
                        Version = "v1",
                        Contact = new Contact { Name = "Luca Leone", Email = "lucaleone@outlook.com" }
                    });                
                c.IncludeXmlComments(GetXMLDocumentation());
            });
        }

        private string GetXMLDocumentation()
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            return Path.Combine(AppContext.BaseDirectory, xmlFile);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSwagger(c => { c.RouteTemplate = "api-docs/{documentName}/CatalogAPI.json"; });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api-docs";
                c.SwaggerEndpoint("v1/CatalogAPI.json",
                    "Catalog API v1");
            });
            app.UseMvc();
        }
    }
}
