using System;
using System.IO;
using System.Reflection;
using LucaLeone.WebCatalog.API.DataAccessors;
using LucaLeone.WebCatalog.API.Services;
using LucaLeone.WebCatalog.API.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace LucaLeone.WebCatalog.API
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
            var ttt = Configuration.GetConnectionString("LucaLeone.WebCatalogDb");
            Console.WriteLine(ttt);
            //services.AddDbContext<CatalogContext>(opt => opt.UseInMemoryDatabase("CatalogContext"));
            services.AddDbContext<CatalogContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LucaLeone.WebCatalogDb")));
            services.AddScoped<ICatalogService, CatalogService>();
            services.AddSingleton<ICatalogValidator, CatalogValidator>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "CatalogAPI",
                        Description =
                            @"🚀 Asp.NET CORE Web API for managing a products catalog",
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
            app.UseStatusCodePages("text/plain", "Status code page, status code: {0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = swaggerDocumentationPath + "/{documentName}/CatalogAPI.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = swaggerDocumentationPath;
                c.SwaggerEndpoint("v1/CatalogAPI.json",
                    "Catalog API v1");
                c.IndexStream = () => GetType().GetTypeInfo().Assembly
                                               .GetManifestResourceStream(
                                                   "LucaLeone.WebCatalog.API.wwwroot.swagger.index.html");
                c.InjectStylesheet("/swagger/custom.css");
            });
            var option = new RewriteOptions();
            option.AddRedirect("^$", swaggerDocumentationPath);
            app.UseRewriter(option);
            app.UseMvc();
        }
    }
}