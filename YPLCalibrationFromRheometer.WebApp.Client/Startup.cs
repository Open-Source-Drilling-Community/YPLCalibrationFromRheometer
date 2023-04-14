using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace YPLCalibrationFromRheometer.WebApp.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            // This needs to match with what is defined in "charts/<helm-chart-name>/templates/values.yaml ingress.Path
            app.UsePathBase("/YPLCalibrationFromRheometer/webapp");

            if (!String.IsNullOrEmpty(Configuration["YPLCalibrationHostURL"]))
                YPLCalibrationFromRheometer.WebApp.Client.Configuration.YPLCalibrationHostURL = Configuration["YPLCalibrationHostURL"];
            if (!String.IsNullOrEmpty(Configuration["DrillingUnitConversionHostURL"]))
                YPLCalibrationFromRheometer.WebApp.Client.Configuration.DrillingUnitConversionHostURL = Configuration["DrillingUnitConversionHostURL"];

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
    }
}
