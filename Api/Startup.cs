using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RawRabbit.Extensions.Client;

namespace Api
{
    public static class Engine
    {
        public static bool IsReady { get; set; }
    }

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();


            var rabbitMqHostname = Configuration.GetSection("RabbitMQ").GetSection("Hostnames").GetChildren().ToList().First().Value;
            Console.Out.WriteLine($"RabbitMQ hostname: {rabbitMqHostname}");

            Policy.Handle<Exception>()
                .WaitAndRetryForever(i =>
                {
                    Console.Out.WriteLine($"Trying to connect to RabbitMQ on {rabbitMqHostname}. Retry count: {i}");
                    return TimeSpan.FromSeconds(5);
                })
                .Execute(() =>
                {
                    services.AddRawRabbit(Configuration.GetSection("RabbitMQ"));
                    Engine.IsReady = true;
                });


            services.Configure<SystemInfo>(info =>
            {
                info.Hostname = Configuration.GetValue<string>("HOSTNAME");
            });

            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
