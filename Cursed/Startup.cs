using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Services;
using Cursed.Models.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.IO;
using System.Data.SqlClient;

namespace Cursed
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        private IWebHostEnvironment CurrentEnvironment { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dataDatabaseConnection = Configuration.GetConnectionString("DataDatabaseConnection");
            var authDatabaseConnection = Configuration.GetConnectionString("AuthenticationDatabaseConnection");

            services.AddDbContext<CursedDataContext>(options => options.UseSqlServer(dataDatabaseConnection));
            services.AddDbContext<CursedAuthenticationContext>(options => options.UseSqlServer(authDatabaseConnection));

            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(15);
            });

            services.AddSingleton<ILicenseValidation, LicenseValidation>();
            services.AddSingleton<IErrorHandlerFactory, StatusMessageFactory>();
            services.AddSingleton<IGenPasswordHash, PasswordHash>();
            services.AddScoped<IOperationDataValidation, OperationDataValidation>();
            services.AddTransient<ILogProvider<CursedAuthenticationContext>, LogProvider>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/login");
                    options.AccessDeniedPath = new PathString("/access-denied");
                    options.LogoutPath = new PathString("/login");
                    options.ExpireTimeSpan = TimeSpan.FromDays(10);
                });

            services.AddAuthorization();

            // initialize databases if we're in development
            if (CurrentEnvironment.IsDevelopment())
            {
                string dbAuthDirectory = Path.Combine(Environment.CurrentDirectory, "Properties", "dbAuth");
                string dbDataDirectory = Path.Combine(Environment.CurrentDirectory, "Properties", "dbData");

                var localServerConnection = Configuration.GetConnectionString("LocalServerConnection");

                // initialize data database
                DatabaseInitializer.ExecuteScript(localServerConnection, Path.Combine(dbDataDirectory, "createDB.sql"));
                DatabaseInitializer.ExecuteScript(dataDatabaseConnection, Path.Combine(dbDataDirectory, "scheme.sql"));
                DatabaseInitializer.ExecuteScript(dataDatabaseConnection, Path.Combine(dbDataDirectory, "data.sql"));
                // initialize auth database
                DatabaseInitializer.ExecuteScript(localServerConnection, Path.Combine(dbAuthDirectory, "createDB.sql"));
                DatabaseInitializer.ExecuteScript(authDatabaseConnection, Path.Combine(dbAuthDirectory, "scheme.sql"));
                DatabaseInitializer.ExecuteScript(authDatabaseConnection, Path.Combine(dbAuthDirectory, "data.sql"));
            }
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
                app.UseExceptionHandler("/hub/error");
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

    static class DatabaseInitializer
    {
        public static void ExecuteScript(string connectionString, string pathToScript)
        {
            string script = File.ReadAllText(pathToScript);

            SqlConnection conn = new SqlConnection(connectionString);

            Server server = new Server(new ServerConnection(conn));

            server.ConnectionContext.ExecuteNonQuery(script);
        }
    }
}
