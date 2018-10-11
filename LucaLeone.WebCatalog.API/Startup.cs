using System;
using System.IO;
using System.Reflection;
using LucaLeone.WebCatalog.Data;
using LucaLeone.WebCatalog.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddDbContext<CatalogContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LucaLeone.WebCatalogDb")));
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
                        Contact = new Contact {Name = "Luca Leone", Email = "lucaleone@outlook.com"}
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
            const string swaggerDocumentationPath = "api-docs";
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = swaggerDocumentationPath + "/{documentName}/CatalogAPI.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = swaggerDocumentationPath;
                c.SwaggerEndpoint("v1/CatalogAPI.json",
                    "Catalog API v1");
            });
            var option = new RewriteOptions();
            option.AddRedirect("^$", swaggerDocumentationPath);
            app.UseRewriter(option);
            app.UseMvc();
        }
    }
}