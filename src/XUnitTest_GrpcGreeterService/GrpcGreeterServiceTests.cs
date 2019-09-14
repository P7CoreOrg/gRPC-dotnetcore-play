using FluentAssertions;
using Grpc.Net.Client;
using GrpcGreeter;
using System;
using Xunit;

namespace XUnitTest_GrpcGreeterService
{
    public class GrpcGreeterServiceTests : IClassFixture<MyTestServerFixture>
    {
        private readonly MyTestServerFixture _fixture;
        private static string GuidS => Guid.NewGuid().ToString("N");
        public GrpcGreeterServiceTests(MyTestServerFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public void AssureFixture()
        {
            _fixture.Should().NotBeNull();
            var client = _fixture.Client;
            client.Should().NotBeNull();
        }

        [Fact]
        public async void exception_SayHelloThrow()
        {
            GrpcChannelOptions options = new GrpcChannelOptions()
            {
                HttpClient = _fixture.Client
            };
            var channel = GrpcChannel.ForAddress("http://localhost", options);
            var greeterClient = new Greeter.GreeterClient(channel);

            Action act = () =>
            {
                greeterClient.SayHelloThrowAsync(
                            new HelloRequest { Name = "GreeterClient" }).GetAwaiter().GetResult();
            };
            act.Should().Throw<Grpc.Core.RpcException>();

        }
        [Fact]
        public async void success_SayHello()
        {
            GrpcChannelOptions options = new GrpcChannelOptions()
            {
                HttpClient = _fixture.Client
            };
            var channel = GrpcChannel.ForAddress("http://localhost", options);
            var greeterClient = new Greeter.GreeterClient(channel);

            var reply = await greeterClient.SayHelloAsync(
                               new HelloRequest { Name = "GreeterClient" });
            reply.Message.Should().Equals("GreeterClient");

        }
    }
}
