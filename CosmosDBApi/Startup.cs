using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CosmosDBAPI.DataProviders;
using CosmosDBAPI.Interfaces;

namespace CosmosDBAPI
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSingleton<IUser>(InitializeUserDBClientInstanceAsync(Configuration.GetSection("UserDB")));
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CosmosDBAPI V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
        private static UserData InitializeUserDBClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;
            Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            UserData userData = new UserData(client, databaseName, containerName);

            return userData;
        }
    }
}
