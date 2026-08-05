using System.Net;
using System.Net.Http.Headers;

namespace LoanMVC.Handlers
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);   //401

            if (response.StatusCode == HttpStatusCode.Unauthorized && _httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Session.Clear();

                _httpContextAccessor.HttpContext.Response.Cookies.Append(
                    "SessionExpiredElsewhere", "1", new CookieOptions { MaxAge = TimeSpan.FromMinutes(2) });
            }

            return response;
        }
    }
}