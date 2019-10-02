using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GrpcGreeter_withAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                    Host.CreateDefaultBuilder(args)
                        .ConfigureAppConfiguration((hostingContext, config) =>
                        {
                            var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                            LoadConfigurations(config, environmentName);
                            config.AddUserSecrets<Startup>();
                            config.AddEnvironmentVariables();
                        })
                        .ConfigureWebHostDefaults(webBuilder =>
                        {
                            webBuilder.UseStartup<Startup>()
                            .UseSerilog()
                            .UseSentry();
                        });
        public static void LoadConfigurations(IConfigurationBuilder config, string environmentName)
        {
            config
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: false, reloadOnChange: true);
        }
    }
}
