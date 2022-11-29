using Core.TableDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static NetEscapades.Extensions.Logging.RollingFile.PeriodicityOptions;

namespace Salony
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args)
                .ConfigureLogging(log =>
                {
                    log.AddFile(file =>
                    {
                        file.FileName = $"Log";
                        file.LogDirectory = "Logs";
                        file.Extension = "txt";
                        file.Periodicity = Daily;
                    });
                })
                .Build();

            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                try
                {
                    UserManager<ApplicationDbUser> userManager = services.GetRequiredService<UserManager<ApplicationDbUser>>();
                    RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await Salony.Seeds.DefaultRoles.SeedAsync(userManager, roleManager);
                    await Salony.Seeds.DefaultBasicUser.SeedAsync(userManager, roleManager);
                }
                catch (Exception)
                {

                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
