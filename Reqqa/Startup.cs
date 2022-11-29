using Core.Interfaces;
using Core.TableDb;
using DinkToPdf;
using DinkToPdf.Contracts;
using Infrastructure;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Salony.Hubs;
using Salony.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Salony
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }


        //string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            #region identiy
            services.AddDefaultIdentity<ApplicationDbUser>(options =>
            {
                // Default Password settings.

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                // options.SignIn.RequireConfirmedEmail = true;
            }).AddRoles<IdentityRole>().AddDefaultUI().AddEntityFrameworkStores<ApplicationDbContext>();

            #endregion
            #region  jwt
            //فى جزء فى ملف الjson
            services.AddAuthorization();

            services.AddAuthentication(
                //option =>
                //{
                //    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                //}
                ).AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = Configuration["Jwt:Site"],
                        ValidIssuer = Configuration["Jwt:Site"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SigningKey"]))
                    };
                });
            #endregion
            #region Swagger
            services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigration>();
            services.AddSingleton<IConfigureOptions<SwaggerUIOptions>, SwaggerUIConfiguration>();
            services.AddSwaggerGen();
            #endregion

            services.AddScoped<IChatServices, ChatServices>();

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = System.TimeSpan.FromDays(30);
            });

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSignalR();

            #region Identity Time Out
            services.AddDataProtection()
        .SetApplicationName($"my-app-{Environment.EnvironmentName}")
        .PersistKeysToFileSystem(new DirectoryInfo($@"{Environment.ContentRootPath}\keys"));
            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(300);
            });

            #endregion



            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                                  });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //app.UseCors(MyAllowSpecificOrigins);
            //app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            #region Swagger

            app.UseSwagger();

            app.UseSwaggerUI();

            #endregion
            app.UseHttpsRedirection();
            //app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
                      "Origin, X-Requested-With, Content-Type, Accept");
                },

            });


            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //        endpoints.MapGet("/echo",
                //context => context.Response.WriteAsync("echo"))
                //.RequireCors(MyAllowSpecificOrigins);

                //        endpoints.MapControllers()
                //     .RequireCors(MyAllowSpecificOrigins);

                //        endpoints.MapGet("/echo2",
                //context => context.Response.WriteAsync("echo2"));





                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<ChatHub>("/chatHub");

                endpoints.MapRazorPages();
            });


        }
    }
}