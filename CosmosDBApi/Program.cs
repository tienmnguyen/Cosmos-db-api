using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CosmosDBAPI
{
    public class Program
    {
        //TODO add Swagger
        // add logging
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        // JSON files, User secrets, environment variables and command line arguments
                        config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddJsonFile("appsettings.json");
                    });
                });
    }
}
