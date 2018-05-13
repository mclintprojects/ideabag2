using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProgrammingIdeas.Api
{
    internal class TokenDelegate : DelegatingHandler
    {
        public TokenDelegate(HttpMessageHandler handler) : base(handler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                Global.Logout();

            return response;
        }
    }
}