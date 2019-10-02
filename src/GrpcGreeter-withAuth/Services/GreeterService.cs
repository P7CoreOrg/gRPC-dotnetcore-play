using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcGreeter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace GrpcGreeter_withAuth
{
    [Authorize]
    public class GreeterService : Greeter.GreeterBase
    {
        private ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }
        public override Task<HelloReply> SayHello(
            HelloRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            var name = (from item in user.Claims
                       where item.Type == ClaimTypes.Name
                       select item.Value).FirstOrDefault();
            var reply = new HelloReply
            {
                Message = $"user.Name:{name} - Hello " + request.Name
            };

            _logger.LogInformation(reply.Message);
            return Task.FromResult(reply);
        }
        public override Task<HelloReply> SayHelloThrow(HelloRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            throw new Exception($"user:{user} - oh my, Derek said it would work!");
        }
    }
}
