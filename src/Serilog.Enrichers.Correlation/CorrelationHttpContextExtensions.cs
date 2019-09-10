using Microsoft.Extensions.DependencyInjection;
using Serilog.Configuration;
using System;

namespace Serilog.Enrichers.Correlation
{
    public static class CorrelationHttpContextExtensions
    {

        public static IServiceCollection AddCorrelationEnricher(this IServiceCollection services)
        {
            services.AddScoped<CorrelationHttpContextModel>();
            services.AddScoped<CorrelationHttpContextEnricher>();

            return services;
        }

        public static LoggerConfiguration WithCorrelationHttpContext(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration, IServiceProvider services)
        {
            return loggerEnrichmentConfiguration.With(services.GetService<CorrelationHttpContextEnricher>());
        }
    }
}
