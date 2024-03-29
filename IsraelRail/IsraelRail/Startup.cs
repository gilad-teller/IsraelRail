﻿using IsraelRail.Models.ViewModels;
using IsraelRail.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApplicationInsightsTelemetryEnhancer22;
using System;

namespace IsraelRail
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddApplicationInsightsTelemetry();
            services.AddDependencyTelemetryEnhancer();
            services.AddLogging();
            services.AddHttpClient("RailApi", options =>
            {
                options.BaseAddress = new Uri(Configuration.GetValue<string>("AppSettings:RailApiUri"));
                options.DefaultRequestHeaders.Add("ocp-apim-subscription-key", Configuration.GetValue<string>("AppSettings:RailSubscriptionKey"));
            });
            services.AddTransient<IRail, RailRepository>();
            services.AddTransient<IGoogle, GoogleApiRepositoryWithGates>();
            services.AddTransient<IRailRouteBuilder, RailRoutesBuilder>();
            services.AddSingleton<IStaticStations, StaticStationsRepository>();
            services.AddSingleton<ITime, TimeRepository>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRequestTelemetryEnhancer();
            app.UseOperationIdHeader();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
