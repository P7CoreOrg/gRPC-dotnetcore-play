using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;

namespace TestServerFixture
{
    public abstract class TestServerFixture<TStartup> :
        ITestServerFixture
        where TStartup : class
    {
        private string _environmentUrl;
        public bool IsUsingInProcTestServer { get; set; }

        public HttpMessageHandler MessageHandler { get; }
        public TestServer TestServer { get; }

        // RelativePathToHostProject = @"..\..\..\..\TheWebApp";
        protected abstract string RelativePathToHostProject { get; }

        public TestServerFixture()
        {
            var contentRootPath = GetContentRootPath();
            var builder = new WebHostBuilder();

            builder.UseContentRoot(contentRootPath)
                .UseEnvironment("Development")
                .ConfigureAppConfiguration(configureDelegate =>
                {
                })
                .ConfigureServices(services =>
                {
                    ConfigureServices(services); // to be overriden
                     
                })
                .ConfigureAppConfiguration(ConfigureAppConfiguration);
            UseSettings(builder);

            builder.UseStartup<TStartup>(); // Uses Start up class from your API Host project to configure the test server
            string environmentUrl = Environment.GetEnvironmentVariable("TestEnvironmentUrl");
            IsUsingInProcTestServer = false;
            if (string.IsNullOrWhiteSpace(environmentUrl))
            {
                environmentUrl = "http://localhost/";

                TestServer = new TestServer(builder);

                MessageHandler = TestServer.CreateHandler();
                IsUsingInProcTestServer = true;

                // We need to suppress the execution context because there is no boundary between the client and server while using TestServer
                MessageHandler = new SuppressExecutionContextHandler(MessageHandler);
            }
            else
            {
                if (environmentUrl.Last() != '/')
                {
                    environmentUrl = $"{environmentUrl}/";
                }
                MessageHandler = new HttpClientHandler();
            }

            _environmentUrl = environmentUrl;

        }

        protected abstract void ConfigureAppConfiguration(
            WebHostBuilderContext hostingContext,
            IConfigurationBuilder config);

        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

        protected virtual void UseSettings(WebHostBuilder builder)
        {
        }

        protected virtual void ConfigureAppConfiguration(IConfigurationBuilder configureDelegate)
        {
        }


        public HttpClient Client =>
            new HttpClient(new SessionMessageHandler(MessageHandler))
            {
                BaseAddress = new Uri(_environmentUrl)
            };

        private string GetContentRootPath()
        {
            var testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;
            return Path.Combine(testProjectPath, RelativePathToHostProject);
        }

    }
}
