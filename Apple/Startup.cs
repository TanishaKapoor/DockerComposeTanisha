using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apple;
using GreenPipes;
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

namespace UserService
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

            services.AddMassTransit(x =>
            {
                x.AddConsumer<AppleBananaConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri($"rabbitmq://{Configuration["RabbitMQHostName"]}"), hostconfig =>
                     {
                         hostconfig.Username("guest");
                         hostconfig.Password("guest");
                     });
                    cfg.ReceiveEndpoint(host, "appleBananaToCart", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<AppleBananaConsumer>(provider);
                    });
                }));

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,IApplicationLifetime lifetime ,IHostingEnvironment env)
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

            try
            {
                var bus = app.ApplicationServices.GetService<IBusControl>();
                var busHandle = TaskUtil.Await(() => {
                    return bus.StartAsync();
                });
                lifetime.ApplicationStopping.Register(() =>
                {
                    busHandle.Stop();
                });
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                app.Run(async (HttpContext context) =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("Rabbitmq is not working...!! Check the connection...!!!");
                    return;
                });
            }

            app.UseDiscoveryClient();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
