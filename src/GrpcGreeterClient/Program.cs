using Grpc.Core;
using Grpc.Net.Client;
using GrpcGreeter;
using System;
using System.Threading.Tasks;

namespace GrpcGreeterClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcGreeter.HelloReply reply = null;
            // This switch must be set before creating the GrpcChannel/HttpClient.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var insecureChannel = GrpcChannel.ForAddress("http://localhost:4700", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
                HttpClient = new System.Net.Http.HttpClient()
                {
                    BaseAddress = new Uri("http://localhost:4700")
                }
            });

           
            var insecureclient = new Greeter.GreeterClient(insecureChannel);
            try
            {
                reply = await insecureclient.SayHelloAsync(
                            new HelloRequest { Name = "GreeterClient" });
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            var secureChannel = GrpcChannel.ForAddress("https://localhost:4701");
            var secureClient = new Greeter.GreeterClient(secureChannel);
            reply = await secureClient.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);

            try
            {
                reply = await secureClient.SayHelloThrowAsync(
                           new HelloRequest { Name = "GreeterClient" });
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
          
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
