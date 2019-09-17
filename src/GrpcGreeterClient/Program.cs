using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Reflection.V1Alpha;
using GrpcGreeter;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static Grpc.Reflection.V1Alpha.ServerReflection;

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
            var serverReflectionClient = new ServerReflectionClient(insecureChannel);
            var response = await SingleRequestAsync(serverReflectionClient, new ServerReflectionRequest
            {
                ListServices = "" // Get all services
            });
            Console.WriteLine("Services:");
            foreach (var item in response.ListServicesResponse.Service)
            {
                Console.WriteLine("- " + item.Name);
            }

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
        private static async Task<ServerReflectionResponse> SingleRequestAsync(ServerReflectionClient client, ServerReflectionRequest request)
        {
            var call = client.ServerReflectionInfo();
            await call.RequestStream.WriteAsync(request);
            Debug.Assert(await call.ResponseStream.MoveNext());

            var response = call.ResponseStream.Current;
            await call.RequestStream.CompleteAsync();
            return response;
        }
    }
}
