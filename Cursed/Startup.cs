using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Services;
using Cursed.Models.Context;
using Cursed.Models.Data.Utility.ErrorHandling;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Cursed
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
            services.AddControllersWithViews();
            services.AddDbContext<CursedDataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DataDatabaseConnection")));
            services.AddDbContext<CursedAuthenticationContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AuthenticationDatabaseConnection")));
            services.AddHttpContextAccessor();
            services.AddSingleton<ILicenseValidation, LicenseValidation>();
            services.AddSingleton<IErrorHandlerFactory, StatusMessageFactory>();
            services.AddSingleton<IGenPasswordHash, PasswordHash>();
            services.AddScoped<IOperationDataValidation, OperationDataValidation>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/login");
                    options.AccessDeniedPath = new PathString("/access-denied");
                    options.LogoutPath = new PathString("/login");
                    options.ExpireTimeSpan = TimeSpan.FromDays(10);
                });

            services.AddAuthorization(opts =>
            {
                //opts.AddPolicy("MinimumTier10", policy => policy.Requirements.Add(new TierEqualOrHigher(10)));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
