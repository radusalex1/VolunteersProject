using MailServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VolunteersProject.Common;
using VolunteersProject.Data;
using VolunteersProject.Repository;
using VolunteersProject.Services;

namespace VolunteersProject
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<VolunteersContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddSession();

            services.AddControllersWithViews();

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            //https://stackoverflow.com/questions/54461127/how-to-return-403-instead-of-redirect-to-access-denied-when-authorizefilter-fail/54461389
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.Events.OnRedirectToAccessDenied = context =>
                        {
                            context.Response.StatusCode = 403;
                            return Task.CompletedTask;
                        };
                    });

            // Register the Swagger generator, defining 1 or more Swagger documents
            //services.AddSwaggerGen();           

            //Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    //Title = "Swagger for Volunteers project API",
                    Title = $"{Configuration["SwaggerDocs_ProductName"]} API",
                    //Description = "A simple example ASP.NET Core Web API",
                    //TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        //Name = "Shayne Boyer",
                        Name = Configuration["SwaggerDocs_ContactName"],
                        //Email = string.Empty,
                        Email = Configuration["SwaggerDocs_ContactEmail"]
                        //Url = new Uri("https://twitter.com/spboyer"),
                    },
                    License = new OpenApiLicense
                    {
                        //Name = "Use under LICX",
                        //Url = new Uri("https://example.com/license"),
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);

                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });

                c.DocInclusionPredicate((name, api) => true);
            });

            services.AddTransient<IVolunteerRepository, VolunteerRepository>();
            services.AddTransient<IContributionRepository, ContributionRepository>();
            services.AddTransient<IEnrollmentRepository, EnrollmentRepository>();
            services.AddTransient<IRolesRepository, RolesRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<ITokenService, TokenService>();

            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddTransient<IEmailService, EmailService>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/mylog-{Date}.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();

            app.Use(async (context, next) =>
            {               
                var token = ApplicationValues.JwtToken;

                if (!string.IsNullOrEmpty(token))
                {
                    context.Session.SetString("userIsLogged", "true");

                    context.Request.Headers.Add("Authorization", "Bearer " + token);
                }

                await next();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            //Enable middleware to serve swagger - ui(HTML, JS, CSS, etc.),
            //specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //todo cia - check if this is need
            //custom jwt auth middleware
            //app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=account}/{action=Login}/{id?}");
            });
        }
    }
}
