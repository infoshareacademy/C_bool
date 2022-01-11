﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using C_bool.BLL.DAL.Context;
using C_bool.BLL.DAL.Entities;
using C_bool.BLL.Repositories;
using C_bool.BLL.Services;
using C_bool.WebApp.Config;
using C_bool.WebApp.Services;
using Microsoft.EntityFrameworkCore;

namespace C_bool.WebApp
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
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddDbContext<ApplicationDbContext>(o => 
                o.UseSqlServer(
                    Configuration.GetConnectionString("Database")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            

            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddControllersWithViews();
            
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //services.AddSingleton<IPlacesRepository, PlacesRepository>();
            services.AddSingleton<PlacesService>();
            services.AddTransient<IUserService, UsersService>();
            
            services.AddHttpClient("GoogleMapsClient", client =>
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com/");
                client.Timeout = new TimeSpan(0, 0, 30);
                client.DefaultRequestHeaders.Clear();
            });
            
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            dataContext.Database.Migrate();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}