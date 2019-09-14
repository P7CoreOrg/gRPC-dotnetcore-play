using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestServerFixture
{
    /// <summary>
    /// An <see cref="HttpMessageHandler"/> that maintains session consistency between requests.
    /// </summary>
    public class SessionMessageHandler : DelegatingHandler
    {
        private string _sessionToken;

        public SessionMessageHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_sessionToken))
            {
                request.Headers.TryAddWithoutValidation("x-ms-session-token", _sessionToken);
            }

            request.Headers.TryAddWithoutValidation("x-ms-consistency-level", "Session");

            var response = await base.SendAsync(request, cancellationToken);

            if (response.Headers.TryGetValues("x-ms-session-token", out var tokens))
            {
                _sessionToken = tokens.SingleOrDefault();
            }

            return response;
        }
    }
}