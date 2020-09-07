using System;
using FluentMessenger.API.DBContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FluentMessenger.API {
    public class Program {
        public static void Main(string[] args) {
            var host = CreateHostBuilder(args).Build();
            ApplyMigrations(host);
            host.Run();
        }

        private static void ApplyMigrations(IHost host) {
            // migrate the database.  Best practice = in Main, using service scope
            using (var scope = host.Services.CreateScope()) {
                try {
                    var context = scope.ServiceProvider.GetService<FluentDbContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex) {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
		        .ConfigureWebHostDefaults(webBuilder => {
                var port=Environment.GetEnvironmentVariable("PORT")?? "8080";
                    webBuilder.UseStartup<Startup>().UseUrls("http://*:" + $"{port}");
            });
    }
}
