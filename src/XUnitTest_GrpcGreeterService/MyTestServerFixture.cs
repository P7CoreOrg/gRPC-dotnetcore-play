using GrpcGreeter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TestServerFixture;

namespace XUnitTest_GrpcGreeterService
{
    public class MyTestServerFixture : TestServerFixture<Startup>
    {
        protected override string RelativePathToHostProject => @"../../../../GrpcGreeter";
        protected override void ConfigureAppConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
            Program.LoadConfigurations(config, environmentName);
            //   config.AddJsonFile($"appsettings.TestServer.json", optional: false);
        }
    }
}
