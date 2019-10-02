using Grpc.Core;
using Grpc.Net.Client;
using GrpcGreeter;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace GrpcGreeterClient_withAuth
{
    class Program
    {
        // The port number(4731) must match the port of the gRPC server.
        private const string Address = "localhost:4730";

        public static object GrpcClient { get; private set; }

        static async Task Main(string[] args)
        { 
            // This switch must be set before creating the GrpcChannel/HttpClient.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var httpClient = new HttpClient { BaseAddress = new Uri($"http://{Address}") };
            var insecureChannel = GrpcChannel.ForAddress($"http://{Address}", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
                HttpClient = httpClient
            });
            var insecureclient = new Greeter.GreeterClient(insecureChannel);

            Console.WriteLine("gRPC Greeter-withAuth");
            Console.WriteLine();
            Console.WriteLine("Press a key:");
            Console.WriteLine("1: Hello");
            Console.WriteLine("2: Hello Throw");
            Console.WriteLine("3: Authenticate");
            Console.WriteLine("4: Exit");
            Console.WriteLine();

            string token = null;

            var exiting = false;
            while (!exiting)
            {
                var consoleKeyInfo = Console.ReadKey(intercept: true);
                switch (consoleKeyInfo.KeyChar)
                {
                    case '1':
                        await SayHelloAsync(insecureclient, token);
                        break;
                    case '2':
                        await SayHelloThrowAsync(insecureclient, token);
                        break;
                    case '3':
                        token = await Authenticate();
                        break;
                    case '4':
                        exiting = true;
                        break;
                }
            }

            Console.WriteLine("Exiting");
        }
        private static async Task SayHelloAsync(Greeter.GreeterClient client, string token)
        {
            Console.WriteLine("Saying Hello ...");
            try
            {
                Metadata? headers = null;
                if (token != null)
                {
                    headers = new Metadata
                    {
                        { "Authorization", $"Bearer {token}" }
                    };
                }

                
                var response = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" }, headers);
                Console.WriteLine($"Response:{response.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Saying Hello." + Environment.NewLine + ex.ToString());
            }
        }
        private static async Task SayHelloThrowAsync(Greeter.GreeterClient client, string token)
        {
            Console.WriteLine("Saying Hello Throw ...");
            try
            {
                Metadata? headers = null;
                if (token != null)
                {
                    headers = new Metadata
                    {
                        { "Authorization", $"Bearer {token}" }
                    };
                }


                var response = await client.SayHelloThrowAsync(new HelloRequest { Name = "GreeterClient" }, headers);
                Console.WriteLine($"Response:{response.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Saying Hello." + Environment.NewLine + ex.ToString());
            }
        }
        private static async Task<string> Authenticate()
        {
            Console.WriteLine($"Authenticating as {Environment.UserName}...");
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"http://{Address}/generateJwtToken?name={HttpUtility.UrlEncode(Environment.UserName)}"),
                Method = HttpMethod.Get,
                Version = new Version(2, 0)
            };
            var tokenResponse = await httpClient.SendAsync(request);
            tokenResponse.EnsureSuccessStatusCode();

            var token = await tokenResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Successfully authenticated.");
            Console.WriteLine("== TOKEN BEGIN ==");
            Console.WriteLine(token);
            Console.WriteLine("== TOKEN END ==");
            return token;
        }
    }
}
