using CorrelationId;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Enrichers.Correlation
{
    public class CorrelationHttpContextEnricher : ILogEventEnricher
    {

        private ICorrelationContextAccessor _correlationContextAccessor;

        public CorrelationHttpContextEnricher(ICorrelationContextAccessor correlationContextAccessor)
        {
            _correlationContextAccessor = correlationContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (this._correlationContextAccessor == null || this._correlationContextAccessor.CorrelationContext == null)
            {
                return;
            }
            dynamic o = new
            {
                @corr = this._correlationContextAccessor.CorrelationContext.CorrelationId
            };
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Correlation", o, true));
        }
    }
}
