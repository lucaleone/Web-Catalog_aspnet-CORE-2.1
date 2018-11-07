using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace LucaLeone.WebCatalog.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
#if DEBUG
                   .CaptureStartupErrors(true)
                   .UseSetting("detailedErrors", "true")
#endif
                   .ConfigureLogging(logging =>
                   {
                       logging.ClearProviders();
#if DEBUG
                       logging.AddDebug();
#endif
                   })
                   .UseStartup<Startup>();
    }
}