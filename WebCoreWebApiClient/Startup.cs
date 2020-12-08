using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebCoreWebApiClient.Infrastructure.PartialRenderString;
using WebCoreWebApiClient.Infrastructure.Signalr;
using WebCoreWebApiClient.Models.Http.Client;

namespace WebCoreWebApiClient
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

           
        
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(45);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });
            services.AddControllersWithViews();
            services.AddSignalR();
            services.AddRazorPages();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            //Using Name property type => 1
            var apibaseUrl = Configuration.GetSection("ApiBaseUrl").Value;
            services.AddHttpClient("MyWebApiClient",options=> {
                options.BaseAddress = new Uri(apibaseUrl);
                options.DefaultRequestHeaders.Add("ContenType", "application/json");
            });

            services.AddHttpClient <MyTypeWebClient>(client =>
            {
                client.BaseAddress = new Uri(apibaseUrl);
            });
            
            //Register the service that render a partial view into html string! 
            services.AddScoped<IPartialRender, PartialRender>();

            services.ConfigureApplicationCookie(options =>
            { 
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                options.LoginPath = new PathString("/Account/Login");
               
               
            });
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //set the base address if hosted in a sub folder in iis use base address apiclient

            app.UsePathBase("/apiclient");
            app.Use((context, next) =>
            {
                context.Request.PathBase = "/apiclient";
                return next();
            });

            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Index}/{id?}");


            });

           
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
