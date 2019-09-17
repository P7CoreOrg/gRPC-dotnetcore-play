using Grpc.Reflection;
using Microsoft.AspNetCore.Routing;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add gRPC service endpoints.
    /// </summary>
    public static class GrpcReflectionEndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Maps incoming requests to the gRPC reflection service.
        /// This service can be queried to discover the gRPC services on the server.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <returns>An <see cref="IEndpointConventionBuilder"/> for endpoints associated with the service.</returns>
        public static IEndpointConventionBuilder MapGrpcReflectionService(this IEndpointRouteBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.MapGrpcService<ReflectionServiceImpl>();
        }
    }
}
