using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Models;
using IdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace IdentityApp {

    public class Startup {

        public Startup(IConfiguration config) => Configuration = config;

        private IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDbContext<ProductDbContext>(opts => {
                opts.UseSqlServer(
                    Configuration["ConnectionStrings:AppDataConnection"]);
            });

            services.AddHttpsRedirection(opts => {
                opts.HttpsPort = 44350;
            });

            services.AddDbContext<IdentityDbContext>(opts =>
            {
                opts.UseSqlServer(Configuration["ConnectionStrings:IdentityConnection"],
                    opts => opts.MigrationsAssembly("IdentityApp")
                );
            });

            services.AddScoped<IEmailSender, ConsoleEmailSender>();

            services.AddIdentity<IdentityUser, IdentityRole>(opts =>
                {
                    opts.Password.RequiredLength = 8;
                    opts.Password.RequireDigit = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireUppercase = false;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.SignIn.RequireConfirmedAccount = true;
                    opts.Lockout.MaxFailedAccessAttempts = 5;
                }).AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<SecurityStampValidatorOptions>(opts => {
                opts.ValidationInterval = System.TimeSpan.FromMinutes(1);
            });

            services.AddScoped<TokenUrlEncoderService>();
            services.AddScoped<IdentityEmailService>();

            services.AddAuthentication()
                .AddFacebook(opts =>
                {
                    opts.AppId = Configuration["Facebook:AppId"];
                    opts.AppSecret = Configuration["Facebook:AppSecret"];
                });

            services.AddAuthentication()
                .AddGoogle(opts =>
                {
                    opts.ClientId = Configuration["Google:ClientId"];
                    opts.ClientSecret = Configuration["Google:ClientSecret"];
                });

            services.ConfigureApplicationCookie(opts => {
                opts.LoginPath = "/Identity/SignIn";
                opts.LogoutPath = "/Identity/SignOut";
                opts.AccessDeniedPath = "/Identity/Forbidden";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });

            app.SeedUserStoreForDashboard();
        }
    }
}
