using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersUsingIdentityApi.Models;
using UsersUsingIdentityApi.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace UsersUsingIdentityApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPasswordValidator<AppUser>,CustomPasswordValidator>();
            services.AddTransient<IUserValidator<AppUser>, CustomUserValidator>();
            services.AddSingleton<IClaimsTransformation,LocationClaimsProvider>();
            services.AddSingleton<IAuthorizationHandler, BlockUsersHandler>();
            //services.AddTransient<IAuthorizationHandler, DocumentAuthorizationHandler>();


            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("DCUsers", policy =>
                {
                    policy.RequireRole("Users");
                    policy.RequireClaim(ClaimTypes.StateOrProvince, "DC");
                });
                opts.AddPolicy("NotBob", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new BlockUsersRequirement("Bob"));
                });
                //opts.AddPolicy("AuthorsAndEditors", policy => {
                //    policy.AddRequirements(new DocumentAuthorizationRequirement
                //    {
                //        AllowAuthors = true,
                //        AllowEditors = true
                //    });
                //});
            });

            services.AddAuthentication().AddGoogle(opts =>
            {
                opts.ClientId = "651391242865-lb7ip1ijlhcqabf71m69o8kh8cb2ev6d.apps.googleusercontent.com";
                opts.ClientSecret = "C-bgs3pBaHJB3Ws3_5nfLQRm";
            });

            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(
                        Configuration["Data:SportStoreIdentity:ConnectionString"]));

            services.AddIdentity<AppUser, IdentityRole>(opts => {
                opts.User.RequireUniqueEmail = true;
                //opts.User.AllowedUserNameCharacters = "abcdefgijklmnopqrstuvwxyz";
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            //services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Users/Login");        
        }
               
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStatusCodePages();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            //AppIdentityDbContext.CreateAdminAccount(app.ApplicationServices, Configuration).Wait();
        }
    }
}
