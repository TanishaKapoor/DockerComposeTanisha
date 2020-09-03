using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Steeltoe.Discovery.Client;

namespace OrderService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDiscoveryClient(Configuration);

            //allow cors routing
            services.AddCors(options => {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            //hostConfig 
            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri($"rabbitmq://{Configuration["RabbitMQHostName"]}"),hostConfig=>
                    {
                        hostConfig.Username("guest");
                        hostConfig.Password("guest");
                    });
                }));
            });
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,IApplicationLifetime lifetime, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("AllowAll");

            //starting the bus
            try
            {
                var bus = app.ApplicationServices.GetService<IBusControl>();
                var busHandle = TaskUtil.Await(() =>
                {
                    return bus.StartAsync();
                });
                lifetime.ApplicationStopping.Register(() =>
                {
                    busHandle.Stop();
                });

            }catch(Exception ex)
            {
                Console.Write(ex.Message);
                app.Run(async (HttpContext context) =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("Rabbitmq is not working...!!Check the connection..!!");
                });
            }

            app.UseDiscoveryClient();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
